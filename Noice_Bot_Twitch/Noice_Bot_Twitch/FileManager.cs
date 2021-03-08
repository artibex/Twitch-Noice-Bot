using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noice_Bot_Twitch
{
    public static class FileManager
    {
        //Path and Filenames of every used file
        private const string settingsFile = @"Settings.txt";
        private const string aliasFile = @"Aliaslist.txt";
        private const string blacklistFile = @"Blacklist.txt";
        private const string ttsblacklistFile = @"TTSBlacklist.txt";
        private const string bridgelistFile = @"BridgeWordList.txt";
        private const string whitelistFile = @"Whitelist.txt";
        private const string soundOffsetFile = @"SoundfileOffset.txt";
        private const string commandsFile = @"Commands.json";
        private const string soundfileIDsFile = @"SoundfileID.json";

        //Folder structure
        private const string settingsFolder = @"Settings"; //Settings of the bot
        private const string soundEffectsFolder = @"Soundeffects"; //Soundeffects folder usually contains Notification folder and Soundboard folder
        private const string notificationSoundsFolder = @"Notifications"; //Notification sounds folder
        private const string soundBoardFolder = @"Soundboard"; //Soundboard folder name, can be defined via Settings file

        //Paths
        private static string path = Directory.GetCurrentDirectory();
        private static string settingsFolderPath = path + Path.DirectorySeparatorChar + settingsFolder + Path.DirectorySeparatorChar;

        public enum FileType { Blacklist, Whitelist, Aliaslist, ttsBlacklist, Bridgelist, Settings, SoundOffset };


        public static void DirectorySetup()
        {
            Directory.CreateDirectory(settingsFolderPath);
        }

        public static List<string> LoadFile(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Blacklist:
                    return Load(settingsFolderPath + blacklistFile);
                case FileType.Whitelist:
                    return Load(settingsFolderPath + whitelistFile);
                case FileType.Aliaslist:
                    return Load(settingsFolderPath + aliasFile);
                case FileType.ttsBlacklist:
                    return Load(settingsFolderPath + ttsblacklistFile);
                case FileType.Bridgelist:
                    return Load(settingsFolderPath + bridgelistFile);
                case FileType.Settings:
                    return Load(settingsFolderPath + settingsFile);
                case FileType.SoundOffset:
                    return Load(settingsFolderPath + soundOffsetFile);
                default:
                    return null;
            }
        }

        private static List<string> Load(string path)
        {
            try
            {
                return File.ReadAllLines(path).ToList();
            }
            catch(FileNotFoundException e)
            {
                //Wenn die Datei nicht gefunden werden kann, wird diese einfach neu erstellt und ein leerer String zurückgegeben
                File.Create(path);
                return null;
            }
        }
        public static SettingsModel LoadAllFiles()
        {
            FileManager.DirectorySetup();

            SettingsModel settingsModel = new SettingsModel();
            settingsModel.Blacklist = LoadFile(FileType.Blacklist);
            settingsModel.Whitelist = LoadFile(FileType.Whitelist);
            settingsModel.Aliaslist = LoadFile(FileType.Aliaslist);
            settingsModel.TtsBlacklist = LoadFile(FileType.ttsBlacklist);
            settingsModel.Settings = LoadFile(FileType.Settings);
            settingsModel.Bridgelist = LoadFile(FileType.Bridgelist);

            return settingsModel;
        }

        public static void SaveFile(FileType fileType, List<string> toSave)
        {
            switch (fileType)
            {
                case FileType.Blacklist:
                    Save(settingsFolderPath + blacklistFile, toSave);
                    break;
                case FileType.Whitelist:
                    Save(settingsFolderPath + whitelistFile, toSave);
                    break;
                case FileType.Aliaslist:
                    Save(settingsFolderPath + aliasFile, toSave);
                    break;
                case FileType.ttsBlacklist:
                    Save(settingsFolderPath + ttsblacklistFile, toSave);
                    break;
                case FileType.Bridgelist:
                    Save(settingsFolderPath + bridgelistFile, toSave);
                    break;
                case FileType.Settings:
                    Save(settingsFolderPath + settingsFile, toSave);
                    break;
                case FileType.SoundOffset:
                    Save(settingsFolderPath + soundOffsetFile, toSave);
                    break;
                default:
                    return;
            }
        }

        private static void Save(string filePath, List<string> list)
        {
            string toWrite = string.Empty;

            foreach(string item in list)
            {
                toWrite += item;
                toWrite += Environment.NewLine;
            }

            try
            {
                File.WriteAllText(filePath, toWrite);
            }
            catch(DirectoryNotFoundException e)
            {
                DirectorySetup();
                File.WriteAllText(filePath, toWrite);
            }
        }

        public static Dictionary<int, string> LoadSoundfileIDs()
        {
            try
            {
                string json = File.ReadAllText(settingsFolderPath + Path.DirectorySeparatorChar + soundfileIDsFile);
                return JsonConvert.DeserializeObject<Dictionary<int, string>>(json);
            }
            catch(FileNotFoundException fnfe)
            {
                return null;
            }
        }

        public static void SaveSoundfileIDs(Dictionary<int, string> soundfileIDs)
        {
            string toWrite = JsonConvert.SerializeObject(soundfileIDs);
            string soundfildIDPath = settingsFolderPath + Path.DirectorySeparatorChar + soundfileIDsFile;
            File.WriteAllText(soundfildIDPath, toWrite);
        }
    }
}
