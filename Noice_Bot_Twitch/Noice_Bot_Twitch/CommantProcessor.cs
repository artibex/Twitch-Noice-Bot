using System;
using System.Linq;
using System.Text.RegularExpressions;

//Processing a given command like changing the username, checking black, whitelist and if it's a command
namespace Noice_Bot_Twitch
{
    class CommantProcessor
    {
        FileManager fm; //Manages all files and data
        CommandIdentifier ci; //Identify a command
        private int _maxTextLength = 200; //Max amount of chars for TTS
        public int MaxTextLength //This is how I should write and plan it, but i don't...
        {
            get { return _maxTextLength; }
            set { _maxTextLength = value; }
        }
        private int _spamThreshold = 30;
        public int BadThreshold
        {
            get { return _spamThreshold; }
            set { _spamThreshold = value; }
        }

        //Init
        public CommantProcessor(FileManager fm, CommandIdentifier ci)
        {
            this.fm = fm;
            this.ci = ci;
            LoadSettings(); //Loading...
        }

        //Load all the settings from the file manager
        public void LoadSettings()
        {
            _maxTextLength = fm.GetMaxTextLength();
            _spamThreshold = fm.GetSpamThreshold();
        }

        //Process the given command and return it
        public Comment Process(Comment c)
        {
            ci.CheckCommand(c); //before everything else is changed, check if it's a command
            c = CheckAlias(c); //Replce username with given alias
            c.user = RemoveNumeric(c.user); //Remove numbers from name for faster reading? Hm... could be bad
            c = SpamProtection(c); //Check with the Spam protection before giving it back
            return c;
        }

        //Checks the Username and replaces it with an Alias of the list
        public Comment CheckAlias(Comment c)
        {
            //For each string in the alias list, check if you can find the username
            foreach (string s in fm.GetAliasList())
            {
                string username = s.Substring(0, s.IndexOf(","));
                string alias = s.Substring(s.IndexOf(",")+1);
                if (c.user == username.ToLower())  c.user = alias.ToLower();
            }
            return c;
        }
        
        //Check if someone is spamming a lot of waird symboles and removes the command
        //Checks the length of a command and reduces it to a given length
        public Comment SpamProtection(Comment c)
        {
            //Cut the comment down
            if(c.comment != null && c.comment.Length > _maxTextLength) c.comment = c.comment.Substring(0, _maxTextLength);

            //If true, remove ASCII emojis
            if (fm.GetRemoveEmojis() && c.comment != null)
            {
                c.comment = Regex.Replace(c.comment, @"\p{Cs}", ""); //Remove all unicode emojis if true
                //If the comment where just unicode emojis say "unicode emoji"
                if (c.comment == "") c.comment = "unicode Emojis";
            }

            //Remove unwanted spamming of specific symboles
            string badStuff = @fm.GetBadCharList();
            int badCounter = 0; //If this counter is to high, remove the command
            if (c.comment != null)
            {
                foreach (char comChar in c.comment)
                {
                    //check with each char in the badStuff string the comment, if badCounter is too high, remove comment
                    foreach (char badChar in badStuff) if (comChar == badChar) badCounter++;
                    if (badCounter >= _spamThreshold) c.comment = ""; //Remove the comment completly
                }
            }
            return c;
        }

        //Checks if a user is on the Blacklist of TTS
        //Return true if user is on the list, otherwise false
        public bool CheckBlacklist(Comment c)
        {
            foreach (string s in fm.GetBlackList())
            {
                string username = s.ToLower();
                if (c.user == username.ToLower()) return true;
            }
            return false;
        }

        //Checks if a user is on the Whitelist of controlling the Bot
        //Return true if he is on the list, otherwise false
        public bool CheckWhiteList(Comment c)
        {
            foreach (string s in fm.GetWhiteList())
            {
                string username = s.ToLower();

                if (c.user == username.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        //Remove numbers in usernames for faster/proper reading
        public String RemoveNumeric(string text)
        {
            string textwithoutnumeric = "";
            if (text != null) textwithoutnumeric = new String(text.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());
            return (textwithoutnumeric);
        }
    }
}
