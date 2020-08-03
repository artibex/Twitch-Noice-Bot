using System;
using System.Timers;

namespace Noice_Bot_Twitch
{

    class UserCooldown
    {
        DateTime EndTime;
        private System.Timers.Timer cooldown; //Timer
        public string username; //Username who played the sound
        public int soundfileID; //The sound who have been played
        public bool done = false; //Variable to check if it's done

        public UserCooldown(string username, double time, int soundfileID)
        {
            EndTime = DateTime.Now.AddSeconds(time);
            this.username = username;
            this.soundfileID = soundfileID;
            cooldown = new System.Timers.Timer(time * 1000);
            cooldown.Elapsed += MarkDone;
            cooldown.Enabled = true;
        }

        public double TimeLeft()
        {
            return Math.Round(EndTime.Subtract(DateTime.Now).TotalSeconds);
        }
        private void MarkDone(Object source, ElapsedEventArgs e)
        {
            done = true;
        }
    }
}
