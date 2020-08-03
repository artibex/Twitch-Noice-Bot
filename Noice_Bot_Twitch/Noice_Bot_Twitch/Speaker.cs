using System;
using NAudio.Wave;
using System.IO;
using System.Threading;


namespace Noice_Bot_Twitch
{
    class Speaker : IDisposable
    {
        string filepath;
        bool deleteFile;
        int outputDeviceID;
        float volume = 0.9f;

        public Speaker(string filepath, int outputDeviceID, float volume, bool deleteFile, bool newThread)
        {
            this.filepath = filepath;
            this.deleteFile = deleteFile;
            this.volume = volume;
            this.outputDeviceID = outputDeviceID;

            if (newThread)
            {
                var th = new Thread(PlaySound);
                th.Start();
            }
            else PlaySound();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

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
                }

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
