using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noice_Bot_Twitch
{
    //Play a random Sound from the "Notifications" Folder
    class NotificationSoundManager
    {
        FileManager fm;
        AudioDeviceManager adm;
        int outputDeviceID = -2;
        float notificationVolume = 0.5f;

        public NotificationSoundManager(FileManager fm, AudioDeviceManager adm)
        {
            this.fm = fm;
            this.adm = adm;
            outputDeviceID = adm.GetNotificationOutputDeviceID();
            notificationVolume = fm.GetNotificationVolume();
        }

        public void Play() //Play a notification
        {
            using (Speaker s = new Speaker(fm.GetRandomNotificationSound(), outputDeviceID, notificationVolume, false)) ;
        }

    }
}
