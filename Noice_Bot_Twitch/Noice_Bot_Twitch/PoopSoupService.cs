using System;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using System.Timers;
using System.Threading;

namespace Noice_Bot_Twitch 
{ 
    //Check Channel Points Redemption of the given Channel ID, Can also be used to check Bit's donations etc.
    public class PoopSoupService : IDisposable
    {
        //FileManager fm; //Get Settings
        //CommandIdentifier ci; //Trigger here a sound playing
        TwitchPubSub pubsub; //Pub Sub to get Events like Bits, Channel Points etc.
        private System.Timers.Timer _timer; //Timer
        public bool disconnected = false;

        public void LoadSettings()
        {
            pubsub = new TwitchPubSub(); //init PubSub
            //Connect PubSub events with custom Methods
            pubsub.OnPubSubServiceConnected += Pubsub_OnPubSubServiceConnected;
            pubsub.OnPubSubServiceClosed += Pubsub_OnPubSubServiceDisconnect; //Reconnect if connection is lost
            pubsub.OnPubSubServiceError += Pubsub_OnPubSubServiceError; //Reconnect if error
            pubsub.OnListenResponse += Pubsub_OnListenResponse;
            pubsub.OnRewardRedeemed += Pubsub_OnRewardRedeemed;
            //pubsub.OnBitsReceived += Pubsub_OnBitsReceived; //Unused
            //pubsub.OnLog += PubSub_OnLog; //Pure Logging Function of PubSub //Unused
            //pubsub.OnWhisper += Pubsub_OnWhisper; //Unused
            pubsub.Connect(); //Connect
            CommandIdentifier.pubsub = this;
            _timer = new System.Timers.Timer(500000);
            _timer.Elapsed += Reconnect;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public void Reconnect(Object source, ElapsedEventArgs e) //Renew every 10 minutes pubsub connection
        {
            pubsub.ListenToRewards(FileManager.GetChannelID()); //Set Channel ID
            //Thread.Sleep(1000);
            pubsub.SendTopics(FileManager.GetOAuth(), false); //Give OAuth Key and connect
            pubsub.Connect();
        }
        //public void RenewPubSub()
        //{
        //    pubsub.ListenToRewards(FileManager.GetChannelID()); //Set Channel ID
        //    pubsub.SendTopics(FileManager.GetOAuth(), false); //Give OAuth Key and connect
        //    _timer = new System.Timers.Timer(40000);
        //    _timer.Elapsed += Reconnect;
        //    _timer.Enabled = true;
        //}

        //Pure debugging with every information
        void PubSub_OnLog(object sender, OnLogArgs a)
        {
            Console.WriteLine(a.Data);
        }

        //Authentication with OAuth and to connect to what ChannelID
        private void Pubsub_OnPubSubServiceConnected(object sender, System.EventArgs e)
        {
            pubsub.ListenToRewards(FileManager.GetChannelID()); //Set Channel ID
            pubsub.SendTopics(FileManager.GetOAuth(), false); //Give OAuth Key and connect
        }

        private void Pubsub_OnPubSubServiceError(object sender, System.EventArgs e)
        {
            Console.WriteLine("Some Pubsub Error Occured, try to reconnect");
            Console.WriteLine(e);
            //RenewPubSub();
        }
        private void Pubsub_OnPubSubServiceDisconnect(object sender, System.EventArgs e)
        {
            Console.WriteLine("PubSub got Disconnected, try to reconnect");
            Console.WriteLine(e);
            //RenewPubSub();
        }


        //Give feedback if connected sucessfully or failed
        private void Pubsub_OnListenResponse(object sender, OnListenResponseArgs e)
        {
            if (!e.Successful)
                throw new Exception($"Failed to listen! Response: {e.Topic}");
            else Console.WriteLine("Connected to PubSub");
        }

        //Unused
        private void Pubsub_OnWhisper(object sender, OnWhisperArgs e)
        {
            Console.WriteLine("Whisper recived");
        }

        //Test Method to see bit redemption
        private void Pubsub_OnBitsReceived(object sender, OnBitsReceivedArgs e)
        {
            Console.WriteLine($"Just received {e.BitsUsed} bits from {e.Username}. That brings their total to {e.TotalBitsUsed} bits!");
        }
        //Channel Point redemption, Check if redemption is connected to code
        private void Pubsub_OnRewardRedeemed(object sender, OnRewardRedeemedArgs e)
        {
            CommandIdentifier.CheckCommand(e);
            //RewardRedeemedDebug(e); //Log everything of a channel point redemption
        }

        //Pure Debug to see what containes a OnRewardRedeemedArgs
        void RewardRedeemedDebug(OnRewardRedeemedArgs e)
        {
            Console.WriteLine("Points redeemed");
            Console.WriteLine(e.Login);
            Console.WriteLine(e.Message);
            Console.WriteLine(e.RewardCost);
            Console.WriteLine(e.RewardId);
            Console.WriteLine(e.RewardPrompt);
            Console.WriteLine(e.RewardTitle);
            Console.WriteLine(e.Status);
            Console.WriteLine(e.TimeStamp);
        }

        public void Dispose()
        {
            disconnected = true;
            
            pubsub.Disconnect();
            //I need this to dispose stuff and don't get errors
        }

    }
}
