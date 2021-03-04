using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Noice_Bot_Twitch
{
    public class TwitchJsonBuilder
    {
        private string _auth_token;
        private string[] _topics;

        private int _nonce = 0;

        public TwitchJsonBuilder(string[] topics, string auth_token)
        {
            _topics = topics;
            _auth_token = auth_token;
        }

        public string GetRequest()
        {
            int non = _nonce;
            _nonce++;
            TwitchRequest tr = new TwitchRequest(_topics, _auth_token, non.ToString());
            string result = JsonConvert.SerializeObject(tr);
            return result;
        }
    }

    class TwitchRequest
    {
        public string type = "LISTEN";
        public string nonce = "";
        public Data data;

        public TwitchRequest(string[] topics, string auth_token, string nonce)
        {
            this.nonce = nonce;
            data = new Data(topics, auth_token);
        }
    }

    class Data
    {
        public string[] topics;
        public string auth_token;

        public Data(string[] topics, string auth_token)
        {
            this.auth_token = auth_token;
            this.topics = topics;
        }
    }
}
