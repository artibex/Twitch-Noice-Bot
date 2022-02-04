using System;

namespace Noice_Bot_Twitch
{
    //Command datacontainer, used to store the command information and further processing
    public class Comment
    {
        public string user;
        public string comment;

        public Comment(string user, string command)
        {
            this.user = user.ToLower();
            this.comment = command.ToLower();
        }
        public Comment(string rawMsg)
        {
            if(rawMsg != null)
            {
                //Pre edit
                rawMsg = rawMsg.ToLower();
                rawMsg = rawMsg.Trim();

                //Check if it's a user command made in the chat
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
            else
            {
                Console.WriteLine("Can't read incoming chat messeges correctly");
                Console.WriteLine(rawMsg);
            }
        }
    }
}
