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
        private string saveFolder => $@"{app.AppDataFolder}\SavedGames";
        public LatrunculiApp app;

        public SaveGameManager(LatrunculiApp app)
        {
            this.app = app;
        }

        public bool SaveToFile(string fileName)
        {
            var json = JsonSerializer.Serialize(app.HistoryManager.Steps);
            File.WriteAllText(prepareSaveFilePath(fileName), json);
            return true;
        }

        public bool LoadFromFile(string fileName)
        {
            var json = File.ReadAllText(prepareSaveFilePath(fileName));
            app.HistoryManager.LoadHistory(JsonSerializer.Deserialize<List<ChangeSet>>(json));
            return true;
        }

        public IEnumerable<string> GetSavedGamesList()
        {
            if (!Directory.Exists(saveFolder))
            {
                return new string[0];
            }
            return Directory.GetFiles(saveFolder)
                .Where(file => Path.GetExtension(file)?.ToLower() == ".json")
                .Select(Path.GetFileNameWithoutExtension);
        }

        private string prepareSaveFilePath(string fileName)
        {
            return $@"{prepareSaveFolder()}\{fileName}.json";
        }

        private string prepareSaveFolder()
        {
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }
            return saveFolder;
        }
    }
}