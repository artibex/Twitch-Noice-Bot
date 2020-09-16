using System;
using System.Diagnostics;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace Noice_Bot_Twitch 
{ 
    //Check Channel Points Redemption of the given Channel ID, Can also be used to check Bit's donations etc.
    class PubSubService    
    {
        FileManager fm; //Get Settings
        CommandIdentifier ci; //Trigger here a sound playing
        TwitchPubSub pubsub; //Pub Sub to get Events like Bits, Channel Points etc.

        public PubSubService(FileManager fm, CommandIdentifier ci)
        {
            this.fm = fm;
            this.ci = ci;
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
        }

        //Pure debugging with every information
        void PubSub_OnLog(object sender, OnLogArgs a)
        {
            Console.WriteLine(a.Data);
        }

        //Authentication with OAuth and to connect to what ChannelID
        private void Pubsub_OnPubSubServiceConnected(object sender, System.EventArgs e)
        {
            pubsub.ListenToRewards(fm.GetChannelID()); //Set Channel ID
            pubsub.SendTopics(fm.GetOAuth()); //Give OAuth Key and connect
        }

        private void Pubsub_OnPubSubServiceError(object sender, System.EventArgs e)
        {
            Console.WriteLine("Some Pubsub Error Occured, try to reconnect");
            Console.WriteLine(e);
            Pubsub_OnPubSubServiceConnected(sender, e);
        }
        private void Pubsub_OnPubSubServiceDisconnect(object sender, System.EventArgs e)
        {
            Console.WriteLine("PubSub got Disconnected, try to reconnect");
            Console.WriteLine(e);
            Pubsub_OnPubSubServiceConnected(sender, e);

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
            ci.CheckCommand(e);
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
    }
}
