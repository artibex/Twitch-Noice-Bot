using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Noice_Bot_Twitch
{

    class UserCooldown
    {
        private System.Timers.Timer cooldown;
        public string username;
        public bool done = false;

        public UserCooldown(string username, float time)
        {
            this.username = username;
            cooldown = new System.Timers.Timer(time * 1000);
            cooldown.Elapsed += MarkDone;
            cooldown.Enabled = true;
        }

        private void MarkDone(Object source, ElapsedEventArgs e)
        {
            done = true;
        }
    }
}
