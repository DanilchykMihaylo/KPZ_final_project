using Checkers.Models;

namespace Checkers.Services
{
    public interface ISettingsService
    {
        AppSettings Current { get; }
        void Save(AppSettings settings);
        AppSettings Load();
    }
}