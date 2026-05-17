using System.Windows.Input;
using Checkers.Models;
using Checkers.Models.Enums;
using Checkers.Services;

namespace Checkers.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private int _boardSize;
        private BoardTheme _selectedTheme;

        public List<int> AvailableSizes { get; } = [6, 8, 10];

        public List<BoardTheme> AvailableThemes { get; } =
            Enum.GetValues<BoardTheme>().ToList();

        public int BoardSize
        {
            get => _boardSize;
            set => SetField(ref _boardSize, value);
        }

        public BoardTheme SelectedTheme
        {
            get => _selectedTheme;
            set => SetField(ref _selectedTheme, value);
        }

        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<AppSettings>? SettingsApplied;
        public event Action? Cancelled;

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            var current = _settingsService.Current;
            _boardSize = current.BoardSize;
            _selectedTheme = current.Theme;

            ApplyCommand = new RelayCommand(OnApply);
            CancelCommand = new RelayCommand(() => Cancelled?.Invoke());
        }

        private void OnApply()
        {
            var settings = new AppSettings
            {
                BoardSize = BoardSize,
                Theme = SelectedTheme
            };

            _settingsService.Save(settings);
            SettingsApplied?.Invoke(settings);
            Cancelled?.Invoke();
        }
    }
}