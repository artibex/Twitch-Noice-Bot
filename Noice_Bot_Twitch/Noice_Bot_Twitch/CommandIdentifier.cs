﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TwitchLib.Client;

using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Noice_Bot_Twitch
{
    //Checks each commant and looks for a bot command (usually begins with "!") and executes further
    public static class CommandIdentifier
    {
        //public static SoundboardManager sm;
        //public static FileManager fm;
        //public static IrcClient client;
        //public static AudioDeviceManager adm;
        //public static NotificationSoundManager nsm;
        //public static TTS tts;
        //public static CommantProcessor cp;
        //public static ExecutionOrderManager eom;
        public static string commandCharacter;
        public static bool whitelistOnly = false;
        public static List<Command> commands = new List<Command>();
        public static PubSubService pubsub;

        //If the user put a "!" or similar infront, check list for commands
        //public CommandIdentifier(SoundboardManager sm, FileManager fm, IrcClient client, AudioDeviceManager adm, NotificationSoundManager nsm, TTS tts, CommantProcessor cp, ExecutionOrderManager eom)
        //{
        //    this.sm = sm;
        //    this.fm = fm;
        //    this.client = client;
        //    this.adm = adm;
        //    this.nsm = nsm;
        //    this.tts = tts;
        //    this.cp = cp;
        //    this.eom = eom;
        //    LoadSettings();
        //}

        public static void LoadSettings()
        {
            commandCharacter = FileManager.GetCommandCharacter();
            whitelistOnly = FileManager.GetWhitelistOnly();
            commands = FileManager.GetCommandsList();
        }
        public static void LoadSettings(IrcClient cl, PubSubService pub)
        {
            pubsub = pub;
            //client = TwitchLibChatClient.client;
            LoadSettings();
        }


        //Return false if it's not a command
        public static bool CheckCommand(Comment c)
        {
            //Console.WriteLine("Checking " + c.comment);
            if(c.comment != null && c.comment.StartsWith(commandCharacter))
            {
                if (whitelistOnly) //Check if only whitelisted people are allowed (Channel Point redemption work allways)
                {
                    foreach (string s in FileManager.GetWhiteList()) if (c.user.ToLower() == s.ToLower()) CheckCommandList(c);
                } else CheckCommandList(c);
                return true;
            }
            return false;
        }
        static void CheckCommandList(Comment c) //Check command against list and execute
        {
            if(c.comment.Length > 2)
            {
                string userCommand = c.comment.Substring(1, c.comment.Length-1).ToLower();
                if (userCommand.Contains(" ")) userCommand = userCommand.Substring(0, userCommand.IndexOf(" "));

                Command foundCommand = new Command("","","");
                foreach(Command co in commands) if (userCommand.Trim() == co.name.ToLower()) foundCommand = co;
                if(foundCommand.name != "")
                {
                    switch (foundCommand.functionName)
                    {
                        case "noice":
                            Help(foundCommand);
                            break;
                        case "help":
                            Help(foundCommand);
                            break;
                        case "ttsenabled":
                            TTSEnabled(foundCommand, c);
                            break;
                        case "ttsvol":
                            TTSVol(foundCommand, c);
                            break;
                        case "ttsskip":
                            TTSSkip(foundCommand, c);
                            break;
                        case "ttsexecutionorder":
                            TTSExecutionOrder(foundCommand, c);
                            break;
                        case "ttsspeed":
                            TTSSpeed(foundCommand, c);
                            break;
                        case "ttsmaxspeed":
                            TTSMaxSpeed(foundCommand, c);
                            break;
                        case "ttstextlength":
                            TTSTextLength(foundCommand, c);
                            break;
                        case "ttsspamthreshold":
                            TTSSpamThreshHold(foundCommand, c);
                            break;
                        case "ttsremoveemojis":
                            TTSRemoveEmojis(foundCommand, c);
                            break;
                        case "ttsbadcharlist":
                            TTSBadCharList(foundCommand, c);
                            break;
                        case "ttsread":
                            TTSRead(foundCommand, c);
                            break;
                        case "ttsoutput":
                            TTSOutput(foundCommand, c);
                            break;
                        case "soundboardoutput":
                            SoundboardOutput(foundCommand, c);
                            break;
                        case "notificationoutput":
                            NotificationOutput(foundCommand, c);
                            break;
                        case "notificationvol":
                            NotificationVol(foundCommand, c);
                            break;
                        case "play":
                            Play(foundCommand, c);
                            break;
                        case "sbvol":
                            SBVol(foundCommand, c);
                            break;
                        case "sbglobalcooldown":
                            SBGlobalCooldown(foundCommand, c);
                            break;
                        case "sbusercooldown":
                            SBUserCooldown(foundCommand, c);
                            break;
                        case "sbresponsecooldown":
                            SBResponseCooldown(foundCommand, c);
                            break;
                        case "sbsoundinterval":
                            SBSoundInterval(foundCommand, c);
                            break;
                        case "sbenablesoundcooldown":
                            SBEnableSoundcooldown(foundCommand, c);
                            break;
                        case "sbreload":
                            SBReload(foundCommand, c);
                            break;
                        case "reloadsettings":
                            ReloadSettings(foundCommand, c);
                            break;
                        default:
                            //Do nothing
                            break;
                    }
                }
            }
        }

        static void Help(Command command)
        {
            string collector = "";
            foreach (Command co in commands) collector += co.name + ", ";
            collector = collector.Substring(0, collector.Length - 2); //Cut away last ','
            //TwitchLibChatClient.SendMessage(command.helpComment + " " + collector);
            TwitchLibChatClient.SendMessage(command.helpComment + " " + collector);
        }
        static void TTSEnabled(Command command, Comment c)
        {
            string [] s = c.comment.Split();
            if (s.Length < 2) return;
            if(s[1].Contains("?"))
            {
                //TwitchLibChatClient.SendMessage(command.helpComment);
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if (s[1].Contains("true"))
            {
                TTS.useTTS = true;
                //client.SendChatMessage("Enabled TTS");
                TwitchLibChatClient.SendMessage("Enabled TTS");
            }
            else if (s[1].Contains("false"))
            {
                TTS.useTTS = false;
                //client.SendChatMessage("Disabled TTS");
                TwitchLibChatClient.SendMessage("Enabled TTS");
            }
        }
        static void TTSVol(Command command, Comment c)
        {
            string[] s = c.comment.Split();
            float f;
            if (s.Length < 2)
            {
                //client.SendChatMessage("Current TTS Volume=" + TTS.ttsVolume);
                TwitchLibChatClient.SendMessage("Current TTS Volume=" + TTS.ttsVolume);
                return;
            }
            if (s[1].Contains("?"))
            {
                //client.SendChatMessage(command.helpComment);
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if (float.TryParse(s[1], out f))
            {
                if (f > 0 && f <= 1)
                {
                    TTS.ttsVolume = f;
                    //client.SendChatMessage("Set TTS volume to " + f);
                    TwitchLibChatClient.SendMessage("Set TTS volume to " + f);
                    return;
                }
                //client.SendChatMessage("TTS volume must be between 0 and 1");
                TwitchLibChatClient.SendMessage("TTS volume must be between 0 and 1");

            }
            else 
            {
                //client.SendChatMessage(command.helpComment);
                TwitchLibChatClient.SendMessage(command.helpComment);
            };
        }
        static void TTSSkip(Command command, Comment c)
        {
            string[] s = c.comment.Split();
            if (s.Length > 1)
            {
                if (s[1].Contains("?"))
                {
                    //client.SendChatMessage(command.helpComment);
                    TwitchLibChatClient.SendMessage(command.helpComment);
                    return;
                }
            }
            //client.SendChatMessage("Skipping current TTS");
            TwitchLibChatClient.SendMessage("Skipping current TTS");
            AudioMixer.StopTTS();
        }
        static void TTSExecutionOrder(Command command, Comment c)
        {
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                //client.SendChatMessage(command.helpComment);
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            foreach(char ch in s[1])
            {
                switch(ch)
                {
                    case 'u':
                        break;
                    case 'b':
                        break;
                    case 'c':
                        break;
                    case 'n':
                        break;
                    default:
                        //client.SendChatMessage("Only chars u,b,c,n are usable");
                        TwitchLibChatClient.SendMessage("Only chars u,b,c,n are usable");
                        return;
                }
            }
            ExecutionOrderManager.executionOrder = s[1];
            //client.SendChatMessage("New execution order= " + s[1]);
            TwitchLibChatClient.SendMessage("New execution order= " + s[1]);

        }
        static void TTSSpeed(Command command, Comment c)
        {
            int i;
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                //client.SendChatMessage(command.helpComment);
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if(int.TryParse(s[1], out i)) {
                if(i > 0 && i <= 10)
                {
                    if(i < TTS.fastestSpeed)
                    {
                        TTS.normalSpeed = i;
                        //client.SendChatMessage("Set TTS base speed to " + i);
                        TwitchLibChatClient.SendMessage("Set TTS base speed to " + i);
                    }
                    else
                    {
                        //client.SendChatMessage("TTS base speed can't be higher then max speed. Max Speed=" + TTS.fastestSpeed);
                        TwitchLibChatClient.SendMessage("TTS base speed can't be higher then max speed. Max Speed=" + TTS.fastestSpeed);
                    }
                }
            }
        }
        static void TTSMaxSpeed(Command command, Comment c)
        {
            int i;
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                //client.SendChatMessage(command.helpComment);
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if (int.TryParse(s[1], out i))
            {
                if (i > 0 && i <= 10)
                {
                    if(i > TTS.normalSpeed)
                    {
                        TTS.fastestSpeed = i;
                        //client.SendChatMessage("Set TTS max speed to " + i);
                        TwitchLibChatClient.SendMessage("Set TTS max speed to " + i);

                    }
                    else
                    {
                        //client.SendChatMessage("TTS max speed must be higher then base speed. Base speed=" + TTS.normalSpeed);
                        TwitchLibChatClient.SendMessage("TTS max speed must be higher then base speed. Base speed=" + TTS.normalSpeed);
                    }
                }
            }

        }
        static void TTSTextLength(Command command, Comment c)
        {
            int i;
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                //client.SendChatMessage(command.helpComment);
                TwitchLibChatClient.SendMessage(command.helpComment);

                return;
            }
            if(int.TryParse(s[1], out i))
            {
                if(i > 0)
                {
                    CommentProcessor.maxTextLength = i;
                    //client.SendChatMessage("Set TTS max text length to " + i);
                    TwitchLibChatClient.SendMessage("Set TTS max text length to " + i);

                }
                else
                {
                    TwitchLibChatClient.SendMessage("Value must be higher then 0");
                    TwitchLibChatClient.SendMessage("Set TTS max text length to " + i);

                }
            }
        }
        static void TTSSpamThreshHold(Command command, Comment c)
        {
            int i;
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if (int.TryParse(s[1], out i))
            {
                if (i > 0)
                {
                    CommentProcessor.spamThreshold = i;
                    TwitchLibChatClient.SendMessage("Set TTS spam threshold to " + i);
                }
                else TwitchLibChatClient.SendMessage("Value must be higher then 0");
            }

        }
        static void TTSRemoveEmojis(Command command, Comment c)
        {
            bool b;
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if(bool.TryParse(s[1], out b))
            {
                CommentProcessor.removeEmojis = b;
                TwitchLibChatClient.SendMessage("Removing ASCII Emojis set to " + b.ToString());
            }
        }
        static void TTSBadCharList(Command command, Comment c)
        {
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?") && s[1].Length < 2)
            {
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            CommentProcessor.badChars = s[1];
            TwitchLibChatClient.SendMessage("Updated bad char list. New list: " + s[1]);
        }
        static void TTSRead(Command command, Comment c)
        {
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }

            string ttsText = "";
            int count = 0;
            foreach(string split in s)
            {
                if (count > 0) ttsText += split + " ";
                count++;
            }
            if (!TTS.useTTS)
            {
                TTS.useTTS = true;
                TTS.Speak(ttsText);
                TTS.useTTS = false;
            }
            else TTS.Speak(ttsText);
        }
        static void TTSOutput(Command command, Comment c)
        {
            int i;
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if(int.TryParse(s[1], out i))
            {
                AudioDeviceManager.ttsOutputDeviceID = i;
                TwitchLibChatClient.SendMessage("Set TTS output device to " + i);
            }
        }
        static void SoundboardOutput(Command command, Comment c)
        {
            int i;
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if (int.TryParse(s[1], out i))
            {
                AudioDeviceManager.soundboardOutputDeviceID = i;
                TwitchLibChatClient.SendMessage("Set Soundboard output device to " + i);
            }
        }
        static void NotificationOutput(Command command, Comment c)
        {
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
        }
        static void NotificationVol(Command command, Comment c)
        {
            float f;
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if (float.TryParse(s[1], out f))
            {
                if (f > 0 && f <= 1)
                {
                    NotificationSoundManager.notificationVolume = f;
                    TwitchLibChatClient.SendMessage("Set notification volume to " + f);
                    return;
                }
                TwitchLibChatClient.SendMessage("notification volume must be between 0 and 1");
            }
            else TwitchLibChatClient.SendMessage(command.helpComment);
        }
        static void Play(Command command, Comment c)
        {
            SoundboardManager.PlaySoundeffect(c, command);
        }
        static void SBVol(Command command, Comment c)
        {
            float f;
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if (float.TryParse(s[1], out f))
            {
                if (f > 0 && f <= 1)
                {
                    SoundboardManager.sbVolume = f;
                    TwitchLibChatClient.SendMessage("Set soundboard volume to " + f);
                    return;
                }
                TwitchLibChatClient.SendMessage("soundboard volume must be between 0 and 1");
            }
            else TwitchLibChatClient.SendMessage(command.helpComment);

        }
        static void SBGlobalCooldown(Command command, Comment c)
        {
            int i;
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if (int.TryParse(s[1], out i))
            {
                if (i >= 0)
                {
                    SoundboardManager.globalCooldown = i;
                    TwitchLibChatClient.SendMessage("Set global soundboard cooldown to " + i);
                }
                else TwitchLibChatClient.SendMessage(command.helpComment);
            }
        }
        static void SBUserCooldown(Command command, Comment c)
        {
            int i;
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if (int.TryParse(s[1], out i))
            {
                if (i >= 0)
                {
                    SoundboardManager.userCooldown = i;
                    TwitchLibChatClient.SendMessage("Set soundboard user cooldown to " + i);
                }
                else TwitchLibChatClient.SendMessage(command.helpComment);
            }
        }
        static void SBResponseCooldown(Command command, Comment c)
        {
            int i;
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if (int.TryParse(s[1], out i))
            {
                if (i >= 0)
                {
                    SoundboardManager.responseCooldown = i;
                    TwitchLibChatClient.SendMessage("Set soundboard response cooldown to " + i);
                }
                else TwitchLibChatClient.SendMessage(command.helpComment);
            }
        }
        static void SBSoundInterval(Command command, Comment c)
        {
            int i;
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if (int.TryParse(s[1], out i))
            {
                if (i >= 0)
                {
                    SoundboardManager.soundInterval = i;
                    TwitchLibChatClient.SendMessage("Set soundboard interval cooldown to " + i);
                }
                else TwitchLibChatClient.SendMessage(command.helpComment);
            }
        }
        static void SBEnableSoundcooldown(Command command, Comment c)
        {
            string[] s = c.comment.Split();
            if (s.Length < 2) return;
            if (s[1].Contains("?"))
            {
                TwitchLibChatClient.SendMessage(command.helpComment);
                return;
            }
            if (s[1].Contains("true"))
            {
                SoundboardManager.useSoundCooldown = true;
                TwitchLibChatClient.SendMessage("Enabled Soundcooldown system");
            }
            else if (s[1].Contains("false"))
            {
                SoundboardManager.useSoundCooldown = false;
                TwitchLibChatClient.SendMessage("Disabled Soundcooldown system");
            }
        }
        static void SBReload(Command command, Comment c)
        {
            string[] s = c.comment.Split();
            if (s.Length > 1)
            {
                if (s[1].Contains("?"))
                {
                    TwitchLibChatClient.SendMessage(command.helpComment);
                    return;
                }
            }
            FileManager.LoadSoundfiles();
            SoundboardManager.LoadSettings();
            TwitchLibChatClient.SendMessage("Soundboard reloaded");
        }
        static void ReloadSettings(Command command, Comment c)
        {
            string[] s = c.comment.Split();
            if (s.Length > 1)
            {
                if (s[1].Contains("?"))
                {
                    TwitchLibChatClient.SendMessage(command.helpComment);
                    return;
                }
            }
            FileManager.LoadSettings();
            AudioDeviceManager.LoadSettings();
            CommandIdentifier.LoadSettings();
            CommentProcessor.LoadSettings();
            ExecutionOrderManager.LoadSettings();
            if (pubsub != null)
            {
                pubsub.Disconnect();
                pubsub.LoadSettings();
            }
            SoundboardManager.LoadSettings();
            TTS.LoadSettings();
            TwitchLibChatClient.SendMessage("Reloaded all settings");
        }

        //Trigger with Channel Points
        public static bool CheckCommand(dynamic redemption)
        {
            string rType = redemption.type;

            switch(rType) {
                case "MESSAGE":
                    break;
                default:
                    Console.WriteLine("<"+redemption.type+">");
                    return false;
            }

            string redemption2 = redemption.data.message;
            dynamic redeption3 = Newtonsoft.Json.Linq.JObject.Parse(redemption2);
            string rTitle = redeption3.data.redemption.reward.title;
            
            //Play Random
            if (rTitle.ToLower() == FileManager.GetCPPlayRandom().ToLower())
            {
                SoundboardManager.PlayRandom();
                return true;
            }
            //Play Name or Random
            else if (rTitle.ToLower() == FileManager.GetCPPlayName().ToLower())
            {
                string rMessage = redeption3.data.redemption.user_input;
                SoundboardManager.PlayName(rMessage);
                return true;
            }
            //Play ID or Random
            else if (rTitle.ToLower() == FileManager.GetCPPlayID().ToLower())
            {
                string rMessage = redeption3.data.redemption.user_input;
                SoundboardManager.PlayID(rMessage);
                return true;
            }
            //Play Folder or Random
            else if (rTitle.ToLower() == FileManager.GetCPPlayFolder().ToLower())
            {
                string rMessage = redeption3.data.redemption.user_input;
                SoundboardManager.PlayFolder(rMessage);
                return true;
            }
            else if (rTitle.ToLower() == FileManager.GetCPToggleTTS().ToLower())
            {
                if (TTS.useTTS)
                {
                    TTS.useTTS = false;
                    TwitchLibChatClient.SendMessage("Disabled TTS");
                }
                else
                {
                    TTS.useTTS = true;
                    TwitchLibChatClient.SendMessage("Enabled TTS");
                }
                return true;
            }
            else if (rTitle.ToLower() == FileManager.GetCPTTSRead().ToLower())
            {
                string rMessage = redeption3.data.redemption.user_input;
                if (TTS.useTTS) TTS.Speak(rMessage);
                else
                {
                    TTS.useTTS = true;
                    TTS.Speak(rMessage);
                    TTS.useTTS = false;
                }
                return true;
            }
            return false;
        }
    }
}
