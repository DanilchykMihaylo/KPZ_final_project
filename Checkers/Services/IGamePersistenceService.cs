using Checkers.Models;

namespace Checkers.Services
{
    public interface IGamePersistenceService
    {
        void Save(GameRecord record);
        GameRecord? Load();
        bool SaveExists();
    }
}