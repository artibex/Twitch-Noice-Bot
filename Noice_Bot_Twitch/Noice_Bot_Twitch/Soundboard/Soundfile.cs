using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noice_Bot_Twitch.Soundboard
{
    public class Soundfile
    {
        public int ID;
        public string SoundfileName;
        public string PathInSoundfolder;
        public string FullPath;
    }

    public class SoundDirectory
    {
        public List<Soundfile> Soundfiles;
        public string DirectoryName;

        public SoundDirectory()
        {
            Soundfiles = new List<Soundfile>();
        }
    }

    public class RootSoundDirectory
    {
        public List<Soundfile> Soundfiles;
        public List<SoundDirectory> SoundDirectories;

        public RootSoundDirectory()
        {
            Soundfiles = new List<Soundfile>();
            SoundDirectories = new List<SoundDirectory>();
        }
    }
}
