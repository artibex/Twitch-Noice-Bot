namespace Noice_Bot_Twitch
{
    //Play a random Sound from the "Notifications" Folder when a chat message popped up
    public static class NotificationSoundManager
    {
        //FileManager fm;
        //AudioDeviceManager adm;
        static int outputDeviceID = -2; //Get's determend via AudioDeviceManager
        private static float _notificationVolume = 0.5f; //Volume get's loaded via settings.txt
        public static float notificationVolume
        {
            get { return _notificationVolume; }
            set { if (value > 0 && value <= 1) _notificationVolume = value; }
        }


        //public static NotificationSoundManager(FileManager fm, AudioDeviceManager adm)
        //{
        //    this.fm = fm;
        //    this.adm = adm;
        //    LoadSettings();
        //}

        public static void LoadSettings()
        {
            outputDeviceID = AudioDeviceManager.GetNotificationOutputDeviceID();
            _notificationVolume = FileManagerOld.GetNotificationVolume();
        }

        public static void Play() //Play a notification
        {
            Speaker s = new Speaker(FileManagerOld.GetRandomNotificationSound(), outputDeviceID, _notificationVolume, false, true);
            AudioMixer.AddNotificationSpeaker(s);
        }
    }
}
