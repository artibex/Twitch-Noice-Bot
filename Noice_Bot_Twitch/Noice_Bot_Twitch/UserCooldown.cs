using System;
using System.Timers;

namespace Noice_Bot_Twitch
{
    //Hold information about what user played what soundfile and for how long the usercooldown/soundfile cooldown is
    class UserCooldown
    {
        DateTime EndTime; //When the timer have to end, needed for returning in seconds how long cooldown still is
        private System.Timers.Timer cooldown; //Timer
        public string username; //Username who played the sound
        public int soundfileID; //The sound who have been played as id
        public bool done = false; //Variable to check if it's done

        public UserCooldown(string username, double time, int soundfileID)
        {
            EndTime = DateTime.Now.AddSeconds(time); //Calculate end timer
            this.username = username; //user
            this.soundfileID = soundfileID; //soundfile id
            cooldown = new System.Timers.Timer(time * 1000); //Convert seconds into milliseconds and set timer
            cooldown.Elapsed += MarkDone; //Add to Elapsed Event the MarkDone method
            cooldown.Enabled = true; //Start the timer
        }

        //Returns the time left in seconds
        public double TimeLeft()
        {
            return Math.Round(EndTime.Subtract(DateTime.Now).TotalSeconds);
        }
        //Mark the timer done to remove it from the list
        private void MarkDone(Object source, ElapsedEventArgs e)
        {
            done = true;
        }
    }
}
