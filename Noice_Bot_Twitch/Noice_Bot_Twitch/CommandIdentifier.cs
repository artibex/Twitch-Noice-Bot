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
                if (whitelistOnly) //Check if only whitelisted people are allowed (Channel Point's work allways)
                {
                    foreach (string s in fm.GetWhiteList()) if (c.user.ToLower() == s.ToLower()) sm.PlaySoundeffect(c);
                } else sm.PlaySoundeffect(c);
            }
            return false;
        }
        public bool CheckCommand(TwitchLib.PubSub.Events.OnRewardRedeemedArgs e)
        {
            //Play Random
            if (e.RewardTitle.ToLower() == fm.GetCPPlayRandom().ToLower()) sm.PlayRandom();
            //Play Name or Random
            else if (e.RewardTitle.ToLower() == fm.GetCPPlayName().ToLower()) sm.PlayName(e);
            //Play ID or Random
            else if (e.RewardTitle.ToLower() == fm.GetCPPlayID().ToLower()) sm.PlayID(e);
            //Play Folder or Random
            else if (e.RewardTitle.ToLower() == fm.GetCPPlayFolder().ToLower()) sm.PlayFolder(e);

            return false;
        }
    }
}
