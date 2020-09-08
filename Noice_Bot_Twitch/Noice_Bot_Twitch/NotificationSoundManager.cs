namespace Noice_Bot_Twitch
{
    //Play a random Sound from the "Notifications" Folder when a chat message popped up
    class NotificationSoundManager
    {
        FileManager fm;
        AudioDeviceManager adm;
        int outputDeviceID = -2; //Get's determend via AudioDeviceManager
        float notificationVolume = 0.5f; //Volume get's loaded via settings.txt

        public NotificationSoundManager(FileManager fm, AudioDeviceManager adm)
        {
            this.fm = fm;
            this.adm = adm;
            LoadSettings();
        }

        public void LoadSettings()
        {
            outputDeviceID = adm.GetNotificationOutputDeviceID();
            notificationVolume = fm.GetNotificationVolume();
        }

        public void Play() //Play a notification
        {
            using (Speaker s = new Speaker(fm.GetRandomNotificationSound(), outputDeviceID, notificationVolume, false, false)) ;
        }
    }
}
