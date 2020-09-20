using System;
using System.Linq;
using System.Text.RegularExpressions;

//Processing a given command like changing the username, checking black, whitelist and if it's a command
namespace Noice_Bot_Twitch
{
    public static class CommentProcessor
    {
        //CommandIdentifier ci; //Identify a command
        private static int _maxTextLength = 200; //Max amount of chars for TTS
        public static int maxTextLength
        {
            get { return _maxTextLength; }
            set { if(value > 0) _maxTextLength = value; }
        }
        private static int _spamThreshold = 30;
        public static int spamThreshold
        {
            get { return _spamThreshold; }
            set { if(value > 0) _spamThreshold = value; }
        }
        private static bool _removeEmojis;
        public static bool removeEmojis
        {
            get { return _removeEmojis; }
            set { _removeEmojis = value; }
        }
        private static string _badChars;
        public static string badChars
        {
            get { return _badChars; }
            set { badChars = value; }
        }


        //Init
        //public CommantProcessor(FileManager fm, CommandIdentifier ci)
        //{
        //    this.fm = fm;
        //    this.ci = ci;
        //    LoadSettings(); //Loading...
        //}

        //Load all the settings from the file manager
        public static void LoadSettings()
        {
            _maxTextLength = FileManager.GetMaxTextLength();
            _spamThreshold = FileManager.GetSpamThreshold();
            _removeEmojis = FileManager.GetRemoveEmojis();
            _badChars = FileManager.GetBadCharList();
        }

        //Process the given command and return it
        public static Comment Process(Comment c)
        {
            if (CheckBlacklist(c)) return new Comment("", ""); //Return empty comment and do nothing

            if (!CommandIdentifier.CheckCommand(c)) //before everything else is changed, check if it's a command
            {
                c = CheckAlias(c); //Replce username with given alias
                c.user = RemoveNumeric(c.user); //Remove numbers from name for faster reading? Hm... could be bad
                c = SpamProtection(c); //Check with the Spam protection before giving it back
                return c;
            } else //If it is a command, remove the comment to not read it out in the TTS
            {
                c.comment = "";
                return c;
            }
        }

        //Checks the Username and replaces it with an Alias of the list
        public static Comment CheckAlias(Comment c)
        {
            //For each string in the alias list, check if you can find the username
            foreach (string s in FileManager.GetAliasList())
            {
                string username = s.Substring(0, s.IndexOf(","));
                string alias = s.Substring(s.IndexOf(",")+1);
                if (c.user == username.ToLower())  c.user = alias.ToLower();
            }
            return c;
        }
        
        //Check if someone is spamming a lot of waird symboles and removes the command
        //Checks the length of a command and reduces it to a given length
        public static Comment SpamProtection(Comment c)
        {
            //Cut the comment down
            if(c.comment != null && c.comment.Length > _maxTextLength) c.comment = c.comment.Substring(0, _maxTextLength);

            //If true, remove ASCII emojis
            if (_removeEmojis && c.comment != null)
            {
                c.comment = Regex.Replace(c.comment, @"\p{Cs}", ""); //Remove all unicode emojis if true
                //If the comment where just unicode emojis say "unicode emoji"
                if (c.comment == "") c.comment = "unicode Emojis";
            }

            //Remove unwanted spamming of specific symboles
            int badCounter = 0; //If this counter is to high, remove the command
            if (c.comment != null)
            {
                foreach (char comChar in c.comment)
                {
                    //check with each char in the badStuff string the comment, if badCounter is too high, remove comment
                    foreach (char ch in _badChars) if (comChar == ch) badCounter++;
                    if (badCounter >= _spamThreshold) c.comment = ""; //Remove the comment completly
                }
            }
            return c;
        }

        //Checks if a user is on the Blacklist of TTS
        //Return true if user is on the list, otherwise false
        public static bool CheckBlacklist(Comment c)
        {
            foreach (string s in FileManager.GetBlackList())
            {
                string username = s.ToLower();
                if (c.user == username.ToLower()) return true;
            }
            return false;
        }

        //Checks if a user is on the Whitelist of controlling the Bot
        //Return true if he is on the list, otherwise false
        public static bool CheckWhiteList(Comment c)
        {
            foreach (string s in FileManager.GetWhiteList())
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
        public static String RemoveNumeric(string text)
        {
            string textwithoutnumeric = "";
            if (text != null) textwithoutnumeric = new String(text.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());
            return (textwithoutnumeric);
        }
    }
}
