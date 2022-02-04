using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noice_Bot_Twitch
{
    public class AudioFileDirectory
    {
        public string name;
        public List<AudioFile> audioFiles;

        public AudioFileDirectory (string name)
        {
            this.name = name;
            audioFiles = new List<AudioFile>();
        }

        public void Print()
        {
            Console.WriteLine(name);
            foreach (AudioFile a in audioFiles) a.Print();
        }
    }
}
