using Newtonsoft.Json.Serialization;
using System;
using System.Timers;
using WebSocket4Net;

namespace Noice_Bot_Twitch
{
    public class PubSubService
    {
        WebSocket4Net.WebSocket socket;
        TwitchJsonBuilder tjb;
        Timer t;

        public PubSubService(TwitchJsonBuilder tjb)
        {
            this.tjb = tjb;
            LoadSettings();
        }

        public void LoadSettings()
        {
            socket = new WebSocket4Net.WebSocket("wss://pubsub-edge.twitch.tv");
            socket.Closed += PubSubClosed;
            socket.Error += PubSubError;
            socket.MessageReceived += PubSubMessageRecived;
            socket.Opened += PubSubConnected;
            socket.Open();
            t = new Timer(300000);
            t.AutoReset = true;
            t.Elapsed += SendPing;
            t.Enabled = true;
        }

        void PubSubConnected(object sender, EventArgs e)
        {
            socket.Send("\"type\": \"PING\"");
            socket.Send(tjb.GetRequest());
            Console.WriteLine("Connected to PoopSoup");
        }

        void SendPing(object source, ElapsedEventArgs e)
        {
            socket.Send("\"type\": \"PING\"");
        }

        void PubSubError(object sender, EventArgs e)
        {
            Console.WriteLine("Some PubSub Error occured");
        }

        void PubSubClosed(object sender, EventArgs e)
        {
            LoadSettings();
        }

        public void Disconnect()
        {
            socket.Dispose();
        }

        void PubSubMessageRecived(object sender, MessageReceivedEventArgs e)
        {
            dynamic redemption = Newtonsoft.Json.Linq.JObject.Parse(e.Message);
            CommandIdentifier.CheckCommand(redemption);
        }
    }
}
