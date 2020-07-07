using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis.TtsEngine;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio;

namespace Noice_Bot_Twitch
{
    class MainLogic
    {
        public MainLogic()
        {
            Init();
        }

        void Init()
        {
            FileManager fm; //Manages all Files 
            fm = new FileManager(); //Init FileManager to get all infos feed into the programm
            IrcClient client; //Create new Connection to a IRC Server
            Pinger pinger; //Ping the server every 5 Minutes so the connection is not getting closed
            AudioDeviceManager adm; //Manages the Output Devices (if non is configured in the settings.txt ask for ID's)
            NotificationSoundManager nsm; //Manages the Notification Sound, Playing, Volume etc.
            TTS tts; //Text to Speech Synthesizer to read out the chat
            CommandProcessor cp; //Processes the command (spam protector for example)

            Console.OutputEncoding = System.Text.Encoding.UTF8; //Changes the Console Encoding to UTF8 (Might be usefull, might be not)

            //Check the needed settings to create a connection, exit if something is wrong
            if (fm.GetIrcClient() == null || fm.GetPort() == 0 || fm.GetBotName() == null || fm.GetOAuth() == null || fm.GetChannelName() == null)
            {
                Console.WriteLine("An error occured while checking your Settings, please check your Settings.txt");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                Application.Exit();
                return;
            }

            //Creat a new IRC Client with the Information ouf of the Settings.txt file
            client = new IrcClient(fm.GetIrcClient(), fm.GetPort(), fm.GetBotName(), fm.GetOAuth(), fm.GetChannelName());
            pinger = new Pinger(client); //Create a Pinger that pings the server every 5 minutes to prevent this connection getting closed
            pinger.Start(); //Start the Pinger

            adm = new AudioDeviceManager(fm); //Manages the Audio ID's
            nsm = new NotificationSoundManager(fm, adm); //manages the Notification sounds

            tts = new TTS(fm, adm); //Create a new Text to Speech Synth
            cp = new CommandProcessor(fm); //Create a new Command Processor

            Console.WriteLine(fm.GetBotName() + " is ready!");
            while (true)
            {
                var rawMsg = client.ReadMessage(); //The whole Message from the IRC Server
                Comment c = new Comment(rawMsg); //Create a new Comment out of it. Cut out the Username ans the Message
                string executionOrder = fm.GetNotificationExecutionOrder();

                //If the Comment is not empty or just spaces execute a notification
                if (!String.IsNullOrWhiteSpace(c.user) && !String.IsNullOrWhiteSpace(c.comment) && !cp.CheckBlacklist(c))
                {
                    string ttsText = ""; //The text how it should be converted to speech

                    //Command Processing Area//
                    c = cp.CheckAlias(c); //Check for given alias
                    c.user = cp.RemoveNumeric(c.user); //Remove numbers in names
                    c = cp.SpamProtection(c); //Cut down the commant
                    //End Command Processing Area//

                    foreach (char exe in executionOrder)
                    {
                        if (exe.ToString() == "u") //Username
                        {
                            ttsText += " " + c.user;
                        }
                        if (exe.ToString() == "c") //Commant
                        {
                            ttsText += " " + c.comment;
                        }
                        if (exe.ToString() == "b") //Bridgeword
                        {
                            ttsText += " " + fm.GetRandomBridgeWord();
                        }
                        if (exe.ToString() == "n") //Notification Sound
                        {
                            nsm.Play();
                            //Play Random Notification Sound
                        }
                    }

                    //Cut out the first space if there is one (there should be allways one)
                    if(ttsText.IndexOf(" ") == 0) ttsText = ttsText.Substring(1, ttsText.Length - 1);
                    if (ttsText == "") //If string is empty, at least write the normal commant in the console
                    {
                        Console.WriteLine(c.user + ": " + c.comment);
                    } else
                    {
                        Console.WriteLine(ttsText);
                        tts.Speak(ttsText);
                    }


                }
            }
        }
    }
}
