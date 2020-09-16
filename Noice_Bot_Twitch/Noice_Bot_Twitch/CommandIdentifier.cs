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

        //Return false if it's not a command
        public bool CheckCommand(Comment c)
        {
            if(c.comment != null && c.comment.StartsWith(commandCharacter))
            {
                if (whitelistOnly) //Check if only whitelisted people are allowed (Channel Point redemption work allways)
                {
                    foreach (string s in fm.GetWhiteList()) if (c.user.ToLower() == s.ToLower()) sm.PlaySoundeffect(c);
                } else sm.PlaySoundeffect(c);
                return true;
            }
            return false;
        }
        //Trigger with Channel Points
        public bool CheckCommand(TwitchLib.PubSub.Events.OnRewardRedeemedArgs e)
        {
            //Play Random
            if (e.RewardTitle.ToLower() == fm.GetCPPlayRandom().ToLower())
            {
                sm.PlayRandom();
                return true;
            }
            //Play Name or Random
            else if (e.RewardTitle.ToLower() == fm.GetCPPlayName().ToLower())
            {
                sm.PlayName(e);
                return true;
            }
            //Play ID or Random
            else if (e.RewardTitle.ToLower() == fm.GetCPPlayID().ToLower())
            {
                sm.PlayID(e);
                return true;
            }
            //Play Folder or Random
            else if (e.RewardTitle.ToLower() == fm.GetCPPlayFolder().ToLower())
            {
                sm.PlayFolder(e);
                return true;
            }
            return false;
        }
    }
}
