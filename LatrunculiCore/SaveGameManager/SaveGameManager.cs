using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using LatrunculiCore.Desk;

namespace LatrunculiCore.SaveGame
{
    public class SaveGameManager
    {
        public string SaveFolder => $@"{app.AppDataFolder}\SavedGames";
        public LatrunculiApp app;

        public SaveGameManager(LatrunculiApp app)
        {
            this.app = app;
        }

        public bool Save(string gameName)
        {
            return SaveToFile(getDefaultSaveFilePath(gameName));
        }

        public bool Load(string gameName)
        {
            return LoadFromFile(getDefaultSaveFilePath(gameName));
        }

        public bool SaveToFile(string fileName)
        {
            var json = JsonSerializer.Serialize(app.HistoryManager.Steps);
            prepareSaveFolder(fileName);
            File.WriteAllText(fileName, json);
            return true;
        }

        public bool LoadFromFile(string fileName)
        {
            var json = File.ReadAllText(fileName);
            app.HistoryManager.LoadHistory(JsonSerializer.Deserialize<List<ChangeSet>>(json));
            return true;
        }

        public IEnumerable<string> GetSavedGamesList()
        {
            if (!Directory.Exists(SaveFolder))
            {
                return new string[0];
            }
            return Directory.GetFiles(SaveFolder)
                .Where(file => Path.GetExtension(file)?.ToLower() == ".json")
                .Select(Path.GetFileNameWithoutExtension);
        }

        private string prepareSaveFolder(string fileName)
        {
            var directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }

        private string getDefaultSaveFilePath(string fileName)
        {
            return $@"{SaveFolder}\{fileName}.json";
        }
    }
}