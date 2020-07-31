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
        }

        public bool CheckCommand(Comment c)
        {
            if(c.comment.StartsWith(commandCharacter))
            {
                Console.WriteLine("Found Character!");
                sm.PlaySoundeffect(c);
            }

            return false;
        }
    }
}
