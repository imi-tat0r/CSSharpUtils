using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace CSSharpUtils.Utils;

/// <summary>
/// Provides utility methods for managing game state.
/// </summary>
public static class GameUtils
{
    /// <summary>
    /// Starts the warmup period with a specified duration.
    /// </summary>
    /// <param name="time">The duration of the warmup period in seconds. If -1, the warmup will be indefinite.</param>
    public static void StartWarmup(int time = -1)
    {
        Server.ExecuteCommand(time > 0
            ? $"mp_warmuptime {time}; mp_warmup_pausetimer 0;mp_warmup_start"
            : "mp_warmuptime 10; mp_warmup_pausetimer 1;mp_warmup_start");
    }

    /// <summary>
    /// Ends the warmup period after a set time.
    /// </summary>
    /// <param name="time">If 0, ends the warmup immediately. Otherwise, sets the warmup to end after the specified time in seconds.</param>
    public static void EndWarmup(int time = 0)
    {
        Server.ExecuteCommand(time == 0
            ? "mp_warmup_end"
            : $"mp_warmuptime {time}; mp_warmup_pausetimer 0;mp_warmup_end");
    }

    /// <summary>
    /// Pauses the match.
    /// </summary>
    public static void PauseMatch()
    {
        Server.ExecuteCommand("mp_pause_match");
    }

    /// <summary>
    /// Unpauses the match.
    /// </summary>
    public static void UnpauseMatch()
    {
        Server.ExecuteCommand("mp_unpause_match");
    }

    /// <summary>
    /// Retrieves the current game rules.
    /// </summary>
    /// <returns>A <see cref="CCSGameRules"/> object, or null if not found.</returns>
    public static CCSGameRules? GetGameRules()
    {
        return Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").FirstOrDefault()?.GameRules;
    }
}