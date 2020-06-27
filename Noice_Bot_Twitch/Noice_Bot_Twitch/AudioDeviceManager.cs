using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Noice_Bot_Twitch
{
    //Manages all audio device ID's for the bot
    class AudioDeviceManager
    {
        int _ttsOutputDeviceID = -2;
        int _soundboardOutputDeviceID = -2;
        FileManager fm;

        public AudioDeviceManager(FileManager fm)
        {
            this.fm = fm;
            _ttsOutputDeviceID = fm.GetTTSOutputDeviceID();
            _soundboardOutputDeviceID = fm.GetSoundboardOutputDeviceID();
            DetectDevices(); //Init Device Detection
        }

        public void DetectDevices()
        {
            if (_ttsOutputDeviceID == -2) //If no device is given via config, let the user determen one
            {
                //Count the available devices
                int deviceCount = -2;
                for (int n = -1; n < WaveOut.DeviceCount; n++)
                {
                    var caps = WaveOut.GetCapabilities(n);
                    Console.WriteLine($"{n}: {caps.ProductName}");
                    deviceCount++;
                }
                SetTTSDevice(deviceCount); //Set the TTS Device ID, skip this step if it's allready set by Settings.txt
                SetSoundboardDevice(deviceCount); //Set the Soundboard Device ID, skip this step if it's allready set by Settings.txt
            }
        }
        //Let the user Select a audio device
        void SetTTSDevice(int deviceCount)
        {
            //Check the given numeric input, if it's not usable, try again
            Console.WriteLine("Please select the Text to Speech output device (number)");
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

        void SetSoundboardDevice(int deviceCount)
        {
            //Check the given numeric input, if it's not usable, try again
            Console.WriteLine("Please select the Soundboard device (number)");
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


        public int GetTTSOutputDeviceID()
        {
            return _ttsOutputDeviceID;
        }
        public int GetSoundboardOutputDeviceID()
        {
            return _soundboardOutputDeviceID;
        }
    }
}
