using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Timers;
using TwitchLib.PubSub.Events;

namespace Noice_Bot_Twitch
{
    //Manages the playing of soundeffects and cooldowns
    class SoundboardManager
    {
        AudioDeviceManager adm; //Load output device ID here
        FileManager fm; //Get Paths to soundfiles here
        IrcClient client; //Write Chat masseges with that
        int userCooldown = 0; // Basic user cooldown, get's loaded by Settings.txt
        int globalCooldown = 0; //Global Cooldown, nobody is allowed to play another one untill it's over
        int responseCooldown = 0; //If set, the bot will not answer to failed tasks
        int soundInterval = 0; //Should there be an intervall between any given soundfile? Like 1 Second etc.?
        bool globalCooldownActive = false; //Is a global cooldown currenlty active?
        bool responseCooldownActive = false; //Is response cooldown active?
        bool useSoundCooldown = false; //Use the soundfile cooldown system
        DateTime globalCooldownEndTimer; //The time when the bot will be back available
        System.Timers.Timer globalCooldownTimer; //Global timer to set
        System.Timers.Timer responseCooldownTimer; //Response timer to set

        List<String> soundFiles = new List<string>(); //The list of every collected sound
        List<String> subDirektories = new List<string>(); //List of all subdirektories containing sounds
        List<UserCooldown> cooldownList = new List<UserCooldown>(); //List of users in cooldown
        List<String> soundfileOffsetList = new List<string>(); //List of all offsets

        public SoundboardManager(AudioDeviceManager adm, FileManager fm, IrcClient client)
        {
            this.adm = adm;
            this.fm = fm;
            this.client = client;
            LoadSettings();
        }

        //Use this to reload stuff
        public void LoadSettings()
        {
            userCooldown = fm.GetUserCooldown();
            globalCooldown = fm.GetGlobalCooldown();
            responseCooldown = fm.GetResponseCooldown();
            soundInterval = fm.GetSoundInterval();
            soundFiles = fm.GetSoundfiles();
            subDirektories = fm.GetSoundboardSubdirektories();
            soundfileOffsetList = fm.GetSoundfileOffsetList();
            useSoundCooldown = fm.GetUseSoundcooldown();
        }

        //Checks if any timer is done and deletes this entry
        public void CheckCooldownList()
        {
            List<int> rmList = new List<int>();
            //Checks if a usertimer is over and removes it from the list
            foreach(UserCooldown uc in cooldownList.ToList()) if(uc.done) cooldownList.Remove(uc);
        }

        //Trigger sound by chat
        public void PlaySoundeffect(Comment c)
        {
            CheckCooldownList(); //Clear out the list of any left over user

            //Info on how to use this command
            if (c.comment.Substring(1) == "play")
            {
                string folders = " ";
                foreach (string s in subDirektories) folders += GetDirName(s) + ", "; //Display all folders/Sublibraries to choose from
                client.SendChatMessage("Use " + fm.GetCommandCharacter() + "play + a sound name, ID or category OR random. Sublibraries:" + folders);
                return;
            }
            //If the user is on the cooldown list, return
            foreach (UserCooldown uc in cooldownList) 
            {
                if (uc.username == c.user)
                {
                    if(!responseCooldownActive)
                    {
                        client.SendChatMessage("@" + c.user + " not ready (" + uc.TimeLeft() + "sec)");
                        SetResponseCooldown();
                        return;
                    } else return;
                }
            }
            //Play a random sound out of the library
            if(c.comment.Substring(1) == "play random")
            {
                PlayRandom(c);
            }
            //Search for a specific sound. ID or name
            else if (c.comment.Substring(1).Contains("play") && c.comment.Length > 6) 
            {
                //Get the comment Substring without !play
                string cSubstring = c.comment.Substring(c.comment.IndexOf(" ") + 1);
                int idSearch; //ID to try to parse

                //Check if it's a ID
                if(int.TryParse(cSubstring, out idSearch)) if(GetPathByID(idSearch) != null) PlayID(c, idSearch);
                //Check if it's a Sublibrarie name
                foreach (string s in subDirektories) if (cSubstring.Equals(s.Substring(s.LastIndexOf(@"\") + 1))) PlayFolder(c, s);
                //Check if it's a soundfile name
                foreach(string path in soundFiles) if (cSubstring.Equals(fm.GetSoundname(path))) PlaySound(path, c, 0);
            }
        }

        //Triggerd by Command
        void PlayRandom(Comment c)
        {
            string randomSoundPath;
            int id;

            GetRandomPath(out id, out randomSoundPath, soundFiles);
            PlaySound(randomSoundPath, c, 20);
        }
        void PlayID(Comment c, int id)
        {
            string cSubstring = c.comment.Substring(c.comment.IndexOf(" ") + 1);
            string path = GetPathByID(id);
            PlaySound(path, c, 0);
        }
        void PlayName(Comment c, string name)
        {
            PlaySound(name, c, 0);

            //No need for this?
            //foreach (string path in soundFiles)
            //{
            //    if (name.Equals(fm.GetSoundname(path))) PlaySound(path, c, 0);
            //}
        }
        void PlayFolder(Comment c, string folderPath)
        {
            string randomSoundPath; //Random path that got pulled out of the Sublibrarie
            int id; //ID of this soundfile
            //Create a temp list with all the available soundfiles
            List<String> dirSoundFiles = Directory.GetFiles(folderPath).ToList(); //Get all soundfiles from this folder
            GetRandomPath(out id, out randomSoundPath, dirSoundFiles); //Get a random path by a given list
            PlaySound(randomSoundPath, c, 0); //Play it
        }

        //Triggerd by Channel Point Redemption
        //Random Sound
        public void PlayRandom() 
        {
            string randomSoundPath;
            int id; //ID
            GetRandomPath(out id, out randomSoundPath, soundFiles);
            PlaySound(randomSoundPath, "");
        }
        //Specific Name or Random if not found
        public void PlayName(OnRewardRedeemedArgs e)
        {
            foreach (string path in soundFiles)
            {
                if (e.Message.Equals(fm.GetSoundname(path)))
                {
                    PlaySound(path, "");
                    return;
                }
            }

            string randomSoundPath;
            int id;
            GetRandomPath(out id, out randomSoundPath, soundFiles);
            PlaySound(randomSoundPath, " Can't find name:" + e.Message + " play random instead");
        }
        //Play ID or Random if not found
        public void PlayID(OnRewardRedeemedArgs e)
        {
            int ID;
            if (int.TryParse(e.Message, out ID)) PlaySound(GetPathByID(ID), "");
            else
            {
                string randomSoundPath;
                int id;
                GetRandomPath(out id, out randomSoundPath, soundFiles);
                PlaySound(randomSoundPath, " Can't find ID:" + e.Message + " play random instead");
            }
        }
        public void PlayFolder(OnRewardRedeemedArgs e) //Currently unused
        {
            foreach (string s in subDirektories)
            {
                if (e.Message.Equals(s.Substring(s.LastIndexOf(@"\") + 1)))
                {
                    string randomSoundPath; //Random path that got pulled out of the Sublibrarie
                    int id; //ID of this soundfile
                    //Create a temp list with all the available soundfiles
                    List<String> dirSoundFiles = Directory.GetFiles(s).ToList(); //Get all soundfiles from this folder
                    GetRandomPath(out id, out randomSoundPath, dirSoundFiles); //Get a random path by a given list
                    PlaySound(randomSoundPath, ""); //Play it
                    return;
                }
            }
            PlayRandom();
        }

        //Play sound, or tell user he have to wait
        public void PlaySound(string path, Comment c, int extraTimeout)
        {
            if(!globalCooldownActive) //If the global cooldown is not active, play the sound
            {
                //Cooldown the sound if wanted to prevent spam of the same soundfile
                if(useSoundCooldown)
                {
                    foreach(UserCooldown uc in cooldownList)
                    {
                        if(uc.soundfileID == GetIDInt(path))
                        {
                            if(!responseCooldownActive)
                            {
                                client.SendChatMessage("Soundfile " + fm.GetSoundname(soundFiles[uc.soundfileID]) + " in cd (" + uc.TimeLeft() + "sec)");
                                SetResponseCooldown();
                            }
                            return;
                        }
                    }
                }

                float combinedCooldown = userCooldown + GetTimeoutOffset(path) + extraTimeout;
                float volume = fm.GetSoundboardVolume() + GetVolumeOffset(path);
                if (volume > 1f) volume = 1f; //Set Volume maximum and minimum
                if (volume < 0f) volume = 0.05f;

                Speaker s = new Speaker(path, 0, volume, false, true);
                CooldownUser(c, combinedCooldown, GetIDInt(path));
                client.SendChatMessage("Playing: " + fm.GetSoundname(path) + " ID:" + GetIDString(path) + " (cd: " + combinedCooldown + "sec)");
                SetGlobalCooldown();
            } else //If the glocal cooldown is active, return the time left
            {
                if(!responseCooldownActive)
                {
                    client.SendChatMessage("Global cd (" + GetGlobalCooldownLeft() + "sec)");
                    SetResponseCooldown();
                }
            }
        }

        //Ignore all cooldowns and play it
        void PlaySound(string path, string extraInfo) 
        {
            float volume = fm.GetSoundboardVolume() + GetVolumeOffset(path);
            if (volume > 1f) volume = 1f; //Set Volume maximum and minimum
            if (volume < 0f) volume = 0.05f;

            Speaker s = new Speaker(path, 0, volume, false, true);
            client.SendChatMessage("Playing: " + fm.GetSoundname(path) + " ID:" + GetIDString(path) + extraInfo);
        }

        //Returns the subdirektorie name of the given path
        string GetDirName(string path)
        {
            string str = path.Substring(path.LastIndexOf(@"\") + 1);
            str = str.ToLower();
            return str;
        }

        //Get the ID of the given songfile path
        string GetIDString(string path)
        {
            int count = 0;

            foreach(string s in soundFiles)
            {
                if(path == s)
                {
                    switch(count)
                    {
                        case int n when (n < 10): //Between 0 and 9
                            //Add 3 Zeros infront of the number
                            return "000" + count;
                        case int n when (n >= 10 && n < 100): //Between 10 and 99
                            //Add 2 Zeros infront of the number
                            return "00" + count;
                        case int n when (n >= 100 && n < 1000): //Between 100 and 999
                            //Add 1 Zero infront of the number
                            return "0" + count;
                        case int n when (n >= 1000 && n < 10000): //Between 1000 and 9999
                            //Ad no Zero infront of the number
                            return count.ToString();

                    }

                    return "00" + count.ToString();
                }
                count++;
            }

            return "";
        }
        int GetIDInt(string path)
        {
            int count = 0;
            string pName = fm.GetSoundname(path);

            foreach(string s in soundFiles)
            {
                string sName = fm.GetSoundname(s);
                if (pName.Equals(sName))
                {
                    return count;
                }
                count++;
            }
            //Should never get here
            return 0;
        }

        //Takes a list and returns a random string from it
        void GetRandomPath(out int id, out string path, List<String> strList)
        {
            Random rand = new Random();
            id = rand.Next(strList.Count);
            path = strList[id];
        }

        //Get the Path by a given ID
        string GetPathByID(int id)
        {
            //If the user put in a to high number, return las in list
            if (id >= soundFiles.Count) return soundFiles[soundFiles.Count-1]; //Out of Range
            else if (id <= -1) return soundFiles[soundFiles.Count - 1]; //Out of Range
            else return soundFiles[id]; //Return the path
        }
        //Get offset for the soundeffect and add or subtract it to the volume
        float GetVolumeOffset(string path)
        {
            string name = fm.GetSoundname(path);
            foreach(string raw in soundfileOffsetList)
            {
                //Split it in 3 parts
                string[] rawSplit = raw.Split(',');

                if (name == rawSplit[0])
                {
                    if (rawSplit[1][0] == ('+')) return float.Parse(rawSplit[1].Substring(1), CultureInfo.InvariantCulture);
                    if (rawSplit[1][0] == ('-')) return float.Parse(rawSplit[1].Substring(1), CultureInfo.InvariantCulture) * -1;
                }
            }
            return 0;
        }

        //Get the timeout offset of a soundfile to add or subtract it to the usercooldown
        float GetTimeoutOffset(string path)
        {
            string name = fm.GetSoundname(path);
            foreach (string raw in soundfileOffsetList)
            {
                //Split it in 3 parts
                string[] rawSplit = raw.Split(',');

                if (name == rawSplit[0])
                {
                    if (rawSplit[2][0] == ('+')) return float.Parse(rawSplit[2].Substring(1));
                    if (rawSplit[2][0] == ('-')) return float.Parse(rawSplit[2].Substring(1)) * -1;
                }
            }
            return 0;
        }

        //Add a new user cooldown
        void CooldownUser(Comment c, double time, int soundfileID)
        {
            cooldownList.Add(new UserCooldown(c.user, time, soundfileID));
        }

        //Response cooldown to prevent chat spamming
        void SetResponseCooldown()
        {
            responseCooldownTimer = new System.Timers.Timer(responseCooldown * 1000);
            responseCooldownTimer.Elapsed += DeactivateResponseCooldown;
            responseCooldownTimer.Enabled = true;
            responseCooldownActive = true;
        }

        //Get's triggerd when timer elapses
        void DeactivateResponseCooldown(Object source, ElapsedEventArgs e)
        {
            responseCooldownActive = false;
        }

        //Starts the global cooldown
        void SetGlobalCooldown()
        {
            globalCooldownEndTimer = DateTime.Now.AddSeconds(globalCooldown);
            globalCooldownTimer = new System.Timers.Timer(globalCooldown * 1000);
            globalCooldownTimer.Elapsed += DeactivateGlobelCooldown;
            globalCooldownTimer.Enabled = true;
            globalCooldownActive = true;
        }
        //How much time is left on the global cooldown?
        double GetGlobalCooldownLeft()
        {
            return Math.Round(globalCooldownEndTimer.Subtract(DateTime.Now).TotalSeconds);
        }
        //Deactivate the Global cooldown when the timer is down
        void DeactivateGlobelCooldown(Object source, ElapsedEventArgs e)
        {
            globalCooldownActive = false;
        }
    }
}
