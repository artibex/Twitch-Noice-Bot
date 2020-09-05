using System;
using System.Runtime.Remoting.Channels;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace Noice_Bot_Twitch 
{ 
    //Check Channel Points Redemption of the given Channel ID, Can also be used to check Bit's donations etc.
    class PubSubService    
    {
        FileManager fm;
        CommandIdentifier ci;
        TwitchPubSub pubsub;

        public PubSubService(FileManager fm, CommandIdentifier ci)
        {
            this.fm = fm;
            this.ci = ci;
            pubsub = new TwitchPubSub(); //init PubSub

            //Connect PubSub events with custom Methods
            pubsub.OnPubSubServiceConnected += Pubsub_OnPubSubServiceConnected;
            pubsub.OnListenResponse += Pubsub_OnListenResponse;
            pubsub.OnRewardRedeemed += Pubsub_OnRewardRedeemed;
            //pubsub.OnBitsReceived += Pubsub_OnBitsReceived; //Unused
            //pubsub.OnLog += PubSub_OnLog; //Pure Logging Function of PubSub
            //pubsub.OnWhisper += Pubsub_OnWhisper; //Testing
            pubsub.Connect(); //Connect
        }

        void PubSub_OnLog(object sender, OnLogArgs a)
        {
            Console.WriteLine(a.Data);
        }

        private void Pubsub_OnPubSubServiceConnected(object sender, System.EventArgs e)
        {
            pubsub.ListenToRewards(fm.GetChannelID()); //Set Channel ID
            pubsub.SendTopics(fm.GetOAuth()); //Give OAuth Key and connect
        }

        //Give feedback if connected sucessfully or failed
        private void Pubsub_OnListenResponse(object sender, OnListenResponseArgs e)
        {
            if (!e.Successful)
                throw new Exception($"Failed to listen! Response: {e.Topic}");
            else Console.WriteLine("Connected to PubSub");
        }

        //Unused
        //private void Pubsub_OnWhisper(object sender, OnWhisperArgs e)
        //{
        //    Console.WriteLine("Whisper recived");
        //}

        //Test Method to see bit redemption
        private void Pubsub_OnBitsReceived(object sender, OnBitsReceivedArgs e)
        {
            Console.WriteLine($"Just received {e.BitsUsed} bits from {e.Username}. That brings their total to {e.TotalBitsUsed} bits!");
        }

        
        private void Pubsub_OnRewardRedeemed(object sender, OnRewardRedeemedArgs e)
        {
            ci.CheckCommand(e);
            //RewardRedeemedDebug(e);
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
