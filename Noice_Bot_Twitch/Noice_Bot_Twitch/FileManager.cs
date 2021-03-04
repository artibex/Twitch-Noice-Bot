using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noice_Bot_Twitch
{
    public class FileManager
    {
        //Path and Filenames of every used file
        private static string path;
        private const string settingsFile = @"Settings.txt";
        private const string aliasFile = @"Aliaslist.txt";
        private const string blacklistFile = @"Blacklist.txt";
        private const string ttsblacklistFile = @"TTSBlacklist.txt";
        private const string bridgelistFile = @"BridgeWordList.txt";
        private const string whitelistFile = @"Whitelist.txt";
        private const string soundOffsetFile = @"SoundfileOffset.txt";
        private const string commandsFile = @"Commands.json";

        //Folder structure
        private const string settingsFolder = @"Settings"; //Settings of the bot
        private const string soundEffectsFolder = @"Soundeffects"; //Soundeffects folder usually contains Notification folder and Soundboard folder
        private const string notificationSoundsFolder = @"Notifications"; //Notification sounds folder
        private const string soundBoardFolder = @"Soundboard"; //Soundboard folder name, can be defined via Settings file

        public FileManager()
        {

        }
    }
}
