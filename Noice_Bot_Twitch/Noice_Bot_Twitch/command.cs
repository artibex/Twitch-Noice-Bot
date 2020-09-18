using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noice_Bot_Twitch
{
    public class Command
    {
        public string functionName; //Generel Name of the Function
        public string name; //Name to trigger the command
        public string helpComment; //Help info

        public Command(string functionName, string name, string help)
        {
            this.functionName = functionName;
            this.name = name;
            this.helpComment = help;
        }
    }
}
