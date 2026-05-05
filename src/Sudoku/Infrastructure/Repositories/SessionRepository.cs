using System.Text.Json;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Sudoku.Infrastructure.Storage;

namespace Sudoku.Infrastructure.Repositories;

/// <summary>
/// Repository for persisting and retrieving game sessions.
/// Manages the active game session storage using JSON serialization.
/// </summary>
public sealed class SessionRepository
{
    private const string SessionStorageKey = "active_session";

    private readonly LocalStorageService _storageService;

    /// <summary>
    /// Initializes a new instance of the SessionRepository class.
    /// </summary>
    /// <param name="storageService">The storage service for persisting session data.</param>
    /// <exception cref="ArgumentNullException">Thrown when storageService is null.</exception>
    public SessionRepository(LocalStorageService storageService)
    {
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
    }

    /// <summary>
    /// Saves a game session to storage.
    /// </summary>
    /// <param name="session">The game session to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when session is null.</exception>
    public async Task SaveSessionAsync(GameSession session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        // Store the session summary and board data separately for efficient serialization
        var sessionData = new SessionStorageData
        {
            Summary = session.GetSummary(),
            BoardData = SerializeBoard(session.Board)
        };

        await _storageService.SaveAsync(SessionStorageKey, sessionData);
    }

    /// <summary>
    /// Loads the active game session from storage.
    /// </summary>
    /// <returns>The loaded GameSession, or null if no session is saved.</returns>
    public async Task<GameSession?> LoadSessionAsync()
    {
        var data = await _storageService.LoadAsync<SessionStorageData>(SessionStorageKey);

        if (data?.Summary == null || data.BoardData == null)
            return null;

        try
        {
            var board = DeserializeBoard(data.BoardData);
            var session = new GameSession(board, data.Summary.GameMode, data.Summary.Difficulty)
            {
                // Restore session properties from summary
            };

            // Use reflection or factory method to restore the session state
            session.UpdateElapsedTime(data.Summary.ElapsedTime);

            // Restore hints used by calling UseHint() the appropriate number of times
            for (int i = 0; i < data.Summary.HintsUsed; i++)
            {
                session.UseHint();
            }

            if (data.Summary.IsCompleted)
            {
                session.Complete();
            }

            return session;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Clears the active game session from storage.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ClearSessionAsync()
    {
        await _storageService.DeleteAsync(SessionStorageKey);
    }

    /// <summary>
    /// Serializes a SudokuBoard to JSON-compatible data.
    /// </summary>
    /// <param name="board">The board to serialize.</param>
    /// <returns>Board data as JSON string.</returns>
    private static string SerializeBoard(SudokuBoard board)
    {
        var cellData = new List<CellData>();

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var cell = board.GetCell(row, col);
                cellData.Add(new CellData
                {
                    Row = row,
                    Column = col,
                    Value = cell.Value,
                    IsGiven = cell.IsGiven,
                    State = cell.State,
                    Notes = cell.Notes.ToList()
                });
            }
        }

        return System.Text.Json.JsonSerializer.Serialize(cellData);
    }

    /// <summary>
    /// Deserializes board data from JSON back into a SudokuBoard.
    /// </summary>
    /// <param name="boardJson">The serialized board data.</param>
    /// <returns>The reconstructed SudokuBoard.</returns>
    private static SudokuBoard DeserializeBoard(string boardJson)
    {
        var cellDataList = System.Text.Json.JsonSerializer.Deserialize<List<CellData>>(boardJson)
            ?? new List<CellData>();

        var board = SudokuBoard.CreateEmpty();

        foreach (var cellData in cellDataList)
        {
            var cell = new Cell(
                cellData.Row,
                cellData.Column,
                cellData.Value,
                cellData.IsGiven,
                cellData.State,
                System.Collections.Immutable.ImmutableHashSet.CreateRange(cellData.Notes)
            );

            board.SetCell(cellData.Row, cellData.Column, cell);
        }

        return board;
    }

    /// <summary>
    /// Represents session data for storage, combining summary and board state.
    /// </summary>
    private class SessionStorageData
    {
        /// <summary>
        /// Gets or sets the session summary.
        /// </summary>
        public GameSessionSummary? Summary { get; set; }

        /// <summary>
        /// Gets or sets the serialized board data.
        /// </summary>
        public string? BoardData { get; set; }
    }

    /// <summary>
    /// Represents cell data for JSON serialization.
    /// </summary>
    private class CellData
    {
        /// <summary>
        /// Gets or sets the row index.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the cell value.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Gets or sets whether the cell is given.
        /// </summary>
        public bool IsGiven { get; set; }

        /// <summary>
        /// Gets or sets the cell state.
        /// </summary>
        public CellState State { get; set; }

        /// <summary>
        /// Gets or sets the notes for the cell.
        /// </summary>
        public List<int> Notes { get; set; } = new();
    }
}
