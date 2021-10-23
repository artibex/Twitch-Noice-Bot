using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;

namespace Noice_Bot_Twitch
{
    //This class Manages all Files the bot needs to operate
    public static class FileManager
    {
        //Path and Filenames of every used file
        private static string path;
        private static string settingsFile = @"Settings.txt";
        private static string aliasFile = @"Aliaslist.txt";
        private static string blacklistFile = @"Blacklist.txt";
        private static string ttsblacklistFile = @"TTSBlacklist.txt";
        private static string bridgelistFile = @"BridgeWordList.txt";
        private static string whitelistFile = @"Whitelist.txt";
        private static string soundOffsetFile = @"SoundfileOffset.txt";
        private static string soundsFile = @"Sounds.txt";
        private static string commandsFile = @"Commands.json";

        //Folder structure
        static string settingsFolder = @"Settings"; //Settings of the bot
        static string soundEffectsFolder = @"Soundeffects"; //Soundeffects folder usually contains Notification folder and Soundboard folder
        static string notificationSoundsFolder = @"Notifications"; //Notification sounds folder
        static string soundBoardFolder = @"Soundboard"; //Soundboard folder name, can be defined via Settings file
        //Alias List, Blacklist, Whitelist, Settings
        static List<String> aliasList; //Loaded Aliases
        static List<String> blackList; //Loaded Blacklist
        static List<String> ttsBlacklist;
        static List<String> bridgeWordList; //Loaded Bridgewords
        static List<String> whiteList; //Loaded Whitelist
        static List<String> settingsList; //Loaded Settings
        static List<String> notificationList; //Paths to the notifications sounds
        static List<String> soundfilePaths; //All the Soundfiles as Path
        static List<String> soundboardSubdirektories; //Subdirektories of the soundboard folder
        static List<String> soundfileOffsetList; //Loaded offsets of soundfiles
        static List<Command> commands;

        //Init Filemanager
        //public static FileManager()
        //{
        //    path = Directory.GetCurrentDirectory(); //Get the current execution Directory
        //    LoadFiles(); //Load the files into string lists
        //    CheckFileExistence(); //Check file existance of all needed files, create missing ones (with examples)
        //}

        public static void LoadSettings()
        {
            path = Directory.GetCurrentDirectory(); //Get the current execution Directory
            LoadFiles(); //Load the files into string lists
            CheckFileExistence(); //Check file existance of all needed files, create missing ones (with examples)
        }

        public static void LoadFiles() //Load all the Files
        {
            try //If an error occures while trying to load a file check the existence
            {
                //Load all notification sounds as paths
                notificationList = Directory.GetFiles(path + @"\" + soundEffectsFolder + @"\" + notificationSoundsFolder).ToList();
                
                settingsList = File.ReadAllLines(path + @"\" + settingsFolder + @"\" + settingsFile).ToList(); //Read "Settings.txt"
                aliasList = File.ReadAllLines(path + @"\" + settingsFolder + @"\" + aliasFile).ToList(); //Read "Aliaslist.txt"
                blackList = File.ReadAllLines(path + @"\" + settingsFolder + @"\" + blacklistFile).ToList(); //Read "Blacklist.txt"
                ttsBlacklist = File.ReadAllLines(path + @"\" + settingsFolder + @"\" + ttsblacklistFile).ToList(); //Read "TTSBlacklist.txt"
                bridgeWordList = File.ReadAllLines(path + @"\" + settingsFolder + @"\" + bridgelistFile).ToList(); //Read "BridgeWordList.txt"
                whiteList = File.ReadAllLines(path + @"\" + settingsFolder + @"\" + whitelistFile).ToList(); //Read "Whitelist.txt"
           
                commands = JsonConvert.DeserializeObject<List<Command>>(File.ReadAllText(path + @"\" + settingsFolder + @"\" + commandsFile));

                LoadSoundfiles(); //Check first for a sounds file and generate it if needed
                UpdateSoundOffsetFile(); //Before loading the file, update it to check if new sounds are added
                soundfileOffsetList = File.ReadAllLines(path + @"\" + settingsFolder + @"\" + soundOffsetFile).ToList(); //Load sound offset
            } catch
            {
                Console.WriteLine("Failed at loading files");
                CheckFileExistence(); //Check existance of every file
                LoadFiles(); //Load files again
            }
        }

        //Load all soundfiles in a given path
        public static void LoadSoundfiles()
        {
            soundfilePaths = new List<string>(); //Create new List

            if (File.Exists(path + @"\" + settingsFolder + @"\" + soundsFile)) //If Sounds.txt does not exist, create it
            {
                UpdateSoundsListFile();

                //read soundfiles with id and name in and extract the path of the file
                List<string> sounds = File.ReadAllLines(path + @"\" + settingsFolder + @"\" + soundsFile).ToList();
                foreach (string s in sounds)
                {
                    string[] split = s.Split(',');
                    soundfilePaths.Add(split[2]);
                }
            } else CheckFileExistence();

            if (Directory.Exists(GetSoundboardPath())) //get soundboardSubdirektories
            {
                soundboardSubdirektories = Directory.GetDirectories(GetSoundboardPath()).ToList(); //Load subdirectories
                Console.WriteLine("Loaded " + soundfilePaths.Count + " Soundfiles"); //Display the loaded amount in console to flex
            }
        }

        static void CheckFileExistence() //Does all the wanted files exist? No? Then Create them and put examples in it
        {
            //Create Folder Structure for notifications and the soundboard if not existing
            if (!Directory.Exists(path + @"\" + soundEffectsFolder)) Directory.CreateDirectory(path + @"\" + soundEffectsFolder);
            if (!Directory.Exists(path + @"\" + soundEffectsFolder + @"\" + notificationSoundsFolder)) Directory.CreateDirectory(path + @"\" + soundEffectsFolder + @"\" + notificationSoundsFolder);
            if (!Directory.Exists(path + @"\" + soundEffectsFolder + @"\" + soundBoardFolder)) Directory.CreateDirectory(path + @"\" + soundEffectsFolder + @"\" + soundBoardFolder);
            if (!Directory.Exists(path + @"\" + settingsFolder)) Directory.CreateDirectory(path + @"\" + settingsFolder);

            if (!File.Exists(path + @"\" + settingsFolder + @"\" + aliasFile)) //Alislist.txt
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Noice_Bot_Twitch.SettingsRef.aliaslistRef.txt";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    File.WriteAllText(path + @"\" + settingsFolder + @"\" + aliasFile, result);
                }
                Console.WriteLine("File: " + aliasFile + " was missing");
            }
            if (!File.Exists(path + @"\" + settingsFolder + @"\" + blacklistFile)) //Blacklist.txt
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Noice_Bot_Twitch.SettingsRef.blacklistRef.txt";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    File.WriteAllText(path + @"\" + settingsFolder + @"\" + blacklistFile, result);
                }
                Console.WriteLine("File: " + blacklistFile + " was missing");
            }
            if (!File.Exists(path + @"\" + settingsFolder + @"\" + ttsblacklistFile)) //Blacklist.txt
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Noice_Bot_Twitch.SettingsRef.ttsblacklistRef.txt";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    File.WriteAllText(path + @"\" + settingsFolder + @"\" + ttsblacklistFile, result);
                }
                Console.WriteLine("File: " + blacklistFile + " was missing");
            }

            if (!File.Exists(path + @"\" + settingsFolder + @"\" +  bridgelistFile)) //BridgeWordlist.txt
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Noice_Bot_Twitch.SettingsRef.bridgewordListRef.txt";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    File.WriteAllText(path + @"\" + settingsFolder + @"\" + bridgelistFile, result);
                }
                Console.WriteLine("File: " + bridgelistFile + " was missing");
            }
            if (!File.Exists(path + @"\" + settingsFolder + @"\" + whitelistFile)) //Whitelist.txt
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Noice_Bot_Twitch.SettingsRef.whitelistRef.txt";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    File.WriteAllText(path + @"\" + settingsFolder + @"\" + whitelistFile, result);
                }
                Console.WriteLine("File: " + whitelistFile + " was missing");
            }
            if (!File.Exists(path + @"\" + settingsFolder + @"\" + settingsFile)) //Settings.txt
            {
                //Basic Conf, the user has to put in his own oauth key, channelname and Channel ID for Channel Point support
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Noice_Bot_Twitch.SettingsRef.settingsRef.txt";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    File.WriteAllText(path + @"\" + settingsFolder + @"\" + settingsFile, result);
                }
                Console.WriteLine("File: " + settingsFile + " was missing");
            }
            if (!File.Exists(path + @"\" + settingsFolder + @"\" +  soundOffsetFile)) //Soundoffsetfile.txt
            {
                //Sound effects offset file to adjust every sound if needed
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Noice_Bot_Twitch.SettingsRef.soundfileOffsetRef.txt";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    result += "\n" + GenSoundfileOffsetList();
                    File.WriteAllText(path + @"\" + settingsFolder + @"\" + soundOffsetFile, result);
                }
                Console.WriteLine("File: " + soundOffsetFile + " was missing");
            }
            if (!File.Exists(path + @"\" + settingsFolder + @"\" + soundsFile)) //Sounds.txt
            {
                //Sound effects offset file to adjust every sound if needed
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Noice_Bot_Twitch.SettingsRef.soundListRef.txt";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    result += "\n" + GenSoundsListFile();
                    File.WriteAllText(path + @"\" + settingsFolder + @"\" + soundsFile, result);
                }
                Console.WriteLine("File: " + soundsFile + " was missing");
            }

            if (!File.Exists(path + @"\" + settingsFolder + @"\" + commandsFile)) //Commands.json
            {
                //Basic Conf, the user has to put in his own oauth key, channelname and Channel ID for Channel Point support
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Noice_Bot_Twitch.SettingsRef.CommandsRef.json";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    File.WriteAllText(path + @"\" + settingsFolder + @"\" + commandsFile, result);
                }
                Console.WriteLine("File: " + commandsFile + " was missing");
            }
        }

        //File Generation, just a simple string that will be pasted in to the file
        static String GenSoundfileOffsetList()
        {
            string str = "";
            if (soundfilePaths != null)
            {
                foreach (string path in soundfilePaths)
                {
                    str += GetSoundname(path) + ",+0,+0\n";
                }
                return str;
            }
            else return str;
        }
        //Generate the list of all soundfiles in the soundboard
        static String GenSoundsListFile()
        {
            string str = "";
            int counter = 0;

            //Check all files in direktory and subdirektories and add every supported file to the list
            if (Directory.Exists(GetSoundboardPath()))
            {
                soundfilePaths = Directory.EnumerateFiles(GetSoundboardPath(), "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".mp3") || s.EndsWith(".wav") || s.EndsWith(".aiff") || s.EndsWith(".wma")).ToList();
            }

            foreach (string path in soundfilePaths)
            {
                str += counter.ToString() + "," + GetSoundname(path) + "," + path + "\n";
                counter++;
            }
            return str;

        }
        //Update the Soundeffects offset file with new found soundeffects, this will not delete any out
        static void UpdateSoundOffsetFile()
        {
            List<String> data = new List<string>();
            List<String> newData = new List<string>();

            //If the file does not exist, generate it first
            if (File.Exists(path + @"\" + settingsFolder + @"\" + soundOffsetFile)) //Soundoffsetfile.txt
            {
                //Read every colum in the file
                data = File.ReadAllLines(path + @"\" + settingsFolder + @"\" + soundOffsetFile).ToList();

                //For each soundfile check if it's in the list
                foreach(string soundfilePath in soundfilePaths)
                {
                    bool found = false; //Simple bool to determen if was not found

                    //Get the soundfile name
                    string soundFileName = GetSoundname(soundfilePath);

                    //Foreach line in the file
                    foreach(string rawData in data)
                    {
                        //Split the data
                        string[] rawSplit = rawData.Split(',');

                        if(soundFileName == rawSplit[0])
                        {
                            //Found it in the list, break out because it would be a waste of time
                            found = true;
                            break;
                        }
                    }
                    //If it was not found, add it to the newData list and combine them
                    if(!found) newData.Add(soundFileName + ",+0,+0");
                }
                //If something is in the list, add it the the data
                if(newData.Count > 0)
                {
                    //Information about new soundfile found
                    foreach(string s in newData) Console.WriteLine("Added " + s + " in soundfile offset");

                    data.AddRange(newData); //Combine new and old data
                    string writingData = "";
                    //Stitch every object of the list together to a string
                    foreach(string s in data) writingData += s + "\n";
                    //Write the file
                    File.WriteAllText(path + @"\" + settingsFolder + @"\" + soundOffsetFile, writingData);
                }
            } else GenSoundfileOffsetList(); //If the file does not exist, create it
        }
        static void UpdateSoundsListFile()
        {
            List<string> foundSoundfiles = new List<string>();
            List<string> readSoundfiles = new List<string>();
            List<string> unknownSoundFiles = new List<string>();

            //Check all files in direktory and subdirektories and add every supported file to the list
            if (Directory.Exists(GetSoundboardPath()))
            {
                foundSoundfiles = Directory.EnumerateFiles(GetSoundboardPath(), "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".mp3") || s.EndsWith(".wav") || s.EndsWith(".aiff") || s.EndsWith(".wma")).ToList();
            }

            //read soundfiles with id and name in and extract the path of the file
            List<string> sounds = File.ReadAllLines(path + @"\" + settingsFolder + @"\" + soundsFile).ToList();
            foreach (string s in sounds)
            {
                string[] split = s.Split(',');
                try
                {
                    if (File.Exists(split[2])){
                        readSoundfiles.Add(split[2]);
                    }
                } catch { GenSoundsListFile(); }
            }

            Console.WriteLine("found sounds:" + foundSoundfiles.Count + " sounds in file:" + readSoundfiles.Count);

            foreach (string fPath in foundSoundfiles)
            {
                if (readSoundfiles.Exists(x => x.Contains(fPath))) //If the string can be found in the list, ignore
                {
                    //If you can't find it in the list, add it to the list
                }
                else
                {
                    Console.WriteLine("Could not find " + fPath);
                    unknownSoundFiles.Add(fPath);
                }
            }

            int counter = 0;
            string writingData = "";
            foreach (string s in readSoundfiles)
            {
                writingData += counter + "," + GetSoundname(s) + "," + s + "\n";
                counter++;
            }
            foreach (string s in unknownSoundFiles)
            {
                writingData += counter + "," + GetSoundname(s) + "," + s + "\n";
                counter++;
            }

            File.WriteAllText(path + @"\" + settingsFolder + @"\" + soundsFile, writingData);
        }

        //#### Getter Methods for all kind of stuff ####
        //Get the directory where the bot got started
        public static string GetPath()
        {
            return path;
        }
        //Get the path where the Soundboard is
        public static string GetSoundboardPath()
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

        //Get the Alias file name
        public static string GetAliasFile()
        {
            return aliasFile;
        }
        //Get the Blacklist file name
        public static string GetBlacklistFile()
        {
            return blacklistFile;
        }
        //Get the Whitelist file name
        public static string GetWhitelistFile()
        {
            return whitelistFile;
        }

        //###Get Specific Settings out of the Settings.txt
        //irc.twitch.tv or other irc capeble plattform
        public static string GetIrcClient()
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
        }
        //Port to connect to IRC server
        public static int GetPort()
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
        //Name of the bot, possibly useless?
        public static string GetBotName()
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
        //Name of the channel to connect to
        public static string GetChannelName()
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
        //Chanel ID used for Channel Point redemption, you need a tool to get your Channel ID
        public static string GetChannelID()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("channelID=") && s.Length > 10)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT CHANNEL ID DETECTED");
            return "";

        }
        //Oauth Key, needed for connecting to the chat
        public static string GetOAuth()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("oauth=") && s.Length > 12)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT OAUTH DETECTED");
            return "";
        } //IRC Settings
        //Fuck you twitch, this is dumb
        public static string GetAppAuth()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("appauth=") && s.Length > 8)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT APPAUTH DETECTED");
            return "";
        }
        //Basic Text to Speech Speed, how slow is it?
        public static int GetTTSBaseSpeed()
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
        //Max speed of Test to Speech, gotta go extra fast today
        public static int GetTTSMaxSpeed()
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
        //How much talking without a pause can you handle?
        public static int GetMaxTextLength() //Anti Spam
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
        //Maximum amount of bad trashy chars in a comment to read out
        public static int GetSpamThreshold()
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
        //Trust me, you don't want to  hear ASCII emojis. But if you hate yourself, go for it ofc.
        public static bool GetRemoveEmojis()
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
        //Should also be the soundfile on cooldown? Or do you want to hear the same funny haha.mp3 50 times in a row?
        public static bool GetUseSoundcooldown()
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
        //This is the bad stuff. Be aware of it and your wallet.
        public static string GetBadCharList()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("badcharlist=") && s.Length > 12)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            return "";
        }
        //Text to Speech output device number, ask for one if null
        public static bool GetUseTTS()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("enabletts=") && s.Length > 10)
                {
                    string str = s.Substring(s.IndexOf("=") + 1);
                    if (str.Contains("true")) return true;
                    if (str.Contains("false")) return false;
                }
            }
            return false;
        }


        public static int GetTTSOutputDeviceID()
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
        }
        //Get soundboard output device number, ask for one if null
        public static int GetSoundboardOutputDeviceID()
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
        //Get notification output device number, ask for one if null
        public static int GetNotificationOutputDeviceID()
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
        //Get in what order the comment should be read out, leave empty for nothing
        public static string GetNotificationExecutionOrder()
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
        //Volume of notification
        public static float GetNotificationVolume()
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
        //Text to Speech volume
        public static float GetTTSVolume()
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
        //Soundboard base volume (offset will change more)
        public static float GetSoundboardVolume()
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
        //Special character to look for. '!' is standard
        public static String GetCommandCharacter()
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
        //Your personal cool kids list
        public static bool GetWhitelistOnly()
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
        //User cooldown, how much time for THIS specific user
        public static int GetUserCooldown()
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
        //Globally cool, nobody allowed to touch. Bad touch till time over.
        public static int GetGlobalCooldown()
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
        //No bot error respone for a specific time window
        public static int GetResponseCooldown()
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
        //Idk if this even works? 2 soundfiles are requested at the same time, put this time in between when playing to strech out the sound stacking
        public static int GetSoundInterval()
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
        //All paths to the soundfiles
        public static List<String> GetSoundfiles()
        {
            if (soundfilePaths != null) return soundfilePaths;
            else return new List<string>(); //If list is null, return empty list
        }
        //All the subdirektories of the soundboard, these are new libraries that can be also triggerd
        public static List<String> GetSoundboardSubdirektories()
        {
            if (soundboardSubdirektories != null) return soundboardSubdirektories;
            else return new List<string>(); //If list is null, return empty list
        }
        //Offsets of every soundfile
        public static List<String> GetSoundfileOffsetList()
        {
            if(soundfileOffsetList != null) return soundfileOffsetList;
            else return new List<string>(); //If list is null, return empty list
        }
        //Return the created String lists
        //All the bad peoples in your small little life, these are extra bad, right?
        public static List<String> GetBlacklist()
        {
            if(blackList != null) return blackList;
            else return new List<string>(); //If list is null, return empty list
        }
        //The magical bridge that connects the username with his comment. Hand in hand they will exist in a perfect sentence. 
        public static List<String> GetTTSBlacklist()
        {
            if (ttsBlacklist != null) return ttsBlacklist;
            else return new List<string>(); //If list is null, return empty list
        }

        public static List<String> GetBridgeWordList()
        {
            if(bridgeWordList != null) return bridgeWordList;
            else return new List<string>(); //If list is null, return empty list
        }
        //Convert any boring, uncool username in 'John' or 'Yo mama'
        public static List<String> GetAliasList()
        {
            if(aliasList != null) return aliasList;
            else return new List<string>(); //If list is null, return empty list
        }
        //All the nice guy's in one place. These people can use the force/pressing F on the Keyboard at the perfect timing + Can use bot commands (but that's kinda boring)
        public static List<String> GetWhiteList()
        {
            if(whiteList != null) return whiteList;
            else return new List<string>(); //If list is null, return empty list
        }
        //Returns a easy to write soundfile name
        public static List<Command> GetCommandsList()
        {
            if (commands != null) return commands;
            else return new List<Command>();
        }
        
        public static string GetSoundname(string path)
        {
            string str = path.Substring(path.LastIndexOf(@"\") + 1);
            str = Regex.Replace(str, @"\s+", "");
            str = str.ToLower();
            str = str.Trim();
            str = str.Remove(str.Length - 4);
            return str;
        }

        //Get Channel Points Redemption Names
        //Play random with this Redemption
        public static string GetCPPlayRandom()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("cpplayrandom=") && s.Length > 13)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT CP PLAY RANDOM DETECTED");
            return "";
        }
        //Play specific sound file with this Redemption
        public static string GetCPPlayName()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("cpplayname=") && s.Length > 11)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT CP PLAY NAME DETECTED");
            return "";
        }
        //Play specific sound by Number with this Redemption
        public static string GetCPPlayID()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("cpplayid=") && s.Length > 9)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT CP PLAY ID DETECTED");
            return "";
        }
        //Play a random sound from a folder library 
        public static string GetCPPlayFolder()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("cpplayfolder=") && s.Length > 13)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT CP PLAY FOLDER DETECTED");
            return "";
        }
        //Get all available channelpoint redemption names
        public static string GetCPToggleTTS()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("cptoggletts=") && s.Length > 12)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT CP PLAY FOLDER DETECTED");
            return "";
        }
        public static string GetCPTTSRead()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("cpttsread=") && s.Length > 10)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT CP PLAY FOLDER DETECTED");
            return "";

        }


        //Gives you a plimp, bloop, bonk or anyhing else you like
        public static String GetRandomNotificationSound()
        {
            Random rand = new Random();
            string str = notificationList[rand.Next(notificationList.Count)];
            return str;
        }

        //Take a random bridge word out of the bridgeWordList
        public static String GetRandomBridgeWord() 
        {
            Random rand = new Random();
            string str = bridgeWordList[rand.Next(bridgeWordList.Count)];
            return str;
        }
    }
}
