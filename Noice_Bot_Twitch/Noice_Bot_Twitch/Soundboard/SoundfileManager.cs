using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noice_Bot_Twitch.Soundboard
{
    public class SoundfileManager
    {
        private RootSoundDirectory _rootSoundDirectory;
        private Dictionary<int, string> _soundfileIDs;
        private Random _random;

        private string _directoryPath;
        private string _directoryName;

        private int _idCount;
        private bool _needNewSoundfileIDs = false;
        private bool _saveNewSoundfileIDs = false;

        public SoundfileManager(string soundDirectoryPath, Dictionary<int, string> soundfileIDs)
        {
            _idCount = 0;
            _directoryPath = soundDirectoryPath;
            _directoryName = new DirectoryInfo(_directoryPath).Name;

            if (soundfileIDs == null)
            {
                _needNewSoundfileIDs = true;
                soundfileIDs = new Dictionary<int, string>();
            }
            else
            {
                CheckIfDeleted(soundfileIDs);
            }
            
            _rootSoundDirectory = GetFiles(soundfileIDs);

            if (_needNewSoundfileIDs || _saveNewSoundfileIDs)
            {
                FileManager.SaveSoundfileIDs(soundfileIDs);
            }

            _soundfileIDs = soundfileIDs;
            _random = new Random();
        }

        private RootSoundDirectory GetFiles(Dictionary<int, string> soundfileIDs)
        {
            RootSoundDirectory rootSoundDirectory = new RootSoundDirectory();

            Directory.CreateDirectory(_directoryPath);
            string[] files = Directory.GetFiles(_directoryPath);

            //sucht alle Dateien im Root Folder
            foreach(string file in files)
            {
                if (Path.GetExtension(file) != ".mp3") continue;

                Soundfile soundfile = new Soundfile();
                soundfile.FullPath = file;
                soundfile.SoundfileName = Path.GetFileNameWithoutExtension(file);
                soundfile.PathInSoundfolder = Path.GetFileName(file);
                soundfile.ID = GetSoundfileID(soundfile.PathInSoundfolder, soundfileIDs);

                rootSoundDirectory.Soundfiles.Add(soundfile);
            }

            //Alle Unterordner suchen
            string[] subfolders = Directory.GetDirectories(_directoryPath);

            foreach(string subdir in subfolders)
            {
                SoundDirectory soundDirectory = new SoundDirectory();

                string dirName = new DirectoryInfo(subdir).Name;
                string[] subfiles = Directory.GetFiles(subdir);

                foreach(string subfile in subfiles)
                {
                    if (Path.GetExtension(subfile) != ".mp3") continue;

                    Soundfile soundfile = new Soundfile();
                    soundfile.FullPath = subfile;
                    soundfile.SoundfileName = Path.GetFileNameWithoutExtension(subfile);
                    soundfile.PathInSoundfolder = dirName + Path.DirectorySeparatorChar + Path.GetFileName(subfile);
                    soundfile.ID = GetSoundfileID(soundfile.PathInSoundfolder, soundfileIDs);

                    soundDirectory.Soundfiles.Add(soundfile);
                }

                rootSoundDirectory.SoundDirectories.Add(soundDirectory);
            }

            return rootSoundDirectory;
        }

        public string GetSoundfileById(int fileID)
        {
            return _soundfileIDs[fileID];
        }

        public string GetRandomSoundfileInDirectory(int dirname)
        {
            var linq = from soundfile in _rootSoundDirectory.SoundDirectories where soundfile.DirectoryName.Equals(dirname) select soundfile.Soundfiles;
            var list = linq.First();
            return list[_random.Next(0, list.Count - 1)].FullPath;
        }

        private int GetSoundfileID(string pathInSoundfolder, Dictionary<int, string> soundfileIDs)
        {
            if (_needNewSoundfileIDs)
            {
                int fileID = _idCount++;
                soundfileIDs.Add(fileID, pathInSoundfolder);
                return fileID;
            }
            else
            {
                 var linq = from name in soundfileIDs where pathInSoundfolder.Equals(name.Value) select name.Key;
                if(linq.Count() == 0)
                {
                    _saveNewSoundfileIDs = true;                //Am Ende muss die JSON Datei neu gespeichert werden
                    List<int> usedIDs = soundfileIDs.Keys.ToList();
                    //Datei nicht hinterlegt, neue ID generieren
                    while (true)
                    {
                        if (usedIDs.Contains(_idCount))
                        {
                            _idCount++;
                        }
                        else
                        {
                            soundfileIDs.Add(_idCount, pathInSoundfolder);
                            Console.WriteLine("New File");
                            return _idCount;
                        }
                    }
                }
                else
                {
                    return linq.First();
                }
            }
        }

        private void CheckIfDeleted(Dictionary<int, string> soundfileIDs)
        {
            List<int> checkedIDs = soundfileIDs.Keys.ToList();      //Falls hier später noch IDs sind, sind die Musikdateien nicht mehr vorhanden, also löschen aus Dictonary
            string[] files = Directory.GetFiles(_directoryPath, "*.mp3", SearchOption.AllDirectories);

            foreach(string file in files)
            {
                string pathInSubdir;
                string parentDirname = new DirectoryInfo(file).Parent.Name;

                if (parentDirname.Equals(_directoryName))     //Ordner ist im root des Soundfolders
                {
                    pathInSubdir = Path.GetFileName(file);
                }
                else                            //Befindet sich in einem Unterordner
                {
                    pathInSubdir = parentDirname + Path.DirectorySeparatorChar + Path.GetFileName(file);
                }

                var linq = from name in soundfileIDs where pathInSubdir.Equals(name.Value) select name.Key;

                if (linq.Count() == 0) continue;    //Falls eine neue Datei hinzugekommen ist, die noch nicht in der Liste ist
                checkedIDs.Remove(linq.First());
            }

            //Ist jetzt noch eine ID in der Liste vorhanden, so gibt es diese Sounddatei dazu nicht mehr, also muss diese auch aus dem Dictonary gelöscht werden
            foreach(int deletedID in checkedIDs)
            {
                soundfileIDs.Remove(deletedID);
                Console.WriteLine("Löschen");
                _saveNewSoundfileIDs = true;
            }
        }
    }
}
