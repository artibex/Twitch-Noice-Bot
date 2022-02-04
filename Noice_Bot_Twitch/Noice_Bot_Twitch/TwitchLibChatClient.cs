using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Noice_Bot_Twitch
{
    public static class TwitchLibChatClient
    {
        public static TwitchClient client;

        public static void LoadSettings()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(FileManager.GetChannelName(), FileManager.GetOAuth());
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, FileManager.GetChannelName());

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;

            client.Connect();
        }

        public static void SendMessage(string message)
        {
            Console.WriteLine(message);
            client.SendMessage(FileManager.GetChannelName(), message);
        }

        private static void Client_OnLog(object sender, OnLogArgs e)
        {
            //Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private static void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private static void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("NoiceBot Ready!");
            client.SendMessage(e.Channel, "NoiceBot Ready!");
        }

        private static void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            //Console.WriteLine(e.ChatMessage.DisplayName + " " + e.ChatMessage.Message);
            //if (e.ChatMessage.Message.Contains("badword"))
            //    client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "Bad word! 30 minute timeout!");
        }

        private static void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "my_friend")
                client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private static void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                client.SendMessage(e.Channel, $"Danke {e.Subscriber.DisplayName} für den PrimeSub!");
            else
                client.SendMessage(e.Channel, $"Danke {e.Subscriber.DisplayName} für den Sub!");
        }

    }
}
