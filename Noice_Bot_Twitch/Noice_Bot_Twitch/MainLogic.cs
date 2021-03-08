using Noice_Bot_Twitch.Soundboard;
using Noice_Bot_Twitch.Twitch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Noice_Bot_Twitch
{
    public class MainLogic
    {
        private UI _ui;
        private SettingsModel _settingsModel;

        public MainLogic()
        {
            //Settings laden
            _settingsModel = FileManager.LoadAllFiles();     //Alle Settings und Listen laden, bevor weitere Dienste gestartet werden

            //UI starten
            Thread uiThread = new Thread(new ThreadStart(StartUI));
            uiThread.Start();

            SoundboardManager soundboardManager = new SoundboardManager(_settingsModel, FileManager.LoadSoundfileIDs());

            TwitchAPI twitchAPI = new TwitchAPI();
            //twitchAPI.GetUserID("unknownSaschka");
        }

        private void StartUI()
        {
            Application.EnableVisualStyles();
            _ui = new UI(_settingsModel);
            _ui.Disposed += (sender, eventArgs) => EndProgram();
            Application.Run(_ui);
        }

        private void EndProgram()
        {
            Application.Exit();
        }
    }
}
