using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noice_Bot_Twitch
{
    public class AudioFile
    {
        public int ID;
        public string Name;
        public string Path;
        public float volumeOffset;

        public AudioFile(int ID, string Name, string Path)
        {
            this.ID = ID;
            this.Name = Name;
            this.Path = Path;
        }

        public void Print()
        {
            Console.WriteLine(ID + "," + Name + "," + Path + "," + volumeOffset);
        }
    }
}
