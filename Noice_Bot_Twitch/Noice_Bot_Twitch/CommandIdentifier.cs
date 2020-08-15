using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noice_Bot_Twitch
{
    //Checks each commant and looks for a bot command (usually begins with "!") and executes further
    class CommandIdentifier
    {
        SoundboardManager sm;
        FileManager fm;
        string commandCharacter;
        bool whitelistOnly = false;

        //If the user put a "!" or similar infront, check list for commands
        public CommandIdentifier(SoundboardManager sm, FileManager fm)
        {
            this.sm = sm;
            this.fm = fm;
            LoadSettings();
        }

        public void LoadSettings()
        {
            commandCharacter = fm.GetCommandCharacter();
            whitelistOnly = fm.GetWhitelistOnly();
        }

        public bool CheckCommand(Comment c)
        {
            if(c.comment.StartsWith(commandCharacter))
            {
                if (whitelistOnly)
                {
                    foreach (string s in fm.GetWhiteList())
                    {
                        if (c.user.ToLower() == s.ToLower()) sm.PlaySoundeffect(c);
                    }
                } else sm.PlaySoundeffect(c);
            }

            return false;
        }
    }
}
