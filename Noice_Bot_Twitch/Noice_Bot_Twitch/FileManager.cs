using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Noice_Bot_Twitch
{
    //This class Manages all Files the bot needs to operate
    class FileManager
    {
        //Path and Filenames of every used file
        string path;
        string settingsFile = @"Settings.txt"; //Settings file name
        string aliasFile = @"Aliaslist.txt"; //Alias file name
        string blacklistFile = @"Blacklist.txt"; //Blacklist file name
        string bridgelistFile = @"BridgeWordList.txt"; //Bridgewords file name
        string whitelistFile = @"Whitelist.txt"; //Whitelist file name
        string soundOffsetFile = @"SoundfileOffset.txt"; //Soundboard offsets file name

        //Folder structure
        string soundEffectsFolder = @"Soundeffects"; //Soundeffects folder usually contains Notification folder and Soundboard folder
        string notificationSoundsFolder = @"Notifications"; //Notification sounds folder
        string soundBoardFolder = @"Soundboard"; //Soundboard folder name, can be defined via Settings file

        //Alias List, Blacklist, Whitelist, Settings
        List<String> aliasList; //Loaded Aliases
        List<String> blackList; //Loaded Blacklist
        List<String> bridgeWordList; //Loaded Bridgewords
        List<String> whiteList; //Loaded Whitelist
        List<String> settingsList; //Loaded Settings
        List<String> notificationList; //Paths to the notifications sounds
        List<String> soundFiles; //All the Soundfiles as Path
        List<String> soundboardSubdirektories; //Subdirektories of the soundboard folder
        List<String> soundfileOffsetList; //Loaded offsets of soundfiles

        //Init Filemanager
        public FileManager()
        {
            path = Directory.GetCurrentDirectory(); //Get the current execution Directory
            LoadFiles(); //Load the files into string lists
            CheckFileExistence(); //Check file existance of all needed files, create missing ones (with examples)
        }

        public void LoadFiles() //Load all the Files
        {
            try //If an error occures while trying to load a file check the existence
            {
                //Load all notification sounds as paths
                notificationList = Directory.GetFiles(path + @"\" + soundEffectsFolder + @"\" + notificationSoundsFolder).ToList();

                settingsList = File.ReadAllLines(path + @"\" + settingsFile).ToList(); //Read "Settings.txt"
                aliasList = File.ReadAllLines(path + @"\" + aliasFile).ToList(); //Read "Aliaslist.txt"
                blackList = File.ReadAllLines(path + @"\" + blacklistFile).ToList(); //Read "Blacklist.txt"
                bridgeWordList = File.ReadAllLines(path + @"\" + bridgelistFile).ToList(); //Read "BridgeWordList.txt"
                whiteList = File.ReadAllLines(path + @"\" + whitelistFile).ToList(); //Read "Whitelist.txt"

                LoadSoundfiles(); //Check in the given path for soundfiles and put them in the list
                UpdateSoundOffsetFile(); //Before loading the file, update it to check if new sounds are added
                soundfileOffsetList = File.ReadAllLines(path + @"\" + soundOffsetFile).ToList(); //Load sound offset
            } catch
            {
                Console.WriteLine("Failed at loading files");
                CheckFileExistence(); //Check existance of every file
                LoadFiles(); //Load files again
            }
        }

        //Load all soundfiles in a given path
        void LoadSoundfiles()
        {
            soundFiles = new List<string>();
            //Check all files in direktory and subdirektories and add every supported file to the list
            if(Directory.Exists(GetSoundboardPath()))
            {
                soundFiles = Directory.EnumerateFiles(GetSoundboardPath(), "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".mp3") || s.EndsWith(".wav") || s.EndsWith(".aiff") || s.EndsWith(".wma")).ToList();

                soundboardSubdirektories = Directory.GetDirectories(GetSoundboardPath()).ToList(); //Load subdirectories
                Console.WriteLine("Loaded " + soundFiles.Count + " Soundfiles"); //Display the loaded amount in console to flex
            }
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
                //Basic Conf, the user has to put in his own oauth key, channelname and Channel ID for Channel Point support
                File.WriteAllText(path + @"\" + settingsFile, GenSettingsFile());
                Console.WriteLine("File: " + settingsFile + " was missing");
            }
            if (!File.Exists(path + @"\" + soundOffsetFile)) //Soundoffsetfile.txt
            {
                //Sound effects offset file to adjust every sound if needed
                File.WriteAllText(path + @"\" + soundOffsetFile, GenSoundOffsetFile());
                Console.WriteLine("File: " + soundOffsetFile + " was missing");
            }

            //Create Folder Structure for notifications and the soundboard if not existing
            if (!Directory.Exists(path + @"\" + soundEffectsFolder)) Directory.CreateDirectory(path + @"\" + soundEffectsFolder);
            if (!Directory.Exists(path + @"\" + soundEffectsFolder + @"\" + notificationSoundsFolder)) Directory.CreateDirectory(path + @"\" + soundEffectsFolder + @"\" + notificationSoundsFolder);
            if (!Directory.Exists(path + @"\" + soundEffectsFolder + @"\" + soundBoardFolder)) Directory.CreateDirectory(path + @"\" + soundEffectsFolder + @"\" + soundBoardFolder);
        }

        //File Generation, just a simple string that will be pasted in to the file
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
Korpsian
";
            return str;
        }
        String GenBridgeListFile()
        {
            string str = @"says
say
speaks
writes
tells
";
            return str;
        }
        String GenWhitelistFile()
        {
            string str = @"User1
USER2
YourMods
Yourself
YourBro42069
";
            return str;
        }
        String GenSettingsFile()
        {
            string str = @"#See the GitHub Page for how to use this file correctly
--IRC Settings--
ircclient=irc.twitch.tv
port=6667
# The Botname is currenlty not displayed in the chat, idk if I even need it?
botname=noisebot
channelname=
# Get your ID with this: https://chrome.google.com/webstore/detail/twitch-username-and-user/laonpoebfalkjijglbjbnkfndibbcoon
channelID=
# Get your oauth key here: https://www.twitchapps.com/tmi/
oauth=oauth:

--TTS Settings--
# Base speed = normal read speed, when the text is not too long
# TTS speed can be between 1 and 10
ttsbasespeed=3
# Max speed = when the text is getting quit long, hurry up!
ttsmaxspeed=7
# n = notification sound, u = username, b = bridgeword, c = comment
# EXAMPLE: 'order=ubc' Username, bridgeword, comment
# EXAMPLE2: 'order=n' Just notification sound
notificationExecutionOrder=

--Anti Spam Settings--
# Meximum amount of characters the bot read out
maxtextlength=100
# Find 8 bad chars in a message and ignore it
spamthreshold=8
# Remove ASCII emojis when reading out, NOT Twitch emotes
removeemojis=true
# All the bad stuff in one place, feel free to add more
badcharlist=!§$%&/()=?`^\{[]}#

--Audio Device Settings--
# Standard output device is 0 of your PC, leave these options empty to get asked which one you would like to use
ttsoutputdevice=0
soundboardoutputdevice=0
notificationoutputdevice=0
# Notification volume, value between 0 and 1
notificationvolume=0.5
# Text to Speech volume, value between 0 and 1
ttsvolume=0.5
# Soundboard volume, value between 0 and 1, for more adjustments see sound offset settings
soundboardvolume=0.5

--Command Identifier Settings--
# The character to look in the chat, can be changed to anything else, exept empty
commandcharacter=!
# Only users on the whitelist can use this bot (Channel Point redemptions are not influenced by this)
whitelistonly=false

--Soundboard Settings--
# Past path of externel folder here, leave empty for standard
# EXAMPLE: customsoundboardfolder=Z:\Soundeffects\Meme Collection
customsoundboardfolder=
# Cooldown of the specific user, can play sound after 90 seconds again
usercooldown=90
# Global cooldown, nobody can play a sound when it's active
globalcooldown=0
# Lower Bot spamming, ignore every incoming message for 30 seconds, if not completed task
responsecooldown=30
# Prevent sound stacking by putting in a intervall timer, idk if it actually works lul.
soundinterval=0
# Prevent spamming the same soundfile by putting the used file in cooldown connected to the user
usesoundcooldown=true
#Create Channel Point Redemptions with these names to trigger the Soundboard
cpplayrandom=Play Random
# The User have to type in something that these 3 work
cpplayname=Play Name
cpplayid=Play ID
cpplayfolder=Play Folder
";
            return str;
        }
        
        String GenSoundOffsetFile()
        {
            string str = @"---Filename, Volume offset, Timeout offset---
--- Volume Offset between -0.5 and +0.5 (don't forget to put + and - infront of it)
--- Timeout offset can be between -9999 and +9999 or bigger (don't forget to put + and - infront of it)
            //soundfile name, volume offset (+ or - e.g.: -0.2, +0.3), timeout offset (e.g.:-10,+30)";
            str += "\n";
            if (soundFiles != null)
            {
                foreach (string path in soundFiles)
                {
                    str += GetSoundname(path) + ",+0,+0\n";
                }
                return str;
            }
            else return str;
        }
        
        //Update the Soundeffects offset file with new found soundeffects, this will not delete any out
        void UpdateSoundOffsetFile()
        {
            List<String> data = new List<string>();
            List<String> newData = new List<string>();

            //If the file dows not exist, generate it first
            if (File.Exists(path + @"\" + soundOffsetFile)) //Soundoffsetfile.txt
            {
                //Read every colum in the file
                data = File.ReadAllLines(path + @"\" + soundOffsetFile).ToList();

                //For each soundfile check if it's in the list
                foreach(string soundfilePath in soundFiles)
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
                    File.WriteAllText(path + @"\" + soundOffsetFile, writingData);
                }
            } else GenSoundOffsetFile(); //If the file does not exist, create it
        }

        //#### Getter Methods for all kind of stuff ####
        //Get the directory where the bot got started
        public string GetPath()
        {
            return path;
        }
        //Get the path where the Soundboard is
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

        //Get the Alias file name
        public string GetAliasFile()
        {
            return aliasFile;
        }
        //Get the Blacklist file name
        public string GetBlacklistFile()
        {
            return blacklistFile;
        }
        //Get the Whitelist file name
        public string GetWhitelistFile()
        {
            return whitelistFile;
        }

        //###Get Specific Settings out of the Settings.txt
        //irc.twitch.tv or other irc capeble plattform
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
        }
        //Port to connect to IRC server
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
        //Name of the bot, possibly useless?
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
        //Name of the channel to connect to
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
        //Chanel ID used for Channel Point redemption, you need a tool to get your Channel ID
        public string GetChannelID()
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
        //Basic Text to Speech Speed, how slow is it?
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
        //Max speed of Test to Speech, gotta go extra fast today
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
        //How much talking without a pause can you handle?
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
        //Maximum amount of bad trashy chars in a comment to read out
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
        //Trust me, you don't want to  hear ASCII emojis. But if you hate yourself, go for it ofc.
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
        //Should also be the soundfile on cooldown? Or do you want to hear the same funny haha.mp3 50 times in a row?
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
        //This is the bad stuff. Be aware of it and your wallet.
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
        }
        //Text to Speech output device number, ask for one if null
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
        }
        //Get soundboard output device number, ask for one if null
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
        //Get notification output device number, ask for one if null
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
        //Get in what order the comment should be read out, leave empty for nothing
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
        //Volume of notification
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
        //Text to Speech volume
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
        //Soundboard base volume (offset will change more)
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
        //Special character to look for. '!' is standard
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
        //Your personal cool kids list
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
        //User cooldown, how much time for THIS specific user
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
        //Globally cool, nobody allowed to touch. Bad touch till time over.
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
        //No bot error respone for a specific time window
        public int GetResponseCooldown()
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
        //All paths to the soundfiles
        public List<String> GetSoundfiles()
        {
            return soundFiles;
        }
        //All the subdirektories of the soundboard, these are new libraries that can be also triggerd
        public List<String> GetSoundboardSubdirektories()
        {
            return soundboardSubdirektories;
        }
        //Offsets of every soundfile
        public List<String> GetSoundfileOffsetList()
        {
            return soundfileOffsetList;
        }
        //Return the created String lists
        //All the bad peoples in your small little life, these are extra bad, right?
        public List<String> GetBlackList()
        {
            return blackList;
        }
        //The magical bridge that connects the username with his comment. Hand in hand they will exist in a perfect sentence. 
        public List<String> GetBridgeWordList()
        {
            return bridgeWordList;        }
        //Convert any boring, uncool username in 'John' or 'Yo mama'
        public List<String> GetAliasList()
        {
            return aliasList;
        }
        //All the nice guy's in one place. These people can use the force/pressing F on the Keyboard at the perfect timing + Can use bot commands (but that's kinda boring)
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

        //Get Channel Points Redemption Names
        //Play random with this Redemption
        public string GetCPPlayRandom()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("cpplayrandom=") && s.Length > 13)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT CP PLAY RANDOM DETECTED");
            return null;
        }
        //Play specific sound file with this Redemption
        public string GetCPPlayName()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("cpplayname=") && s.Length > 11)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT CP PLAY NAME DETECTED");
            return null;
        }
        //Play specific sound by Number with this Redemption
        public string GetCPPlayID()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("cpplayid=") && s.Length > 9)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT CP PLAY ID DETECTED");
            return null;
        }
        //Play a random sound from a folder library 
        public string GetCPPlayFolder()
        {
            foreach (string s in settingsList)
            {
                if (s.Contains("cpplayfolder=") && s.Length > 13)
                {
                    return s.Substring(s.IndexOf("=") + 1);
                }
            }
            Console.WriteLine("INCORRECT CP PLAY FOLDER DETECTED");
            return null;
        }

        //Gives you a plimp, bloop, bonk or anyhing else you like
        public String GetRandomNotificationSound()
        {
            Random rand = new Random();
            string str = notificationList[rand.Next(notificationList.Count)];
            return str;
        }

        //Take a random bridge word out of the bridgeWordList
        public String GetRandomBridgeWord() 
        {
            Random rand = new Random();
            string str = bridgeWordList[rand.Next(bridgeWordList.Count)];
            return str;
        }
    }
}
