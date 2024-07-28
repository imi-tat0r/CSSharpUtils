using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;

namespace CSSharpUtils.Utils;

/// <summary>
/// Provides utility methods for working with CsTeams.
/// </summary>
public static class CsTeamUtils
{
    /// <summary>
    /// Gets the total number of players in the specified team.
    /// </summary>
    /// <param name="team">The team to count players in.</param>
    /// <returns>The number of players in the specified team.</returns>
    public static int GetPlayerCount(CsTeam team) => Utilities.GetPlayers().Count(p => p.TeamNum == (byte)team);

    /// <summary>
    /// Gets the count of alive players in the specified team.
    /// </summary>
    /// <param name="team">The team to count alive players in.</param>
    /// <returns>The number of alive players in the specified team.</returns>
    public static int GetPlayerAliveCount(CsTeam team) => Utilities.GetPlayers().Count(p => p.TeamNum == (byte)team && p.PawnHealth > 0);

    /// <summary>
    /// Gets the combined health of all players in the specified team.
    /// </summary>
    /// <param name="team">The team to calculate combined health for.</param>
    /// <returns>The total health of all players in the specified team.</returns>
    public static int GetCombinedHealth(CsTeam team) => Utilities.GetPlayers().Where(p => p.TeamNum == (byte)team).Sum(p => p.Health);

    /// <summary>
    /// Selects a random team between Terrorist and Counter-Terrorist.
    /// </summary>
    /// <returns>A randomly selected team.</returns>
    public static CsTeam GetRandomTeam()
    {
        var teams = new List<CsTeam>
        {
            CsTeam.Terrorist,
            CsTeam.CounterTerrorist
        };

        return teams.MinBy(_ => Guid.NewGuid());
    }
    
    /// <summary>
    /// Gets the chat color associated with a specific team.
    /// </summary>
    /// <param name="team">The team to get the chat color for.</param>
    /// <returns>The chat color character for the specified team.</returns>
    public static char GetChatColor(CsTeam team)
    {
        return team switch
        {
            CsTeam.Spectator => ChatColors.LightPurple,
            CsTeam.Terrorist => ChatColors.Yellow,
            CsTeam.CounterTerrorist => ChatColors.LightBlue,
            _ => ChatColors.Default
        };
    }
}