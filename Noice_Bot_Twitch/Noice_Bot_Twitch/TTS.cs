using System;
using System.Speech.Synthesis;

namespace Noice_Bot_Twitch
{
    //Give this Text To Speech module a text and it will read it
    public static class TTS
    {
        //public static FileManager fm;
        //public static AudioDeviceManager adm;
        private static int _normalSpeed = 0;
        public static int normalSpeed
        {
            get { return _normalSpeed; }
            set { if (value >= 1 && value <= 10) _normalSpeed = value; CalcMidSpeed(); }
        }
        private static int _fasterSpeed = 3;
        private static int _fastestSpeed = 5;
        public static int fastestSpeed
        {
            get { return _fastestSpeed; }
            set { if (value >= 1 && value <= 10) _fastestSpeed = value; CalcMidSpeed(); }
        }
        private static int _outputDeviceID = -2;
        public static int outputDeviceID
        {
            get { return _outputDeviceID; }
            set { if (value > -1) _outputDeviceID = value; }
        }
        private static float _ttsVolume = 1f;
        public static float ttsVolume
        {
            get { return _ttsVolume; }
            set { if(value > 0 && value <= 1) _ttsVolume = value; }
        }
        public static bool useTTS = false;

        //Init TextToSpeech, needs a filemanager to know where to place the soundfile
        //public TTS (FileManager fm, AudioDeviceManager adm)
        //{
        //    this.fm = fm; //Set the Filemanager
        //    this.adm = adm;
        //    LoadSettings();
        //}

        public static void LoadSettings()
        {
            _outputDeviceID = AudioDeviceManager.GetTTSOutputDeviceID(); //Get the configured device
            _ttsVolume = FileManager.GetTTSVolume(); //Get the volume
            _normalSpeed = FileManager.GetTTSBaseSpeed(); //Get the normal speed
            _fastestSpeed = FileManager.GetTTSMaxSpeed(); //Get the max speed
            CalcMidSpeed();
            useTTS = FileManager.GetUseTTS();
        }

        static void CalcMidSpeed()
        {
            //fasterSpeed is in the middle of base and max, so calculate it
            _fasterSpeed = Convert.ToInt32((_normalSpeed + _fastestSpeed) / 2);
        }

        //Method to speak any given text, create a .wav file and play it. Then delete it
        public static void Speak(string text)
        {
            if (!useTTS) return;
            int speed = 0; //Defined PlaybackSpeed
            string filepath = String.Empty; //Filepath

            if (text.Length > Convert.ToInt32(FileManager.GetMaxTextLength()*0.5)) speed = _fasterSpeed; //50% of 100 letters = 50
            if (text.Length > Convert.ToInt32(FileManager.GetMaxTextLength()*0.8)) speed = _fastestSpeed; //80% of 100 letters = 80
            else speed = _normalSpeed; //If the text is small enough, use normal speed

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

            filepath = FileManager.GetPath() + @"\" + filename; //Get the path to save to

            SpeechSynthesizer tempSynth = new SpeechSynthesizer(); //Create a new synth
            tempSynth.SetOutputToWaveFile(filepath); //Set the output path
            tempSynth.Rate = speed; //Create with that speed
            tempSynth.Speak(text); //Generate the .wav file
            tempSynth.Dispose(); //Get rid of the Synth

            Speaker s = new Speaker(filepath, _outputDeviceID, _ttsVolume, true, true);
            AudioMixer.AddTTSSpeaker(s);
        }
    }
}
