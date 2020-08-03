using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Timers;

namespace Noice_Bot_Twitch
{
    class SoundboardManager
    {
        AudioDeviceManager adm;
        FileManager fm;
        IrcClient client;
        int userCooldown = 0;
        int globalCooldown = 0; //Global Cooldown, nobody is allowed to play another one untill it's over
        int responseCooldown = 0;
        int soundInterval = 0; //Should there be an intervall between any given soundfile? Like 1 Second etc.?
        bool globalCooldownActive = false; //Is the cooldown active?
        bool responseCooldownActive = false; //Prevent spamming in the chat the same thing over and over again
        bool useSoundCooldown = false; //Use the sound cooldown system
        bool soundCooldownActive = false; //The current sound is unavailable due to other user usage
        DateTime globalCooldownEndTimer; //The time when the bot will be back available

        System.Timers.Timer globalCooldownTimer;
        System.Timers.Timer responseCooldownTimer;

        List<String> soundFiles = new List<string>(); //The list of every collected sound
        List<String> subDirektories = new List<string>(); //List of all subdirektories containing sounds
        List<UserCooldown> cooldownList = new List<UserCooldown>(); //List of users in cooldown
        List<String> soundfileOffsetList = new List<string>();

        public SoundboardManager(AudioDeviceManager adm, FileManager fm, IrcClient client)
        {
            this.adm = adm;
            this.fm = fm;
            this.client = client;
            LoadSettings();
        }

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

        public void CheckCooldownList() //Checks if any timer is done and deletes this entry
        {
            List<int> rmList = new List<int>();
            //Checks if a usertimer is over and removes it from the list
            foreach(UserCooldown uc in cooldownList.ToList()) if(uc.done) cooldownList.Remove(uc);
        }

        //Public Method to trigger a soundeffect, check for global, user and interval cooldowns
        public void PlaySoundeffect(Comment c)
        {
            CheckCooldownList(); //Clear out the list of any left over user

            if (c.comment.Substring(1) == "play") //Info on how to use this command
            {
                string folders = "";
                foreach (string s in subDirektories) folders += " " + GetDirName(s);
                client.SendChatMessage("Use " + fm.GetCommandCharacter() + "play + a sound name, ID or foldername. Foldernames:" + folders);
                return;
            }
            foreach (UserCooldown uc in cooldownList) //If the user is on the cooldown list, return
            {
                if (uc.username == c.user)
                {
                    if(!responseCooldownActive)
                    {
                        client.SendChatMessage("@" + c.user + " not ready (" + uc.TimeLeft() + "sec)");
                        SetResponseCooldown();
                        return;
                    } else
                    {
                        return;
                    }
                }
            }
            //Play a random sound out of the library
            if(c.comment.Substring(1) == "play random")
            {
                string randomSoundPath;
                int id;

                GetRandomPath (out id, out randomSoundPath, soundFiles);
                PlaySound(randomSoundPath, c, 20);
            }
            else if(c.comment.Substring(1).Contains("play") && c.comment.Length > 6) //Search for a specific sound. ID or name
            {
                //Get the comment Substring
                string cSubstring = c.comment.Substring(c.comment.IndexOf(" ") + 1);
                int idSearch;

                //Check if it's a ID
                if(int.TryParse(cSubstring, out idSearch))
                {
                    if(GetPathByID(idSearch) != null)
                    {
                        string path = GetPathByID(idSearch);
                        PlaySound(path, c, 0);
                    }
                }
                //Check if it's a soundfile name
                foreach(string path in soundFiles)
                {
                    if (cSubstring.Equals(fm.GetSoundname(path)))
                    {
                        PlaySound(path, c, 0);
                    }
                }
                //Check if it's a folder name
                foreach(string s in subDirektories)
                {
                   //type friendly name
                   string dirName = s.Substring(s.LastIndexOf(@"\")+1);
                   //If the user typed in a directory name, play a random sound from that directory
                   if (cSubstring.Equals(dirName))
                   {
                        string randomSoundPath;
                        int id;
                        //Create a temp list with all the available soundfiles
                        List<String> dirSoundFiles = Directory.GetFiles(s).ToList();

                        GetRandomPath(out id, out randomSoundPath, dirSoundFiles);
                        PlaySound(randomSoundPath, c, 0);
                    }
                }
            }
        }

        //Play the actual sound
        void PlaySound(string path, Comment c, int extraTimeout)
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
                Speaker s = new Speaker(path, 0, 1f + GetVolumeOffset(path), false, true);
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
                            break;
                        case int n when (n >= 10 && n < 100): //Between 10 and 99
                            //Add 2 Zeros infront of the number
                            return "00" + count;
                            break;
                        case int n when (n >= 100 && n < 1000): //Between 100 and 999
                            //Add 1 Zero infront of the number
                            return "0" + count;
                            break;
                        case int n when (n >= 1000 && n < 10000): //Between 1000 and 9999
                            //Ad no Zero infront of the number
                            return count.ToString();
                            break;

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
            //If the user put in a to high number, return -1
            if (id >= soundFiles.Count) return null; //Out of Range
            else if (id <= -1) return null; //Out of Range
            else return soundFiles[id]; //Return the path
        }

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

        void CooldownUser(Comment c, double time, int soundfileID)
        {
            cooldownList.Add(new UserCooldown(c.user, time, soundfileID));
        }

        //Set the response cooldown to prevent chat spam
        void SetResponseCooldown()
        {
            responseCooldownTimer = new System.Timers.Timer(responseCooldown * 1000);
            responseCooldownTimer.Elapsed += DeactivateResponseCooldown;
            responseCooldownTimer.Enabled = true;
            responseCooldownActive = true;
        }

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
