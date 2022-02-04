using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace Noice_Bot_Twitch
{
    //Manages the connection to the chat
    public class IrcClient
    {
        private string userName;
        private string channel;

        private TcpClient tcpClient;
        private StreamReader inputStream;
        private StreamWriter outputStream;

        public IrcClient(string ip, int port, string userName, string password, string channel)
        {
            this.userName = userName;
            this.channel = channel;

            tcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());

            outputStream.WriteLine($"PASS {password}");
            outputStream.WriteLine($"NICK {userName}");
            outputStream.WriteLine($"USER {userName} 8 * :{userName}");
            outputStream.WriteLine($"JOIN #{channel}");
            outputStream.Flush();
        }

        public void SendIrcMessage(string message)
        {
            outputStream.WriteLine(message);
            outputStream.Flush();
        }

        public string ReadMessage()
        {
            Console.WriteLine(inputStream.ReadLine());
            return inputStream.ReadLine();
        }

        public void SendChatMessage(string message)
        {
            SendIrcMessage($":{userName}!{userName}@{userName}.tmi.twitch.tv PRIVMSG #{channel} :{message}");
        }
    }
}
//© 2020 GitHub, Inc.
//https://github.com/babelshift/SimpleTwitchBot 
