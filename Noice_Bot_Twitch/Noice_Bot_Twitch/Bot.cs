using System;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace Noice_Bot_Twitch
{

    class Bot
    {
        FileManager fm;
        TwitchPubSub pubsub;

        public Bot(FileManager fm)
        {
            this.fm = fm;
            pubsub = new TwitchPubSub();
            pubsub.OnPubSubServiceConnected += Pubsub_OnPubSubServiceConnected;
            pubsub.OnListenResponse += Pubsub_OnListenResponse;
            //pubsub.OnBitsReceived += Pubsub_OnBitsReceived;
            pubsub.OnRewardRedeemed += Pubsub_OnRewardRedeemed;

            pubsub.OnWhisper += Pubsub_OnWhisper;

            pubsub.Connect();
        }

        private void Pubsub_OnPubSubServiceConnected(object sender, System.EventArgs e)
        {
            //pubsub.ListenToWhispers("xkbb3fwbhk5jsvtlieketeb4fd9ap4");
            //pubsub.ListenToBitsEvents("gp762nuuoqcoxypju8c569th9wz7q5");
            pubsub.ListenToRewards(fm.GetChannelID());
            //pubsub.SendTopics("18ge866fs4z4138wt8j3bkyvt9or7z");
            pubsub.SendTopics(fm.GetOAuth());

        }

        private void Pubsub_OnListenResponse(object sender, OnListenResponseArgs e)
        {
            if (!e.Successful)
                throw new Exception($"Failed to listen! Response: {e.Topic}");
            else Console.WriteLine("Connected to PubSub");
        }

        private void Pubsub_OnWhisper(object sender, OnWhisperArgs e)
        {
            Console.WriteLine("Whisper recived");
        }

        private void Pubsub_OnBitsReceived(object sender, OnBitsReceivedArgs e)
        {
            Console.WriteLine($"Just received {e.BitsUsed} bits from {e.Username}. That brings their total to {e.TotalBitsUsed} bits!");
        }

        private void Pubsub_OnRewardRedeemed(object sender, OnRewardRedeemedArgs e)
        {
            Console.WriteLine("Points redeemed");
            Console.WriteLine(e.ChannelId);
        }
    }
}
