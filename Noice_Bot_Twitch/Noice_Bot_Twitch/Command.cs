using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Noice_Bot_Twitch
{
    //Command datacontainer, used to store the command information and further processing
    class Command
    {
        public string user;
        public string comment;

        public Command(string user, string command)
        {
            this.user = user;
            this.comment = command;
        }
        public Command(string rawMsg)
        {
            //Pre edit
            rawMsg = rawMsg.ToLower();
            rawMsg = rawMsg.Trim();

            //Check if it's a user command
            if(rawMsg.Contains("privmsg") == true)
            {
                //Find the index of specific characters
                int usrIndex = rawMsg.IndexOf(":");
                int usrIndexEnd = rawMsg.IndexOf("!");
                int cmdIndex = rawMsg.LastIndexOf(":");

                //Get user and command and save it
                this.user = rawMsg.Substring(usrIndex+1, usrIndexEnd-1);
                this.comment = rawMsg.Substring(cmdIndex+1);
            }
        }
    }
}
