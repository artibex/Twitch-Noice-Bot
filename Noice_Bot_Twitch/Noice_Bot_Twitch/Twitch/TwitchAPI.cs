using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Noice_Bot_Twitch.Twitch
{
    class TwitchAPI
    {
        private const string client_ID = "gp762nuuoqcoxypju8c569th9wz7q5";  //Twitch Dev-App Client ID
        private string _oauth = "s4nt3m1kk5jqt28uric5opwgjufrhw";

        public TwitchAPI()
        {

        }

        public int GetUserID(string username)
        {
            int userID = -1;    //return -1 if error

            WebRequest request = WebRequest.Create($"https://api.twitch.tv/helix/search/channels?query='{username}'");
            request.Headers.Add("Authorization", $"Bearer {_oauth}");
            request.Headers.Add("Client-Id", client_ID);

            WebResponse response = null;

            try
            {
                response = request.GetResponse();
            }
            catch (WebException webE)
            {
                Console.WriteLine(webE.Message);
                return -1;
            }

            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);     // Open the stream using a StreamReader for easy access.
                string responseFromServer = reader.ReadToEnd();         // Read the content.
                dynamic json = JsonConvert.DeserializeObject(responseFromServer);

                Console.WriteLine($"Username: {username}, ID: {json.data[0].id}");
            }

            return userID;
        }
    }
}
