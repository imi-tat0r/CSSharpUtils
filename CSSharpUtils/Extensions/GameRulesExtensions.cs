using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace CSSharpUtils.Extensions;

/// <summary>
/// Provides extension methods for <see cref="CCSGameRules"/> to enhance functionality.
/// </summary>
public static class GameRulesExtensions
{
    /// <summary>
    /// Calculates the remaining time in the current round.
    /// </summary>
    /// <param name="gameRules">The game rules instance, or null if not available.</param>
    /// <returns>The remaining time in seconds; returns 0.0f if <paramref name="gameRules"/> is null.</returns>
    public static float GetRemainingRoundTime(this CCSGameRules? gameRules)
    {
        if (gameRules == null)
            return 0.0f;

        // Calculate remaining time by subtracting the current time from the sum of round start time and round duration
        return (gameRules.RoundStartTime + gameRules.RoundTime) - Server.CurrentTime;
    }
}