using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noice_Bot_Twitch.Soundboard
{
    public class SoundboardManager
    {
        private SettingsModel _settingsModel;

        public SoundboardManager(SettingsModel settingsModel, Dictionary<int, string> soundfileIDs)
        {
            _settingsModel = settingsModel;

            //Lese vorhandene Soundfiles ein und zur Liste machen
            //Dabei auf neue oder gelöschte Files prüfen (evtl über eigens gespeicherte Liste mit Zuweisung ID zu Sounddatei)
            SoundfileManager soundfileManager = new SoundfileManager(@"C:\Users\Sascha\source\repos\Twitch-Noice-Bot\Noice_Bot_Twitch\Noice_Bot_Twitch\bin\Debug\Soundeffects", _soundfileIDs);


            //Soundboard initialisieren

            //Optional: TTS initialisieren

            //Anmelden an PubSub für Channel Points
            //IRC Client initialisieren fürs Chat lesen (evtl. optional Einstellbar über Settings)
            //Auch auf Commands hören

            //Ablauf Soundboard
            //Zuerst prüfen, ob Whitelist aktiv und User in Whitelist
            //Wenn keine Whitelist, prüfen ob User in Blacklist
            //Chat ausgelöstes SB: Global Timer, User Timer, Soundfile Timer checken und setzen
            //Ausgeben in Chat, welcher Sound abgespielt wird

        }
    }
}
