using System;
using System.Threading;
using System.Windows.Forms;

namespace Noice_Bot_Twitch
{
    //Main instance of everything. The answer to everything. *insert 42 joke*
    class MainLogicOld
    {
        //FileManager fm; //Manages all Files 
        TwitchJsonBuilder tjb;
        PubSubService pubSub;
        IrcClient client; //Create new Connection to a IRC Server
        Pinger pinger; //Ping the server every 5 Minutes so the connection is not getting closed
        //AudioDeviceManager adm; //Manages the Output Devices (if non is configured in the settings.txt ask for ID's)
        //NotificationSoundManager nsm; //Manages the Notification Sound, Playing, Volume etc.
        //TTS tts; //Text to Speech Synthesizer to read out the chat
        //CommantProcessor cp; //Processes the command (spam protector for example)
        //SoundboardManager sm; //Manages all sounds + cooldowns of users
        //CommandIdentifier ci; //Checks the user commant for a command to execute
        //ExecutionOrderManager eom; //Manages TTS Execution

        //Get ready for startup. 3... 2... 1...
        public MainLogicOld()
        {
            Init();
        }

        void Init()
        {
            //fm = new FileManager(); //Init FileManager to get all infos feed into the programm

            Console.OutputEncoding = System.Text.Encoding.UTF8; //Changes the Console Encoding to UTF8 (Might be usefull, might be not)
            FileManagerOld.LoadSettings(); //Setup File Manager

            client = new IrcClient(FileManagerOld.GetIrcClient(), FileManagerOld.GetPort(), FileManagerOld.GetBotName(), FileManagerOld.GetOAuth(), FileManagerOld.GetChannelName().ToLower());
            pinger = new Pinger(client); //Create a Pinger that pings the server every 5 minutes to prevent this connection getting closed
            tjb = new TwitchJsonBuilder(new string[] { "channel-points-channel-v1." + FileManagerOld.GetChannelID()}, FileManagerOld.GetAppAuth());
            pubSub = new PubSubService(tjb,client);

            //Load all Settings in (These functions can also be used to reload settings)
            AudioDeviceManager.LoadSettings();
            NotificationSoundManager.LoadSettings();
            TTS.LoadSettings();
            SoundboardManagerOld.LoadSettings(client);
            CommandIdentifier.LoadSettings(client, pubSub);
            CommentProcessor.LoadSettings();
            ExecutionOrderManager.LoadSettings();

            //Check the needed settings to create a connection, exit if something is wrong
            if (FileManagerOld.GetIrcClient() == null || FileManagerOld.GetPort() == 0 || FileManagerOld.GetBotName() == null || FileManagerOld.GetOAuth() == null || FileManagerOld.GetChannelName() == null)
            {
                Console.WriteLine("An error occured while checking your Settings, please check your Settings.txt");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                Application.Exit();
                return;
            }

            pinger.Start(); //Start the Pinger
            Console.WriteLine(FileManagerOld.GetBotName() + " is ready!");
            while (true) //Loop throu this, react when something in the chat happend
            {
                var rawMsg = client.ReadMessage(); //The whole Message from the IRC Server
                Comment c = new Comment(rawMsg); //Create a new Comment out of it. Cut out the Username ans the Message
                c = CommentProcessor.Process(c); //Edit the given User Comment
                string executionOrder = FileManagerOld.GetNotificationExecutionOrder(); //Check if to read it out and how
                string ttsText = ExecutionOrderManager.GetTTSText(c); //The text how it should be converted to speech

                
                if (ttsText == "" && !String.IsNullOrWhiteSpace(c.user) && !String.IsNullOrWhiteSpace(c.comment)) Console.WriteLine(c.user + ": " + c.comment); //If comment is empty, write normal comment in console
                else if(ttsText != "" && !String.IsNullOrWhiteSpace(c.user) && !String.IsNullOrWhiteSpace(c.comment))
                {
                    Console.WriteLine(ttsText);
                    TTS.Speak(ttsText);
                }

                //If the Comment is not empty or just spaces execute a notification
                //if (!String.IsNullOrWhiteSpace(c.user) && !String.IsNullOrWhiteSpace(c.comment) && !cp.CheckBlacklist(c) && executionOrder != "")
                //{
                //    //foreach (char exe in executionOrder)
                //    //{
                //    //    if (exe.ToString() == "u") //Username
                //    //    {
                //    //        ttsText += " " + c.user;
                //    //    }
                //    //    if (exe.ToString() == "c") //Commant
                //    //    {
                //    //        ttsText += " " + c.comment;
                //    //    }
                //    //    if (exe.ToString() == "b") //Bridgeword
                //    //    {
                //    //        ttsText += " " + fm.GetRandomBridgeWord();
                //    //    }
                //    //    if (exe.ToString() == "n") //Notification Sound
                //    //    {
                //    //        nsm.Play();
                //    //        //Play Random Notification Sound
                //    //    }
                //    //}
                //    //Cut out the first space if there is one (there should be allways one)
                //    if (ttsText.IndexOf(" ") == 0) ttsText = ttsText.Substring(1, ttsText.Length - 1);
                //    if (ttsText == "") //If string is empty, at least write the normal commant in the console
                //    {
                //        Console.WriteLine(c.user + ": " + c.comment);
                //    }
                //    else
                //    {
                //        Console.WriteLine(ttsText);
                //        tts.Speak(ttsText);
                //    }
                //}
                //else if(c.user != "" && c.comment != "") Console.WriteLine(c.user + ": " + c.comment);
            }
        }
    }
}
