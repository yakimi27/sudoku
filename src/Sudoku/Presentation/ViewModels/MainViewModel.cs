using Sudoku.Core.Enums;
using Sudoku.Presentation.ViewModels.Base;

namespace Sudoku.Presentation.ViewModels;

/// <summary>
/// ViewModel for the main application shell.
/// Manages page navigation and application-level state like theme and language.
/// </summary>
public class MainViewModel : ObservableObject
{
    private PageType _currentPage;
    private string _applicationTheme;
    private string _applicationLanguage;

    /// <summary>
    /// Enumerates the available pages in the application.
    /// </summary>
    public enum PageType
    {
        /// <summary>
        /// Main menu page.
        /// </summary>
        Menu = 0,

        /// <summary>
        /// Active game page.
        /// </summary>
        Game = 1,

        /// <summary>
        /// Statistics page.
        /// </summary>
        Statistics = 2,

        /// <summary>
        /// Settings page.
        /// </summary>
        Settings = 3,

        /// <summary>
        /// Player profile page.
        /// </summary>
        Profile = 4
    }

    /// <summary>
    /// Event raised when navigation to a new page is requested.
    /// </summary>
    public event EventHandler<PageType>? NavigationRequested;

    /// <summary>
    /// Initializes a new instance of the MainViewModel class.
    /// </summary>
    public MainViewModel()
    {
        _currentPage = PageType.Menu;
        _applicationTheme = "Light";
        _applicationLanguage = "English";

        NavigateCommand = new RelayCommand(param => NavigateTo(param), CanNavigate);
    }

    /// <summary>
    /// Gets or sets the currently displayed page.
    /// </summary>
    public PageType CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    /// <summary>
    /// Gets or sets the application theme ("Light", "Dark", "Auto").
    /// </summary>
    public string ApplicationTheme
    {
        get => _applicationTheme;
        set => SetProperty(ref _applicationTheme, value);
    }

    /// <summary>
    /// Gets or sets the application language.
    /// </summary>
    public string ApplicationLanguage
    {
        get => _applicationLanguage;
        set => SetProperty(ref _applicationLanguage, value);
    }

    /// <summary>
    /// Gets the command to navigate to a page.
    /// </summary>
    public RelayCommand NavigateCommand { get; }

    /// <summary>
    /// Navigates to the specified page.
    /// </summary>
    /// <param name="pageParameter">The page to navigate to (can be PageType or string representation).</param>
    public void NavigateTo(object? pageParameter)
    {
        PageType pageType = pageParameter switch
        {
            PageType page => page,
            string pageName when Enum.TryParse<PageType>(pageName, ignoreCase: true, out var result) => result,
            int pageValue when Enum.IsDefined(typeof(PageType), pageValue) => (PageType)pageValue,
            _ => CurrentPage
        };

        if (CanNavigate(pageParameter))
        {
            CurrentPage = pageType;
            NavigationRequested?.Invoke(this, pageType);
        }
    }

    /// <summary>
    /// Determines whether navigation to a page is allowed.
    /// </summary>
    /// <param name="pageParameter">The page parameter to check.</param>
    /// <returns>True if navigation is allowed, false otherwise.</returns>
    private bool CanNavigate(object? pageParameter)
    {
        return pageParameter != null;
    }

    /// <summary>
    /// Sets the application theme and raises property change notification.
    /// </summary>
    /// <param name="theme">The theme to set ("Light", "Dark", or "Auto").</param>
    public void SetApplicationTheme(string theme)
    {
        if (!string.IsNullOrWhiteSpace(theme))
        {
            ApplicationTheme = theme;
        }
    }

    /// <summary>
    /// Sets the application language and raises property change notification.
    /// </summary>
    /// <param name="language">The language to set (e.g., "English", "Spanish", "French").</param>
    public void SetApplicationLanguage(string language)
    {
        if (!string.IsNullOrWhiteSpace(language))
        {
            ApplicationLanguage = language;
        }
    }
}
