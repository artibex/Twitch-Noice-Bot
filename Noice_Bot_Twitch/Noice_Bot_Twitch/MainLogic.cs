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
        private UISettings _uiSettings;
        private UI _ui;

        public MainLogic()
        {
            //Settings laden
            _uiSettings = new UISettings();

            //UI starten
            Thread uiThread = new Thread(new ThreadStart(StartUI));
            uiThread.Start();

            TwitchAPI twitchAPI = new TwitchAPI();
            twitchAPI.GetUserID("unknownSaschka");
        }

        private void StartUI()
        {
            Application.EnableVisualStyles();
            _ui = new UI(_uiSettings);
            _ui.Disposed += (sender, eventArgs) => EndProgram();
            Application.Run(_ui);
        }

        private void EndProgram()
        {
            Application.Exit();
        }
    }
}
