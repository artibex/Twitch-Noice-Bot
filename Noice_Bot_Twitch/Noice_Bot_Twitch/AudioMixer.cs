using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noice_Bot_Twitch
{
    public static class AudioMixer
    {
        private static List<Speaker> ttsSpeakers = new List<Speaker>();
        private static List<Speaker> sbSpeakers = new List<Speaker>();
        private static List<Speaker> notificationSpeakers = new List<Speaker>();

        public static void CheckLists() //In case some speakers are null
        {
            foreach (Speaker s in ttsSpeakers.ToList()) if (s == null) ttsSpeakers.Remove(s);
            foreach (Speaker s in sbSpeakers.ToList()) if (s == null) sbSpeakers.Remove(s);
            foreach (Speaker s in notificationSpeakers.ToList()) if (s == null) notificationSpeakers.Remove(s);
        }

        public static void AddTTSSpeaker(Speaker s)
        {
            ttsSpeakers.Add(s);
        }
        public static void AddSoundboardSpeaker(Speaker s)
        {
            sbSpeakers.Add(s);
        }
        public static void AddNotificationSpeaker(Speaker s)
        {
            notificationSpeakers.Add(s);
        }
        public static void RemoveSpeaker(Speaker rmSpeaker)
        {
            foreach (Speaker s in ttsSpeakers.ToList()) if (s == rmSpeaker) ttsSpeakers.Remove(s);
            foreach (Speaker s in sbSpeakers.ToList()) if (s == rmSpeaker) sbSpeakers.Remove(s);
            foreach (Speaker s in notificationSpeakers.ToList()) if (s == rmSpeaker) notificationSpeakers.Remove(s);
        }

        public static void StopTTS()
        {
            Console.WriteLine("Stopping TTS");
            foreach (Speaker s in ttsSpeakers) if(s != null) s.stop = true;
        }
        public static void StopSoundboard()
        {
            foreach (Speaker s in sbSpeakers) if (s != null) s.stop = true;
        }
        public static void StopNotification()
        {
            foreach (Speaker s in notificationSpeakers) if (s != null) s.stop = true;
        }
    }
}
