using System;
using System.Speech.Synthesis;

namespace Noice_Bot_Twitch
{
    //Give this Text To Speech module a text and it will read it
    class TTS
    {
        private FileManager fm;
        private AudioDeviceManager adm;
        private int normalSpeed = 0;
        private int fasterSpeed = 3;
        private int fastestSpeed = 5;
        private int outputDeviceID = -2;
        private float ttsVolume = 1f;

        //Init TextToSpeech, needs a filemanager to know where to place the soundfile
        public TTS (FileManager fm, AudioDeviceManager adm)
        {
            this.fm = fm; //Set the Filemanager
            this.adm = adm;
            LoadSettings();
        }

        public void LoadSettings()
        {
            outputDeviceID = adm.GetTTSOutputDeviceID(); //Get the configured device
            ttsVolume = fm.GetTTSVolume(); //Get the volume
            normalSpeed = fm.GetTTSBaseSpeed(); //Get the normal speed
            fastestSpeed = fm.GetTTSMaxSpeed(); //Get the max speed
            //fasterSpeed is in the middle of base and max, so calculate it
            fasterSpeed = Convert.ToInt32((normalSpeed + fastestSpeed) / 2);
        }

        //Method to speak any given text, create a .wav file and play it. Then delete it
        public void Speak(string text)
        {
            int speed = 0; //Defined PlaybackSpeed
            string filepath = String.Empty; //Filepath

            if (text.Length > Convert.ToInt32(fm.GetMaxTextLength()*0.5)) speed = fasterSpeed; //50% of 100 letters = 50
            if (text.Length > Convert.ToInt32(fm.GetMaxTextLength()*0.8)) speed = fastestSpeed; //80% of 100 letters = 80
            else speed = normalSpeed; //If the text is small enough, use normal speed

            //Create a hash with the current date and format out useless characters
            string filename = text.GetHashCode().ToString() + DateTime.Now; //Generate filename
            filename = filename.Replace(".", String.Empty); //Kick some stuff out that would lead to file errors
            filename = filename.Replace(":", String.Empty);
            filename = filename.Replace(")", String.Empty);
            filename = filename.Replace("(", String.Empty);
            filename = filename.Replace("%", String.Empty);
            filename = filename.Replace("!", String.Empty);
            filename = filename.Replace("=", String.Empty);
            filename = filename.Replace("§", String.Empty);
            filename = filename.Replace("$", String.Empty);
            filename = filename.Replace("&", String.Empty);
            filename = filename.Replace("/", String.Empty);
            filename = filename.Replace(" ", String.Empty);
            filename = filename + ".wav";

            filepath = fm.GetPath() + @"\" + filename; //Get the path to save to

            SpeechSynthesizer tempSynth = new SpeechSynthesizer(); //Create a new synth
            tempSynth.SetOutputToWaveFile(filepath); //Set the output path
            tempSynth.Rate = speed; //Create with that speed
            tempSynth.Speak(text); //Generate the .wav file
            tempSynth.Dispose(); //Get rid of the Synth

            using (Speaker s = new Speaker(filepath, outputDeviceID,ttsVolume, true, false)) ; //Do nothing, exept playing it
        }
    }
}
