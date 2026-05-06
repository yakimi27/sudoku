using Sudoku.Core.Services;
using Sudoku.Presentation.ViewModels.Base;

namespace Sudoku.Presentation.ViewModels;

/// <summary>
/// ViewModel for the player profile screen.
/// Allows editing of profile information like name and avatar selection.
/// </summary>
public class ProfileViewModel : ObservableObject
{
    private readonly IProfileService _profileService;
    private string _playerName;
    private int _avatarIndex;

    /// <summary>
    /// Event raised when the profile is saved successfully.
    /// </summary>
    public event EventHandler? ProfileSaved;

    /// <summary>
    /// Initializes a new instance of the ProfileViewModel class.
    /// </summary>
    /// <param name="profileService">The profile service for managing profile data.</param>
    /// <exception cref="ArgumentNullException">Thrown when profileService is null.</exception>
    public ProfileViewModel(IProfileService profileService)
    {
        _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
        _playerName = _profileService.PlayerName;
        _avatarIndex = 0;

        SaveProfileCommand = new RelayCommand(_ => SaveProfileAsync(), _ => !string.IsNullOrWhiteSpace(PlayerName));
    }

    /// <summary>
    /// Gets or sets the player's display name.
    /// </summary>
    public string PlayerName
    {
        get => _playerName;
        set
        {
            if (SetProperty(ref _playerName, value))
            {
                (SaveProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected avatar index.
    /// </summary>
    public int AvatarIndex
    {
        get => _avatarIndex;
        set => SetProperty(ref _avatarIndex, value);
    }

    /// <summary>
    /// Gets the command to save profile changes.
    /// </summary>
    public RelayCommand SaveProfileCommand { get; }

    /// <summary>
    /// Saves the profile changes asynchronously.
    /// </summary>
    private async void SaveProfileAsync()
    {
        if (string.IsNullOrWhiteSpace(PlayerName))
            return;

        try
        {
            await _profileService.SetPlayerNameAsync(PlayerName);
            ProfileSaved?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception)
        {
            // Log or handle save error
        }
    }
}
