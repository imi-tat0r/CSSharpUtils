using System.Globalization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CSSharpUtils.Extensions;

/// <summary>
/// Provides extension methods for <see cref="CCSPlayerController"/> to perform common player actions.
/// </summary>
public static class PlayerControllerExtensions
{
    /// <summary>
    /// Freezes the player, preventing them from moving.
    /// </summary>
    /// <param name="playerController">The player controller to freeze.</param>
    public static void Freeze(this CCSPlayerController? playerController)
    {
        if (!playerController.IsPlayer())
            return;

        playerController!.PlayerPawn.Value!.Freeze();
    }
    
    /// <summary>
    /// Unfreezes the player, allowing them to move.
    /// </summary>
    /// <param name="playerController">The player controller to unfreeze.</param>
    public static void Unfreeze(this CCSPlayerController? playerController)
    {
        if (!playerController.IsPlayer())
            return;

        playerController!.PlayerPawn.Value!.Unfreeze();
    }
    
    /// <summary>
    /// Kicks the player from the server with a specified reason.
    /// </summary>
    /// <param name="playerController">The player controller to kick.</param>
    /// <param name="reason">The reason for kicking the player.</param>
    public static void Kick(this CCSPlayerController? playerController, string reason)
    {
        if (!playerController.IsPlayer())
            return;

        var kickCommand = string.Create(CultureInfo.InvariantCulture,
            $"kickid {playerController!.UserId!.Value} \"{reason}\"");

        // Queue for next frame to avoid threading issues
        Server.NextFrame(() => { Server.ExecuteCommand(kickCommand); });
    }

    /// <summary>
    /// Renames the player with a specified name and clan tag (optional).
    /// </summary>
    /// <param name="playerController">The player controller to set the name for.</param>
    /// <param name="name">The new name for the player.</param>
    public static void SetName(this CCSPlayerController? playerController, string name)
    {
        if (!playerController.IsPlayer())
            return;
        
        if (name == playerController!.PlayerName)
            return;
        
        playerController.PlayerName = name;
        Utilities.SetStateChanged(playerController, "CBasePlayerController", "m_iszPlayerName");
    }
    
    /// <summary>
    /// Renames the player with a specified name and clan tag (optional).
    /// </summary>
    /// <param name="playerController">The player controller to set the clantag for.</param>
    /// <param name="clantag">The new clan tag for the player.</param>
    /// <remarks>
    /// Requires <see cref="BasePluginExtensions.InitializeUtils"/> to be called in the OnPluginLoad method.
    /// </remarks>
    public static void SetClantag(this CCSPlayerController? playerController, string clantag = "")
    {
        if (!playerController.IsPlayer())
            return;
        
        if (clantag == playerController!.Clan)
            return;
        
        playerController.Clan = clantag;
        playerController.SetName(playerController.PlayerName + " ");
        
        // Set the state changed for the player name and clan tag after a delay
        BasePluginExtensions.PluginInstance.AddTimer(0.25f, () =>
        {
            Utilities.SetStateChanged(playerController, "CCSPlayerController", "m_szClan");
            Utilities.SetStateChanged(playerController, "CBasePlayerController", "m_iszPlayerName");
        });
        
        // Reset the player name to the intended value after a delay
        BasePluginExtensions.PluginInstance.AddTimer(0.3f, () =>
        {
            playerController.SetName(playerController.PlayerName.Trim());
        });
        
        // Set the state changed for the player name after another delay
        BasePluginExtensions.PluginInstance.AddTimer(0.4f, () =>
        {
            Utilities.SetStateChanged(playerController, "CBasePlayerController", "m_iszPlayerName");
        });
    }
    
    /// <summary>
    /// Moves the player to a specified team.
    /// </summary>
    /// <param name="playerController">The player controller to move.</param>
    /// <param name="team">The team to move the player to.</param>
    public static void MoveToTeam(this CCSPlayerController? playerController, CsTeam team)
    {
        if (!playerController.IsPlayer() || playerController!.TeamNum == (byte)team)
            return;

        // Queue for next frame to avoid threading issues
        Server.NextFrame(() => { playerController.ChangeTeam(team); });
    }

    /// <summary>
    /// Gets the eye position of the player.
    /// </summary>
    /// <param name="playerController">The player controller to get the eye position for.</param>
    /// <returns>The eye position as a <see cref="Vector"/>.</returns>
    public static Vector GetEyePosition(this CCSPlayerController? playerController)
    {
        if (!playerController.IsPlayer())
            return Vector.Zero;

        var absOrigin = playerController?.PlayerPawn.Value?.AbsOrigin ?? Vector.Zero;
        var camera = playerController?.PlayerPawn.Value?.CameraServices;

        return new Vector(absOrigin.X, absOrigin.Y, absOrigin.Z + camera?.OldPlayerViewOffsetZ);
    }

    /// <summary>
    /// Sets the armor value for the player, optionally including a helmet and heavy armor.
    /// </summary>
    /// <param name="playerController">The player controller to set armor for.</param>
    /// <param name="armor">The armor value to set.</param>
    /// <param name="helmet">Whether to include a helmet.</param>
    /// <param name="heavy">Whether to include heavy armor.</param>
    public static void SetArmor(this CCSPlayerController? playerController, int armor, bool helmet = false, bool heavy = false)
    {
        if (!playerController.IsPlayer() || !playerController!.PawnIsAlive)
            return;

        playerController.PlayerPawn.Value!.ArmorValue = armor;
        Utilities.SetStateChanged(playerController.PlayerPawn.Value, "CCSPlayerPawnBase", "m_ArmorValue");

        if (!helmet && !heavy)
            return;

        var services = new CCSPlayer_ItemServices(playerController.PlayerPawn.Value.ItemServices!.Handle);
        services.HasHelmet = helmet;
        services.HasHeavyArmor = heavy;
        Utilities.SetStateChanged(playerController.PlayerPawn.Value, "CBasePlayerPawn", "m_pItemServices");
    }
    
    /// <summary>
    /// Sets the health value for the player
    /// </summary>
    /// <param name="playerController">The player controller to set health for.</param>
    /// <param name="health">The health value to set.</param>
    /// <param name="allowOverflow">Whether to allow the health to exceed the maximum health value.</param>
    public static void SetHealth(this CCSPlayerController? playerController, int health, bool allowOverflow = true)
    {
        if (!playerController.IsPlayer() || !playerController!.PawnIsAlive)
            return;

        playerController.PlayerPawn.Value!.Health = health;
        
        if (allowOverflow && health > playerController.PlayerPawn.Value.MaxHealth)
            playerController.PlayerPawn.Value.MaxHealth = health;
        
        Utilities.SetStateChanged(playerController.PlayerPawn.Value, "CBaseEntity", "m_iHealth");
    }
    
    /// <summary>
    /// Sets the money for the player
    /// </summary>
    /// <param name="playerController">The player controller to set money for.</param>
    /// <param name="money">The money value to set.</param>
    public static void SetMoney(this CCSPlayerController? playerController, int money)
    {
        if (!playerController.IsPlayer())
            return;
        
        var moneyServices = playerController!.InGameMoneyServices;
        if (moneyServices == null)
            return;
        
        moneyServices.Account = money;
        Utilities.SetStateChanged(playerController, "CCSPlayerController", "m_pInGameMoneyServices");
    }

    /// <summary>
    /// Checks if the specified controller represents a valid player.
    /// </summary>
    /// <param name="player">The player controller to check.</param>
    /// <returns><c>true</c> if the player is valid; otherwise, <c>false</c>.</returns>
    public static bool IsPlayer(this CCSPlayerController? player)
    {
        return player is
            { PlayerPawn.Value: not null, IsValid: true, IsHLTV: false, IsBot: false, UserId: not null, SteamID: > 0, Connected: PlayerConnectedState.PlayerConnected };
    }
}