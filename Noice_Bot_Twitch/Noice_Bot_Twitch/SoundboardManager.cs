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
        List<String> subDirektories = new List<string>();
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
            soundFiles = Directory.GetFiles(fm.GetSoundboardPath()).ToList();
            subDirektories = Directory.GetDirectories(fm.GetSoundboardPath()).ToList();
            foreach(string dir in subDirektories)
            {
                soundFiles.AddRange(Directory.GetFiles(dir).ToList());
            }
            foreach(string s in soundFiles)
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
                client.SendChatMessage("Playing: " + GetSoundname(randomSoundPath) + " (ID:00" + id + ")");
                Speaker s = new Speaker(randomSoundPath, 0, 1f, false);
            } else if(c.comment.Substring(1).Contains("play")) //Search for a specific sound. ID or name
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
                        client.SendChatMessage("Playing: " + GetSoundname(randomSoundPath) + " (ID:00" + id + ")");
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

        //Takes a list and returns a random path from it
        void GetRandomPath(out int id, out string path, List<String> strList)
        {
            Random rand = new Random();
            id = rand.Next(strList.Count);
            path = strList[id];
        }
    }
}
