using Microsoft.UI.Xaml;

namespace Sudoku;

/// <summary>
/// Main application window for the Sudoku game.
/// </summary>
public sealed partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the MainWindow class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        Title = "Sudoku - MVVM Game";
    }
}
