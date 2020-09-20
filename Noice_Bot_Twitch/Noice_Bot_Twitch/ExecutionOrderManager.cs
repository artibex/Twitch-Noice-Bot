using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noice_Bot_Twitch
{
    public static class ExecutionOrderManager
    {
        private static string _executionOrder = "";
        public static string executionOrder
        {
            get { return _executionOrder; }
            set { _executionOrder = value; }
        }

        //FileManager fm;
        //NotificationSoundManager nsm;
        //CommantProcessor cp;

        //public static ExecutionOrderManager(FileManager fm, NotificationSoundManager nsm, CommantProcessor cp)
        //{
        //    this.fm = fm;
        //    this.nsm = nsm;
        //    this.cp = cp;
        //    LoadSettings();
        //}

        public static void LoadSettings()
        {
            _executionOrder = FileManager.GetNotificationExecutionOrder();
        }

        public static String GetTTSText(Comment c)
        {
            string ttsText = "";

            if (!String.IsNullOrWhiteSpace(c.user) && !String.IsNullOrWhiteSpace(c.comment) && _executionOrder != "")
            {
                foreach (char exe in _executionOrder)
                {
                    switch (exe)
                    {
                        case 'u':
                            ttsText += " " + c.user;
                            break;
                        case 'c':
                            ttsText += " " + c.comment;
                            break;
                        case 'b':
                            ttsText += " " + FileManager.GetRandomBridgeWord();
                            break;
                        case 'n':
                            NotificationSoundManager.Play();
                            break;
                        default:
                            Console.WriteLine("Wrong execution order letter detected. Current execution order: " + _executionOrder);
                            break;
                    }
                }
                if (ttsText.IndexOf(" ") == 0) ttsText = ttsText.Substring(1, ttsText.Length - 1);
                return ttsText;
            }
            else return "";
        }
    }
}
