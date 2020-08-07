using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;

namespace Noice_Bot_Twitch
{
    //This class Manages all Files the bot need to operate
    class FileManager
    {
        //Path and Filenames of every used file
        string path;
        string settingsFile = @"Settings.txt";
        string aliasFile = @"Aliaslist.txt";
        string blacklistFile = @"Blacklist.txt";
        string bridgelistFile = @"BridgeWordList.txt";
        string whitelistFile = @"Whitelist.txt";
        string soundOffsetFile = @"SoundfileOffset.txt";

        //Folder structure
        string soundEffectsFolder = @"Soundeffects";
        string notificationSoundsFolder = @"Notifications";
        string soundBoardFolder = @"Soundboard";

        //Alias List, Blacklist, Whitelist, Settings
        List<String> aliasList;
        List<String> blackList;
        List<String> bridgeWordList;
        List<String> whiteList;
        List<String> settingsList;
        List<String> notificationList; //Paths to the notifications sounds
        List<String> soundFiles; //All the Soundfiles as Path
        List<String> soundboardSubdirektories;  //Subdirektories of the soundboard folder
        List<String> soundfileOffsetList;

        public FileManager()
        {
            path = Directory.GetCurrentDirectory(); //Get the current execution Directory
            LoadFiles(); //Load the files into a string list object
            CheckFileExistence(); //Check file existance of all needed files, create missing ones (with examples)
        }

        public void LoadFiles() //Load all the Files
        {
            try //If an error occures while trying to load a file check the existence
            {
                notificationList = Directory.GetFiles(path + @"\" + soundEffectsFolder + @"\" + notificationSoundsFolder).ToList();

                aliasList = File.ReadAllLines(path + @"\" + aliasFile).ToList();
                blackList = File.ReadAllLines(path + @"\" + blacklistFile).ToList();
                bridgeWordList = File.ReadAllLines(path + @"\" + bridgelistFile).ToList();
                whiteList = File.ReadAllLines(path + @"\" + whitelistFile).ToList();
                settingsList = File.ReadAllLines(path + @"\" + settingsFile).ToList();

                LoadSoundfiles();
                UpdateSoundOffsetFile(); //Before loading the file, update it
                soundfileOffsetList = File.ReadAllLines(path + @"\" + soundOffsetFile).ToList();

            } catch
            {
                CheckFileExistence(); //Check existance
                LoadFiles(); //Load files again
            }
        }

        void LoadSoundfiles()
        {
            soundFiles = Directory.GetFiles(GetSoundboardPath()).ToList(); //Load spare soundfiles
            soundboardSubdirektories = Directory.GetDirectories(GetSoundboardPath()).ToList(); //Load subdirectories
            foreach (string dir in soundboardSubdirektories) //Get each soundfile from each subdirektorie
            {
                soundFiles.AddRange(Directory.GetFiles(dir).ToList());
            }
            Console.WriteLine("Loaded " + soundFiles.Count + " Soundfiles");
        }

        void CheckFileExistence() //Does all the wanted files exist? No? Then Create them and put examples in it
        {
            if(!File.Exists(path + @"\" + aliasFile)) //Alislist.txt
            {
                File.WriteAllText(path + @"\" + aliasFile, GenAliasFile());
                Console.WriteLine("File: " + aliasFile + " was missing");
            }
            if (!File.Exists(path + @"\" + blacklistFile)) //Blacklist.txt
            {
                File.WriteAllText(path + @"\" + blacklistFile, GenBlacklisteFile());
                Console.WriteLine("File: " + blacklistFile + " was missing");
            }
            if (!File.Exists(path + @"\" + bridgelistFile)) //Bridgelist.txt
            {
                File.WriteAllText(path + @"\" + bridgelistFile, GenBridgeListFile());
                Console.WriteLine("File: " + bridgelistFile + " was missing");
            }
            if (!File.Exists(path + @"\" + whitelistFile)) //Whitelist.txt
            {
                File.WriteAllText(path + @"\" + whitelistFile, GenWhitelistFile());
                Console.WriteLine("File: " + whitelistFile + " was missing");
            }
            if (!File.Exists(path + @"\" + settingsFile)) //Settings.txt
            {
                //Basic Conf, the user has to put in his own oauth key and channelname
                File.WriteAllText(path + @"\" + settingsFile, GenSettingsFile());
                Console.WriteLine("File: " + settingsFile + " was missing");
            }
            if (!File.Exists(path + @"\" + soundOffsetFile)) //Soundoffsetfile.txt
            {
                //Sound effects offset file to adjust every sound if needed
                File.WriteAllText(path + @"\" + soundOffsetFile, GenSoundOffsetFile());
                Console.WriteLine("File: " + soundOffsetFile + " was missing");
            }

            //Create Folder Structure for notifications and the soundboard
            if (!Directory.Exists(path + @"\" + soundEffectsFolder))
            {
                Directory.CreateDirectory(path + @"\" + soundEffectsFolder);
            }
            if (!Directory.Exists(path + @"\" + soundEffectsFolder + @"\" + notificationSoundsFolder))
            {
                Directory.CreateDirectory(path + @"\" + soundEffectsFolder + @"\" + notificationSoundsFolder);
            }
            if (!Directory.Exists(path + @"\" + soundEffectsFolder + @"\" + soundBoardFolder))
            {
                Directory.CreateDirectory(path + @"\" + soundEffectsFolder + @"\" + soundBoardFolder);
            }
        
        }

        //File Generation
        String GenAliasFile()
        {
            string str = @"Korpsian,the-developer
User1,CoolKid
USER2,NiceDude
UsEr3,TrollyDude
";
            return str;
        }
        String GenBlacklisteFile()
        {
            string str = @"YourBot
TrollKid69420
HahaFunnyGuy1
User1
user2
USER3
";
            return str;
        }
        String GenBridgeListFile()
        {
            string str = @"says
say
speaks
writes
";
            return str;
        }
        String GenWhitelistFile()
        {
            string str = @"User1
USER2
YourMods
Yourself
";
            return str;
        }
        String GenSettingsFile()
        {
            string str = @"#See the GitHub Page for how to use this file correctly
--IRC Settings--
ircclient=irc.twitch.tv
port=6667
botname=noisebot
channelname=
oauth=oauth:

--TTS Settings--
ttsbasespeed=3
ttsmaxspeed=7
#n = notification sound, u = username, b = bridgeword, c = comment
notificationExecutionOrder=ubc

--Anti Spam Settings--
maxtextlength=100
spamthreshold=8
removeemojis=true
badcharlist=!§$%&/()=?`^\{[]}#

--Audio Device Settings--
ttsoutputdevice=
soundboardoutputdevice=
notificationoutputdevice=
notificationvolume=0.5
ttsvolume=0.5
soundboardvolume=0.5


--Command Identifier Settings--
commandcharacter=!
whitelistonly=false

--Soundboard Settings--
customsoundboardfolder=
usercooldown=90
globalcooldown=0
responsecooldown=30
soundinterval=0
usesoundcooldown=true
";
            return str;
        }
        
        String GenSoundOffsetFile()
        {
            string str = "---Filename, Volume offset, Timeout offset--- \n";
            //soundfile name, volume offset (+ or - e.g.: -0.2, +0.3), timeout offset (e.g.:-10,+30)
            foreach(string path in soundFiles)
            {
                str += GetSoundname(path) + ",+0,+0\n";
            }
            return str;
        }
        
        //Update the Soundeffects offset file with new found soundeffects
        void UpdateSoundOffsetFile()
        {
            List<String> data = new List<string>();
            List<String> newData = new List<string>();

            if (File.Exists(path + @"\" + soundOffsetFile)) //Soundoffsetfile.txt
            {
                data = File.ReadAllLines(path + @"\" + soundOffsetFile).ToList();

                foreach(string soundfilePath in soundFiles)
                {
                    bool found = false;

                    //Get the soundfile name
                    string soundFileName = GetSoundname(soundfilePath);

                    //Foreach line in the file
                    foreach(string rawData in data)
                    {
                        //Split the data
                        string[] rawSplit = rawData.Split(',');

                        if(soundFileName == rawSplit[0])
                        {
                            //Found it in the list, break
                            found = true;
                            break;
                        }
                    }
                    if(!found)
                    {
                        newData.Add(soundFileName + ",+0,+0");
                    }
                }

                if(newData.Count > 0)
                {
                    foreach(string s in newData)
                    {
                        Console.WriteLine(s);
                    }

                    data.AddRange(newData);
                    string writingData = "";
                    foreach(string s in data)
                    {
                        writingData += s + "\n";
                    }
                    File.WriteAllText(path + @"\" + soundOffsetFile, writingData);
                    //Console.WriteLine("Updated the sound offset file"); //Debugging
                }
            }
            GenSoundOffsetFile(); //If the file does not exist, create it
        }

        //Getter methos for general path, alias, blacklist, whitelist and soundfile
        public string GetPath()
        {
            return path;
        }
        public string GetSoundboardPath()
        {
            //See if a custom path was given
            foreach (string s in settingsList)
            {
                if (s.Contains("customsoundboardfolder=") && s.Length > 23)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            //If not, return the default path
            return path + @"\" + soundEffectsFolder + @"\" + soundBoardFolder;
        }

        public string GetAliasFile()
        {
            return aliasFile;
        }
        public string GetBlacklistFile()
        {
            return blacklistFile;
        }
        public string GetWhitelistFile()
        {
            return whitelistFile;
        }

        //Get Specific Settings out of the Settings.txt
        public string GetIrcClient()
        {
            foreach(string s in settingsList)
            {
                if(s.Contains("ircclient=") && s.Length > 10)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT IRC CLIENT DETECTED");
            return null;
        } //IRC Settings
        public int GetPort()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("port=") && s.Length > 5)
                {
                    int i = -1;
                    if(int.TryParse(s.Substring(s.IndexOf("=")+1), out i))
                    {
                        return i;
                    }
                }
            }
            Console.WriteLine("INCORRECT PORT DETECTED");
            return 0;
        }
        public string GetBotName()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("botname=") && s.Length > 8)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT BOT NAME DETECTED");
            return null;
        }
        public string GetChannelName()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("channelname=") && s.Length > 12)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT CHANNELNAME DETECTED");
            return null;
        }
        public string GetOAuth()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("oauth=") && s.Length > 12)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT OAUTH DETECTED");
            return null;
        } //IRC Settings
        public int GetTTSBaseSpeed()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("ttsbasespeed=") && s.Length > 13)
                {
                    int i = -2;
                    if (int.TryParse(s.Substring(s.IndexOf("=") + 1), out i))
                    {
                        return i;
                    }
                }
            }
            return -11;
        }//TTS Settings
        public int GetTTSMaxSpeed()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("ttsmaxspeed=") && s.Length > 12)
                {
                    int i = -2;
                    if (int.TryParse(s.Substring(s.IndexOf("=") + 1), out i))
                    {
                        return i;
                    }
                }
            }
            return -11;
        } //TTS Settings
        public int GetMaxTextLength() //Anti Spam
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("maxtextlength=") && s.Length > 14)
                {
                    int i = -2;
                    if (int.TryParse(s.Substring(s.IndexOf("=") + 1), out i))
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
        public int GetSpamThreshold()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("spamthreshold=") && s.Length > 14)
                {
                    int i = -2;
                    if (int.TryParse(s.Substring(s.IndexOf("=") + 1), out i))
                    {
                        return i;
                    }
                }
            }
            return 0;
        } //Anti Spam
        public bool GetRemoveEmojis()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("removeemojis=") && s.Length > 13)
                {
                    string str = s.Substring(s.IndexOf("=") + 1);
                    if (str.Contains("true")) return true;
                    if (str.Contains("false")) return false;
                }
            }
            return false;
        }
        public bool GetUseSoundcooldown()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("usesoundcooldown=") && s.Length > 14)
                {
                    string str = s.Substring(s.IndexOf("=") + 1);
                    if (str.Contains("true")) return true;
                    if (str.Contains("false")) return false;
                }
            }
            return false;
        }

        public string GetBadCharList()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("badcharlist=") && s.Length > 12)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            return "";
        } //IRC Settings
        public int GetTTSOutputDeviceID()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("ttsoutputdevice=") && s.Length > 16)
                {
                    int i = -2;
                    if (int.TryParse(s.Substring(s.IndexOf("=") + 1), out i))
                    {
                        return i;
                    }
                }
            }
            return -2;
        } //Audio Device Settings
        public int GetSoundboardOutputDeviceID()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("soundboardoutputdevice=") && s.Length > 23)
                {
                    int i = -2;
                    if (int.TryParse(s.Substring(s.IndexOf("=") + 1), out i))
                    {
                        return i;
                    }
                }
            }
            return -2;
        }
        public int GetNotificationOutputDeviceID()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("notificationoutputdevice=") && s.Length > 25)
                {
                    int i = -2;
                    if (int.TryParse(s.Substring(s.IndexOf("=") + 1), out i))
                    {
                        return i;
                    }
                }
            }
            return -2;
        } //Audio Device Settings
        public string GetNotificationExecutionOrder()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("notificationExecutionOrder=") && s.Length > 27)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            return "";
        } //IRC Settings
        public float GetNotificationVolume()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("notificationvolume=") && s.Length > 19)
                {
                    try
                    {
                        return float.Parse(s.Substring(s.IndexOf("=") + 1), CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        Console.WriteLine("Could not read proper Notification Volume");
                        return 0.5f;
                    }
                }
            }
            return 0.5f;
        }
        public float GetTTSVolume()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("ttsvolume=") && s.Length > 10)
                {
                    try
                    {
                        return float.Parse(s.Substring(s.IndexOf("=")+1), CultureInfo.InvariantCulture);
                    } catch
                    {
                        Console.WriteLine("Could not read proper TTS Volume");
                        return 0.5f;
                    }
                }
            }
            return 0.5f;
        }
        public float GetSoundboardVolume()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("soundboardvolume=") && s.Length > 17)
                {
                    try
                    {
                        return float.Parse(s.Substring(s.IndexOf("=") + 1), CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        Console.WriteLine("Could not read proper Soundboard Volume");
                        return 0.5f;
                    }
                }
            }
            return 0.5f;

        }

        public String GetCommandCharacter()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("commandcharacter=") && s.Length > 17)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT SPECIAL CHARACTER DETECTED");
            return null;
        }
        public bool GetWhitelistOnly()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("whitelistonly=") && s.Length > 14)
                {
                    string b = s.Substring(s.IndexOf("=") + 1);
                    if (b == "true" || b == "t") return true;
                    else return false;
                }
            }
            return false;

        }
        public int GetUserCooldown()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("usercooldown=") && s.Length > 13)
                {
                    int i = -2;
                    if (int.TryParse(s.Substring(s.IndexOf("=") + 1), out i))
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
        public int GetGlobalCooldown()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("globalcooldown=") && s.Length > 15)
                {
                    int i = -2;
                    if (int.TryParse(s.Substring(s.IndexOf("=") + 1), out i))
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
        public int GetResponseCooldown() //Bot Response Cooldown
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("responsecooldown=") && s.Length > 16)
                {
                    int i = -2;
                    if (int.TryParse(s.Substring(s.IndexOf("=") + 1), out i))
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
        public int GetSoundInterval()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("soundinterval=") && s.Length > 14)
                {
                    int i = -2;
                    if (int.TryParse(s.Substring(s.IndexOf("=") + 1), out i))
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
        public List<String> GetSoundfiles()
        {
            return soundFiles;
        }
        public List<String> GetSoundboardSubdirektories()
        {
            return soundboardSubdirektories;
        }
        public List<String> GetSoundfileOffsetList()
        {
            return soundfileOffsetList;
        }
        
        //Return the created String lists
        public List<String> GetBlackList()
        {
            return blackList;
        }
        public List<String> GetBridgeWordList()
        {
            return bridgeWordList;        }
        public List<String> GetAliasList()
        {
            return aliasList;
        }
        public List<String> GetWhiteList()
        {
            return whiteList;
        }


        //Returns a easy to write soundfile name
        public string GetSoundname(string path)
        {
            string str = path.Substring(path.LastIndexOf(@"\") + 1);
            str = Regex.Replace(str, @"\s+", "");
            str = str.ToLower();
            str = str.Trim();
            str = str.Remove(str.Length - 4);
            return str;
        }


        public String GetRandomNotificationSound()
        {
            Random rand = new Random();
            string str = notificationList[rand.Next(notificationList.Count)];
            return str;
        }

        //Take a random bridge word out of Brideword.txt and return it
        public String GetRandomBridgeWord() 
        {
            Random rand = new Random();
            string str = bridgeWordList[rand.Next(bridgeWordList.Count)];
            return str;
        }
    }
}
