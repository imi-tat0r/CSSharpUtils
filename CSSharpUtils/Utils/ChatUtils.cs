using System.Reflection;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;

namespace CSSharpUtils.Utils;

/// <summary>
/// Provides utility methods for chat operations.
/// </summary>
public static class ChatUtils
{
    /// <summary>
    /// Represents a new line character for chat messages.
    /// </summary>
    public static char NewLine = '\u2029';

    /// <summary>
    /// A dictionary mapping predefined color names to their corresponding character codes.
    /// </summary>
    private static readonly Dictionary<string, char> PredefinedColors = typeof(ChatColors)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .ToDictionary(field => $"{{{field.Name}}}", field => (char)(field.GetValue(null) ?? '\x01'));

    /// <summary>
    /// Applies predefined color codes to a message string.
    /// </summary>
    /// <param name="message">The message to format with color codes.</param>
    /// <returns>A new string with color codes applied.</returns>
    public static string FormatMessage(string message) =>
        PredefinedColors.Aggregate(message, (current, color) => current.Replace(color.Key, $"{color.Value}"));

    /// <summary>
    /// Removes all predefined color tags and codes from a message string.
    /// </summary>
    /// <param name="message">The message to clean of color codes.</param>
    /// <returns>A new string with all color codes removed.</returns>
    public static string CleanMessage(string message) =>
        PredefinedColors.Aggregate(message, (current, color) => current.Replace(color.Key, "").Replace(color.Value.ToString(), ""));

    /// <summary>
    /// Sends a chat message to all players in a specified team.
    /// </summary>
    /// <param name="team">The team to which the message will be sent.</param>
    /// <param name="message">The message to send.</param>
    public static void PrintToTeam(CsTeam team, string message)
    {
        var teamPlayers = Utilities.GetPlayers().Where(p => p.TeamNum == (byte)team);
        foreach (var player in teamPlayers)
            player.PrintToChat(message);
    }
}