using System;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using NAudio.Wave;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Noice_Bot_Twitch
{
    class TTS
    {
        private FileManager fm;
        private AudioDeviceManager adm;
        private int normalSpeed = 0;
        private int fasterSpeed = 3;
        private int fastestSpeed = 5;
        private int outputDeviceID = -2;


        //Init TextToSpeech, needs a filemanager to know where to place the soundfile
        public TTS (FileManager fm, AudioDeviceManager adm)
        {
            this.fm = fm; //Set the Filemanager
            outputDeviceID = adm.GetTTSOutputDeviceID(); //Get the configured device
            
            //DetectDevices(); //Detect and Select the Audiodevice to play the sound
            
            normalSpeed = fm.GetTTSBaseSpeed();
            fastestSpeed = fm.GetTTSMaxSpeed();
            //fasterSpeed is in the middle of base and max, so calculate it
            fasterSpeed = Convert.ToInt32((normalSpeed + fastestSpeed) / 2);
        }

        public void Speak(string text)
        {
            int speed = 0; //Defined PlaybackSpeed
            string filepath = String.Empty; //Filepath

            if (text.Length > 80) speed = fasterSpeed; //Set the speed according to the length
            if (text.Length > 100) speed = fastestSpeed;
            else speed = normalSpeed;

            //Create a hash with the current date and format out useless characters
            string filename = text.GetHashCode().ToString() + DateTime.Now; //Generate filename
            filename = filename.Replace(".", String.Empty);
            filename = filename.Replace(":", String.Empty);
            filename = filename.Replace(" ", String.Empty);
            filename = filename + ".wav";

            filepath = fm.GetPath() + @"\" + filename;

            using (WaveOutEvent tempWave = new WaveOutEvent())
            {
                SpeechSynthesizer tempSynth = new SpeechSynthesizer();
                WaveFileReader reader = null;

                tempWave.DeviceNumber = outputDeviceID; //Set the Playing Device
                tempWave.DeviceNumber = outputDeviceID;
                tempSynth.Rate = speed;

                tempSynth.SetOutputToWaveFile(filepath); //Set the output path
                tempSynth.Speak(text); //Generate the .wav file
                tempSynth.Dispose(); //Get rid of the Synth
                reader = new WaveFileReader(filepath);
                WaveChannel32 inputStream = new WaveChannel32(reader);
                inputStream.PadWithZeroes = false;
                tempWave.Init(reader);
                tempWave.Play();

                while (tempWave.PlaybackState != PlaybackState.Stopped)
                {
                    //Wait and continue when finished
                }

                reader.Dispose(); //Dispose reader
                tempWave.Dispose(); //Dispose wave
                //Thread.Sleep(500);
                File.Delete(filepath); //Delete used and created file
            }
        }

        //Detect the connected audio devices
        void DetectDevices()
        {
            if(outputDeviceID == -2) //If no device is given via config, let the user determen one
            {
                //Count the available devices
                int deviceCount = -2;
                for (int n = -1; n < WaveOut.DeviceCount; n++)
                {
                    var caps = WaveOut.GetCapabilities(n);
                    Console.WriteLine($"{n}: {caps.ProductName}");
                    deviceCount++;
                }
                SetDevice(deviceCount);
            }
        }
        //Let the user Select a audio device
        void SetDevice(int deviceCount)
        {
            //Check the given numeric input, if it's not usable, try again
            Console.WriteLine("Please select the Text to Speech output device (number)");
            while (outputDeviceID == -2)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out outputDeviceID))
                {
                    if(outputDeviceID > deviceCount || outputDeviceID < -1)
                    {
                        outputDeviceID = -2;
                        Console.WriteLine("Wrong Input, please select a audiodevice (0-" + deviceCount);
                    } else Console.WriteLine("You selected " + outputDeviceID);
                }
                else Console.WriteLine("Wrong Input, please select a audiodevice (0-" + deviceCount);
            }
        }
    }
}
