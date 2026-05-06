using Sudoku.Core.Models;
using Sudoku.Infrastructure.Repositories;
using Sudoku.Presentation.ViewModels.Base;

namespace Sudoku.Presentation.ViewModels;

/// <summary>
/// ViewModel for application settings.
/// Manages theme and language preferences with persistence to ProfileRepository.
/// </summary>
public class SettingsViewModel : ObservableObject
{
    private string _selectedTheme;
    private string _selectedLanguage;
    private readonly ProfileRepository _profileRepository;

    private const string LanguageCodeEnglish = "en-US";
    private const string LanguageCodeUkrainian = "uk-UA";

    /// <summary>
    /// Event raised when the theme is changed.
    /// </summary>
    public event EventHandler<string>? ThemeChanged;

    /// <summary>
    /// Event raised when the language is changed.
    /// </summary>
    public event EventHandler<string>? LanguageChanged;

    /// <summary>
    /// Initializes a new instance of the SettingsViewModel class.
    /// </summary>
    /// <param name="profileRepository">The repository for persisting profile settings.</param>
    /// <exception cref="ArgumentNullException">Thrown when profileRepository is null.</exception>
    public SettingsViewModel(ProfileRepository profileRepository)
    {
        _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));

        _selectedTheme = PlayerProfile.Instance.PreferredTheme;
        _selectedLanguage = MapLanguageCodeToDisplayName(PlayerProfile.Instance.PreferredLanguage);

        ChangeThemeCommand = new RelayCommand(param => OnThemeChanged(param?.ToString() ?? "Light"));
        ChangeLanguageCommand = new RelayCommand(param => OnLanguageChanged(param?.ToString() ?? "English"));
    }

    /// <summary>
    /// Gets or sets the currently selected theme ("Light", "Dark", "Solarized").
    /// </summary>
    public string SelectedTheme
    {
        get => _selectedTheme;
        set => SetProperty(ref _selectedTheme, value);
    }

    /// <summary>
    /// Gets or sets the currently selected language.
    /// </summary>
    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set => SetProperty(ref _selectedLanguage, value);
    }

    /// <summary>
    /// Gets the available themes.
    /// </summary>
    public IReadOnlyList<string> AvailableThemes { get; } = new[] { "Light", "Dark", "Solarized" };

    /// <summary>
    /// Gets the available languages.
    /// </summary>
    public IReadOnlyList<string> AvailableLanguages { get; } = new[] { "English", "Ukrainian" };

    /// <summary>
    /// Gets the command to change the theme.
    /// </summary>
    public RelayCommand ChangeThemeCommand { get; }

    /// <summary>
    /// Gets the command to change the language.
    /// </summary>
    public RelayCommand ChangeLanguageCommand { get; }

    /// <summary>
    /// Handles theme change and persists it to profile storage.
    /// </summary>
    /// <param name="theme">The new theme selection.</param>
    private void OnThemeChanged(string theme)
    {
        if (AvailableThemes.Contains(theme))
        {
            SelectedTheme = theme;
            PlayerProfile.Instance.PreferredTheme = theme;
            PlayerProfile.Instance.MarkAsModified();
            _ = _profileRepository.SaveProfileAsync(PlayerProfile.Instance);
            ThemeChanged?.Invoke(this, theme);
        }
    }

    /// <summary>
    /// Handles language change and persists it to profile storage.
    /// Triggers resource context update for localization.
    /// </summary>
    /// <param name="language">The new language selection.</param>
    private void OnLanguageChanged(string language)
    {
        if (AvailableLanguages.Contains(language))
        {
            SelectedLanguage = language;
            var languageCode = MapLanguageNameToCode(language);
            PlayerProfile.Instance.PreferredLanguage = languageCode;
            PlayerProfile.Instance.MarkAsModified();
            _ = _profileRepository.SaveProfileAsync(PlayerProfile.Instance);
            LanguageChanged?.Invoke(this, languageCode);
        }
    }

    /// <summary>
    /// Maps a language display name to its language code.
    /// </summary>
    /// <param name="languageName">The display name (e.g., "English", "Ukrainian").</param>
    /// <returns>The language code (e.g., "en-US", "uk-UA").</returns>
    private static string MapLanguageNameToCode(string languageName) => languageName switch
    {
        "English" => LanguageCodeEnglish,
        "Ukrainian" => LanguageCodeUkrainian,
        _ => LanguageCodeEnglish
    };

    /// <summary>
    /// Maps a language code to its display name.
    /// </summary>
    /// <param name="languageCode">The language code (e.g., "en-US", "uk-UA").</param>
    /// <returns>The display name (e.g., "English", "Ukrainian").</returns>
    private static string MapLanguageCodeToDisplayName(string languageCode) => languageCode switch
    {
        LanguageCodeEnglish => "English",
        LanguageCodeUkrainian => "Ukrainian",
        _ => "English"
    };
}
