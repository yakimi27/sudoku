using Sudoku.Core.Models;
using Sudoku.Core.Services;
using Sudoku.Infrastructure.Repositories;

namespace Sudoku.Infrastructure.Services;

/// <summary>
/// Implements daily challenge management including completion tracking and streak calculation.
/// Uses ProfileRepository to persist completion data.
/// </summary>
public class DailyChallengeService : IDailyChallengeService
{
    private readonly ProfileRepository _profileRepository;
    private readonly SessionRepository _sessionRepository;
    private DailyChallenge? _todaysChallengeCache;

    /// <summary>
    /// Initializes a new instance of the DailyChallengeService class.
    /// </summary>
    /// <param name="profileRepository">Repository for profile persistence.</param>
    /// <param name="sessionRepository">Repository for session persistence.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public DailyChallengeService(ProfileRepository profileRepository, SessionRepository sessionRepository)
    {
        _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    }

    /// <summary>
    /// Gets today's daily challenge asynchronously.
    /// Caches the result to avoid repeated creation.
    /// </summary>
    /// <returns>A task that resolves to today's DailyChallenge.</returns>
    public Task<DailyChallenge> GetTodaysChallengeAsync()
    {
        if (_todaysChallengeCache != null && _todaysChallengeCache.ChallengeDate.Date == DateTime.UtcNow.Date)
        {
            return Task.FromResult(_todaysChallengeCache);
        }

        var challenge = DailyChallenge.GetTodayChallenge();
        _todaysChallengeCache = challenge;
        return Task.FromResult(challenge);
    }

    /// <summary>
    /// Marks today's challenge as completed with the given time.
    /// Persists the completion to profile storage.
    /// </summary>
    /// <param name="completionTime">The time taken to complete the challenge.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task MarkCompletedAsync(TimeSpan completionTime)
    {
        var challenge = await GetTodaysChallengeAsync();
        challenge.MarkCompleted(completionTime);

        // Update profile with completion
        var profile = PlayerProfile.Instance;
        profile.MarkAsModified();
        await _profileRepository.SaveProfileAsync(profile);

        // Optionally persist challenge record
        // This could be extended to track historical completions
    }

    /// <summary>
    /// Gets the current completion streak (consecutive days completed).
    /// A streak breaks if a day is missed.
    /// </summary>
    /// <returns>The streak count as an integer.</returns>
    public int GetStreak()
    {
        int streak = 0;
        DateTime currentDate = DateTime.UtcNow.Date;

        // Walk backwards through days to count consecutive completions
        while (true)
        {
            // Check if today or a previous day was completed
            if (IsDayCompletedSync(currentDate))
            {
                streak++;
                currentDate = currentDate.AddDays(-1);
            }
            else
            {
                break;
            }
        }

        return streak;
    }

    /// <summary>
    /// Gets the completion status for a specific date asynchronously.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>A task that resolves to true if completed, false otherwise.</returns>
    public async Task<bool> IsCompletedAsync(DateTime date)
    {
        // Get all sessions and check if any daily challenges match this date
        // This is a placeholder implementation - extend as needed
        return await Task.FromResult(IsDayCompletedSync(date));
    }

    /// <summary>
    /// Synchronous helper to check if a day was completed.
    /// </summary>
    private bool IsDayCompletedSync(DateTime date)
    {
        // In a real implementation, this would query session records
        // For now, we'll use a simple heuristic based on today
        // This should be extended to actually query historical data

        if (date.Date == DateTime.UtcNow.Date)
        {
            // Check if today's challenge exists and is marked complete
            var todaysChallenge = DailyChallenge.GetTodayChallenge();
            return todaysChallenge.IsCompleted;
        }

        // For past dates, you would need to query a persistent store
        // This is simplified for the current implementation
        return false;
    }
}
