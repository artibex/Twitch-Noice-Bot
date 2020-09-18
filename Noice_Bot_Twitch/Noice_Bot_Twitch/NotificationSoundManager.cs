namespace Noice_Bot_Twitch
{
    //Play a random Sound from the "Notifications" Folder when a chat message popped up
    public static class NotificationSoundManager
    {
        //FileManager fm;
        //AudioDeviceManager adm;
        static int outputDeviceID = -2; //Get's determend via AudioDeviceManager
        static float notificationVolume = 0.5f; //Volume get's loaded via settings.txt

        //public static NotificationSoundManager(FileManager fm, AudioDeviceManager adm)
        //{
        //    this.fm = fm;
        //    this.adm = adm;
        //    LoadSettings();
        //}

        public static void LoadSettings()
        {
            outputDeviceID = AudioDeviceManager.GetNotificationOutputDeviceID();
            notificationVolume = FileManager.GetNotificationVolume();
        }

        public static void Play() //Play a notification
        {
            Speaker s = new Speaker(FileManager.GetRandomNotificationSound(), outputDeviceID, notificationVolume, false, true);
            AudioMixer.AddNotificationSpeaker(s);
        }
    }
}
