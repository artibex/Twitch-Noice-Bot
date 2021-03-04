using System;
using NAudio.Wave;
using System.IO;
using System.Threading;


namespace Noice_Bot_Twitch
{
    //Speaker to play soundfiles
    public class Speaker : IDisposable
    {
        string filepath; //Path to file to play
        bool deleteFile; //Should the file be deleted after using?
        int outputDeviceID; //ID where to play
        float volume = 0.9f; //Volume of speaker (Is global, need to create a mixer I think)
        public bool stop = false; //Bool to stop current speaker

        //Init
        public Speaker(string filepath, int outputDeviceID, float volume, bool deleteFile, bool newThread)
        {
            this.filepath = filepath;
            this.deleteFile = deleteFile;
            this.volume = volume;
            this.outputDeviceID = outputDeviceID;

            if (newThread) //Play sound in a new thread to don't block the bot
            {
                var th = new Thread(PlaySound);
                th.Start();
            }
            else PlaySound();
        }

        public void Dispose()
        {
            //I need this to dispose stuff and don't get errors
        }

        //Create a disposable Sound Player
        void PlaySound()
        {
            using (WaveOutEvent tempWave = new WaveOutEvent())
            {
                tempWave.DeviceNumber = outputDeviceID; //Set the Output Device
                WaveFileReader reader = null;
                reader = new WaveFileReader(filepath);
                WaveChannel32 inputStream = new WaveChannel32(reader);
                inputStream.PadWithZeroes = false;
                tempWave.Init(reader);
                tempWave.Volume = volume;
                tempWave.Play();
                
                while (tempWave.PlaybackState != PlaybackState.Stopped)
                {
                    //Wait and continue when finished
                    if (stop) tempWave.Stop();
                }

                AudioMixer.RemoveSpeaker(this); //Remove Speaker from lists
                reader.Dispose(); //Dispose reader
                tempWave.Dispose(); //Dispose wave

                if(deleteFile) //Delete file if deleteFile is true
                {
                    File.Delete(filepath); //Delete used and created file
                }
            }
        }
    }
}
