using Sudoku.Presentation.ViewModels.Base;

namespace Sudoku.Presentation.ViewModels;

/// <summary>
/// ViewModel for application settings.
/// Manages theme and language preferences.
/// </summary>
public class SettingsViewModel : ObservableObject
{
    private string _selectedTheme;
    private string _selectedLanguage;

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
    public SettingsViewModel()
    {
        _selectedTheme = "Light";
        _selectedLanguage = "English";

        ChangeThemeCommand = new RelayCommand(param => OnThemeChanged(param?.ToString() ?? "Light"));
        ChangeLanguageCommand = new RelayCommand(param => OnLanguageChanged(param?.ToString() ?? "English"));
    }

    /// <summary>
    /// Gets or sets the currently selected theme ("Light", "Dark", "Auto").
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
    public IReadOnlyList<string> AvailableThemes { get; } = new[] { "Light", "Dark", "Auto" };

    /// <summary>
    /// Gets the available languages.
    /// </summary>
    public IReadOnlyList<string> AvailableLanguages { get; } = new[] { "English", "Spanish", "French", "German" };

    /// <summary>
    /// Gets the command to change the theme.
    /// </summary>
    public RelayCommand ChangeThemeCommand { get; }

    /// <summary>
    /// Gets the command to change the language.
    /// </summary>
    public RelayCommand ChangeLanguageCommand { get; }

    /// <summary>
    /// Handles theme change and raises the ThemeChanged event.
    /// </summary>
    /// <param name="theme">The new theme selection.</param>
    private void OnThemeChanged(string theme)
    {
        if (AvailableThemes.Contains(theme))
        {
            SelectedTheme = theme;
            ThemeChanged?.Invoke(this, theme);
        }
    }

    /// <summary>
    /// Handles language change and raises the LanguageChanged event.
    /// </summary>
    /// <param name="language">The new language selection.</param>
    private void OnLanguageChanged(string language)
    {
        if (AvailableLanguages.Contains(language))
        {
            SelectedLanguage = language;
            LanguageChanged?.Invoke(this, language);
        }
    }
}
