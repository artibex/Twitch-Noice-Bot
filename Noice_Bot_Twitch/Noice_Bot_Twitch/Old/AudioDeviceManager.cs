using System;
using NAudio.Wave;

namespace Noice_Bot_Twitch
{
    //Manages all audio device ID's for the bot
    public static class AudioDeviceManager
    {
        private static int _ttsOutputDeviceID = -2; //Text to Speech
        public static int ttsOutputDeviceID
        {
            get { return _ttsOutputDeviceID; }
            set { if (value > -2) _ttsOutputDeviceID = value; }
        }

        private static int _soundboardOutputDeviceID = -2; //Soundboard
        public static int soundboardOutputDeviceID
        {
            get { return _soundboardOutputDeviceID; }
            set { if (value > -2) _soundboardOutputDeviceID = value; }
        }
        private static int _notificationOutputDeviceID = -2; //Notification
        public static int notificationOutputDeviceID
        {
            get { return _notificationOutputDeviceID; }
            set { if (value > -2) _notificationOutputDeviceID = value; }
        }

        //FileManager fm; //File Manager

        //public static AudioDeviceManager(FileManager fm)
        //{
        //    this.fm = fm;
        //    LoadSettings(); //Load settings from file manager, if there is anything set
        //    DetectDevices(); //Init Device Detection, if you can find them in the settings, use this one
        //}

        public static void LoadSettings()
        {
            //First, check the File manager for a setting, otherwise ask user to select one
            _ttsOutputDeviceID = FileManagerOld.GetTTSOutputDeviceID();
            _soundboardOutputDeviceID = FileManagerOld.GetSoundboardOutputDeviceID();
            _notificationOutputDeviceID = FileManagerOld.GetNotificationOutputDeviceID();
            DetectDevices();
        }

        //Let the user Select a audio device
        public static void DetectDevices()
        {
            if (_ttsOutputDeviceID == -2) //If no device is given via config, let the user determen one, -2 is NEVER a output device
            {
                //Count the available devices and display them
                int deviceCount = -2;
                for (int n = -1; n < WaveOut.DeviceCount; n++)
                {
                    var caps = WaveOut.GetCapabilities(n);
                    Console.WriteLine($"{n}: {caps.ProductName}");
                    deviceCount++;
                }
                SetTTSDevice(deviceCount); //Set the TTS Device ID, skip this step if it's allready set by Settings.txt
                SetSoundboardDevice(deviceCount); //Set the Soundboard Device ID, skip this step if it's allready set by Settings.txt
                SetNotificationDevice(deviceCount); //Set the Notification Device ID, skip this step if it's allready set by Settings.txt        
            }
        }
        static void SetTTSDevice(int deviceCount)
        {
            if (_ttsOutputDeviceID == -2) Console.WriteLine("Please select the TEXT TO SPEECH output device (number)");
            else Console.WriteLine("TTS output device ID = " + _ttsOutputDeviceID);

            //Check the given numeric input, if it's not usable, try again
            while (_ttsOutputDeviceID == -2)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out _ttsOutputDeviceID))
                {
                    if (_ttsOutputDeviceID > deviceCount || _ttsOutputDeviceID < -1)
                    {
                        _ttsOutputDeviceID = -2;
                        Console.WriteLine("Wrong Input, please select a audiodevice (-1-" + deviceCount);
                    }
                    else Console.WriteLine("You selected " + _ttsOutputDeviceID);
                }
                else Console.WriteLine("Wrong Input, please select a audiodevice (-1-" + deviceCount);
            }
        }

        static void SetSoundboardDevice(int deviceCount)
        {
            if (_soundboardOutputDeviceID == -2) Console.WriteLine("Please select the SOUNDBOARD output device (number)");
            else Console.WriteLine("Soundboard output device ID = " + _soundboardOutputDeviceID);

            //Check the given numeric input, if it's not usable, try again
            while (_soundboardOutputDeviceID == -2)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out _soundboardOutputDeviceID))
                {
                    if (_soundboardOutputDeviceID > deviceCount || _soundboardOutputDeviceID < -1)
                    {
                        _soundboardOutputDeviceID = -2;
                        Console.WriteLine("Wrong Input, please select a audiodevice (-1-" + deviceCount);
                    }
                    else Console.WriteLine("You selected " + _soundboardOutputDeviceID);
                }
                else Console.WriteLine("Wrong Input, please select a audiodevice (-1-" + deviceCount);
            }
        }

        static void SetNotificationDevice(int deviceCount)
        {
            if (_notificationOutputDeviceID == -2) Console.WriteLine("Please select the NOTIFICATION output device (number)");
            else Console.WriteLine("Notification output device ID = " + _notificationOutputDeviceID);

            //Check the given numeric input, if it's not usable, try again
            while (_notificationOutputDeviceID == -2)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out _notificationOutputDeviceID))
                {
                    if (_notificationOutputDeviceID > deviceCount || _notificationOutputDeviceID < -1)
                    {
                        _notificationOutputDeviceID = -2;
                        Console.WriteLine("Wrong Input, please select a audiodevice (-1-" + deviceCount);
                    }
                    else Console.WriteLine("You selected " + _notificationOutputDeviceID);
                }
                else Console.WriteLine("Wrong Input, please select a audiodevice (-1-" + deviceCount);
            }
        }

        //Get Tex to Speech device ID
        public static int GetTTSOutputDeviceID()
        {
            return _ttsOutputDeviceID;
        }
        //Get Soundbaord device ID
        public static int GetSoundboardOutputDeviceID()
        {
            return _soundboardOutputDeviceID;
        }
        //Get Notification device ID
        public static int GetNotificationOutputDeviceID()
        {
            return _notificationOutputDeviceID;
        }
    }
}
