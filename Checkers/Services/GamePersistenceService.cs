using System.IO;
using System.Text.Json;
using Checkers.Models;

namespace Checkers.Services
{
    public class GamePersistenceService : IGamePersistenceService
    {
        private static readonly string SavePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Checkers",
            "save.json");

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public void Save(GameRecord record)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath)!);
            var json = JsonSerializer.Serialize(record, JsonOptions);
            File.WriteAllText(SavePath, json);
        }

        public GameRecord? Load()
        {
            if (!File.Exists(SavePath))
                return null;

            var json = File.ReadAllText(SavePath);
            return JsonSerializer.Deserialize<GameRecord>(json, JsonOptions);
        }

        public bool SaveExists() => File.Exists(SavePath);
    }
}