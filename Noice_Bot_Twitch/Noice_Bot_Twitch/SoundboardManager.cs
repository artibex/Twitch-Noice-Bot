using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Noice_Bot_Twitch
{
    class SoundboardManager
    {
        AudioDeviceManager adm;
        FileManager fm;
        IrcClient client;
        int userCooldown = 0;
        int globalCooldown = 0;
        int soundInterval = 0;

        List<String> soundFiles = new List<string>(); //The list of every collected sound
        List<String> subDirektories = new List<string>(); //List of all subdirektories containing sounds
        List<UserCooldown> cooldownList = new List<UserCooldown>(); //List of users in cooldown

        public SoundboardManager(AudioDeviceManager adm, FileManager fm, IrcClient client)
        {
            this.adm = adm;
            this.fm = fm;
            this.client = client;
            LoadSoundfiles();
            LoadSettings();
        }

        public void LoadSettings()
        {
            userCooldown = fm.GetUserCooldown();
            globalCooldown = fm.GetGlobalCooldown();
            soundInterval = fm.GetSoundInterval();
        }

        void LoadSoundfiles()
        {
            soundFiles = Directory.GetFiles(fm.GetSoundboardPath()).ToList(); //Load spare soundfiles
            subDirektories = Directory.GetDirectories(fm.GetSoundboardPath()).ToList(); //Load subdirectories
            foreach(string dir in subDirektories) //Get each soundfile from each subdirektorie
            {
                soundFiles.AddRange(Directory.GetFiles(dir).ToList());
            }
            foreach (string s in soundFiles) //Write out the loaded files (debugging)
            {
                Console.WriteLine(s);
            }
        }

        public void CheckCooldownList() //Checks if any timer is done and deletes this entry
        {
            List<int> rmList = new List<int>();
            //Checks if a usertimer is over and removes it from the list
            foreach(UserCooldown uc in cooldownList) if(uc.done) rmList.Add(cooldownList.IndexOf(uc));
            foreach(int i in rmList) cooldownList.RemoveAt(i);
        }

        public void PlaySoundeffect(Comment c)
        {
            CheckCooldownList(); //Clear out the list of any left over user
            foreach(UserCooldown uc in cooldownList) //If the user is on the cooldown list, return
            {
                if (uc.username == c.user) return;
            }
            //Play a random sound out of the library
            if(c.comment.Substring(1) == "play random")
            {
                string randomSoundPath;
                int id;

                GetRandomPath (out id, out randomSoundPath, soundFiles);
                client.SendChatMessage("Playing: " + GetSoundname(randomSoundPath) + " ID:" + GetID(randomSoundPath));
                Speaker s = new Speaker(randomSoundPath, 0, 1f, false);
            } else if(c.comment.Substring(1) == "play")
            {
                string folders = "";
                foreach(string s in subDirektories) folders += " " + GetDirName(s);
                client.SendChatMessage("Use " + fm.GetCommandCharacter() + "play + a sound name, ID or foldername. Foldernames:" + folders);
            }
            
            else if(c.comment.Substring(1).Contains("play") && c.comment.Length > 6) //Search for a specific sound. ID or name
            {
                //Check if it's a folder name
                foreach(string s in subDirektories)
                {
                   //type friendly name
                   string dirName = s.Substring(s.LastIndexOf(@"\"));
                   //If the user typed in a directory name, play a random sound from that directory
                   if (c.comment.Substring(c.comment.IndexOf(" ")) == dirName);
                   {
                        string randomSoundPath;
                        int id;
                        //Create a temp list with all the available soundfiles
                        List<String> dirSoundFiles = Directory.GetFiles(s).ToList();

                        GetRandomPath(out id, out randomSoundPath, dirSoundFiles);
                        client.SendChatMessage("Playing: " + GetSoundname(randomSoundPath) + " ID:" + GetID(randomSoundPath));
                        Speaker sp = new Speaker(randomSoundPath, 0, 1f, false);
                    }
                }
            }
        }

        string GetSoundname(string path)
        {
            string str = path.Substring(path.LastIndexOf(@"\") + 1);
            str = Regex.Replace(str, @"\s+", "");
            str = str.ToLower();
            str = str.Trim();
            str = str.Remove(str.Length - 4);
            return str;
        }
        //Returns the subdirektorie name of the given path
        string GetDirName(string path)
        {
            string str = path.Substring(path.LastIndexOf(@"\") + 1);
            str = str.ToLower();
            return str;
        }

        //Get the ID of the given songfile path
        string GetID(string path)
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

        //Takes a list and returns a random string from it
        void GetRandomPath(out int id, out string path, List<String> strList)
        {
            Random rand = new Random();
            id = rand.Next(strList.Count);
            path = strList[id];
        }


    }
}
