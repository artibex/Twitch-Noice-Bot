using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

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
        string soundFile = @"synth.wav";

        //Alias List, Blacklist, Whitelist, Settings
        List<String> aList;
        List<String> blackList;
        List<String> bridgeWordList;
        List<String> wList;
        List<String> sList;

        public FileManager()
        {
            path = Directory.GetCurrentDirectory(); //Get the current execution Directory
            CheckExistence(); //Check file existance of all needed files, create missing ones (with examples)
            LoadFiles(); //Load the files into a string list object
        }

        public void LoadFiles() //Load all the Files
        {
            try //If an error occures while trying to load a file check the existence
            {
                aList = File.ReadAllLines(path + @"\" + aliasFile).ToList();
                blackList = File.ReadAllLines(path + @"\" + blacklistFile).ToList();
                bridgeWordList = File.ReadAllLines(path + @"\" + bridgelistFile).ToList();
                wList = File.ReadAllLines(path + @"\" + whitelistFile).ToList();
                sList = File.ReadAllLines(path + @"\" + settingsFile).ToList();
            } catch
            {
                CheckExistence(); //Check existance
                LoadFiles(); //Load files again
            }
        }
        void CheckExistence() //Does all the wanted files exist? No? Then Create them and put examples in it
        {
            if(!File.Exists(path + @"\" + aliasFile)) //Alislist.txt
            {
                string str = "USER1,NewName\nuseer2,OtherNewName\nuSeR3,CoolName"; //Example
                File.WriteAllText(path + @"\" + aliasFile, str);
                Console.WriteLine("File: " + aliasFile + " was missing");
            }
            if (!File.Exists(path + @"\" + blacklistFile)) //Blacklist.txt
            {
                string str = "USER1\nuser2\nuSeR3"; //Example
                File.WriteAllText(path + @"\" + blacklistFile, str);
                Console.WriteLine("File: " + blacklistFile + " was missing");
            }
            if (!File.Exists(path + @"\" + bridgelistFile)) //Bridgelist.txt
            {
                string str = "says\nsay\ntells";
                File.WriteAllText(path + @"\" + bridgelistFile, str);
                Console.WriteLine("File: " + bridgelistFile + " was missing");
            }

            if (!File.Exists(path + @"\" + whitelistFile)) //Whitelist.txt
            {
                string str = "USER1\nuser2\nuSeR3"; //Example
                File.WriteAllText(path + @"\" + whitelistFile, str);
                Console.WriteLine("File: " + whitelistFile + " was missing");
            }
            if (!File.Exists(path + @"\" + settingsFile)) //Settings.txt
            {
                //Basic Conf, the user has to put in his own oauth key and channelname
                string str = "";
                str = str + "--IRC Settings--";
                str = str + "\nircclient=irc.twitch.tv\nport=6667\nbotname=noisebot\nchannelname=\noauth=oauth:"; //IRC Settings
                str = str + "\n\n--TTS Settings--";
                str = str + "\nttsbasespeed=3\nttsmaxspeed=7"; //TTS Settings
                str = str + "\n\n--AntiSpam Settings--";
                str = str + "\nmaxtextlength=150\nspamthreshold=15\nremoveemojis=true\n" + @"badcharlist=!§$%&/()=?`^\{[]}#"; //anti spam settings
                str = str + "\n\n--Sound Device Settings--";
                str = str + "\nttsoutputdevice=\nsoundboardoutputdevice=";
                File.WriteAllText(path + @"\" + settingsFile, str);
                Console.WriteLine("File: " + settingsFile + " was missing");
            }
        }

        //Getter methos for general path, alias, blacklist, whitelist and soundfile
        public string GetPath()
        {
            return path;
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
        public string GetSoundFile()
        {
            return soundFile;
        }

        //Get Specific Settings out of the Settings.txt
        public string GetIrcClient()
        {
            foreach(string s in sList)
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
            foreach (string s in sList)
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
            foreach (string s in sList)
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
            foreach (string s in sList)
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
            foreach (string s in sList)
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
            foreach (string s in sList)
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
            foreach (string s in sList)
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
            foreach (string s in sList)
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
            foreach (string s in sList)
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
            foreach (string s in sList)
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
        public string GetBadCharList()
        {
            foreach (string s in sList)
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
            foreach (string s in sList)
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
            foreach (string s in sList)
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
        } //Audio Device Settings


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
            return aList;
        }
        public List<String> GetWhiteList()
        {
            return wList;
        }

        public String GetRandomBridgeWord() //Take a random bridge word out of Brideword.txt and return it
        {
            Random rand = new Random();
            string str = bridgeWordList[rand.Next(bridgeWordList.Count)];
            return str;
        }
    }
}
