using CounterStrikeSharp.API;
using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CSSharpUtils;

public static class HudMessageExtensions
{
    private static Dictionary<uint, CCSPlayerController> WorldTextOwners = new();

    public static CPointWorldText PrintToHud(this CCSPlayerController player, string text, int size = 100, Color? color = null, string font = "Verdana Bold", float shiftX = 0f, float shiftY = 0f, bool background = true, float backgroundWidth = 0.10f, float backgroundHeight = 0.10f)
    {
        var pawnBase = player.Pawn.Value;
        if (pawnBase == null)
            return null!;

        var pawn = pawnBase.As<CCSPlayerPawn>();
        if (pawn == null)
            return null!;

        bool isSpectating = pawn.LifeState == (byte)LifeState_t.LIFE_DEAD;
        if (isSpectating)
        {
            var observerServices = pawnBase.ObserverServices;
            if (observerServices == null)
                return null!;

            var observerPawn = observerServices.ObserverTarget?.Value?.As<CCSPlayerPawn>();
            if (observerPawn == null || !observerPawn.IsValid)
                return null!;

            pawn = observerPawn;
        }

        var viewModel = player.EnsureCustomView(0);
        if (viewModel == null)
            return null!;

        return CreateWorldText(
            player,
            pawn,
            viewModel,
            text,
            size,
            color,
            font,
            shiftX,
            shiftY,
            background,
            backgroundHeight,
            backgroundWidth,
            isSpectating
        );
    }

    internal static CPointWorldText? CreateForAlive(CCSPlayerController player, string text, int size = 100, Color? color = null, string font = "", float shiftX = 0f, float shiftY = 0f, bool background = true, float backgroundWidth = 0.10f, float backgroundHeight = 0.10f)
    {
        CCSGOViewModel? viewModel = player.EnsureCustomView(0);
        if (viewModel == null)
            return null;

        var pawnBase = player.Pawn.Value;
        if (pawnBase == null)
            return null;

        CCSPlayerPawn? pawn = pawnBase.As<CCSPlayerPawn>();
        if (pawn == null)
            return null;

        return CreateWorldText(
            player,
            pawn,
            viewModel,
            text,
            size,
            color,
            font,
            shiftX,
            shiftY,
            background,
            backgroundHeight,
            backgroundWidth,
            false
        );
    }

    internal static CPointWorldText? CreateForDead(CCSPlayerController player, string text, int size = 100, Color? color = null, string font = "", float shiftX = 0f, float shiftY = 0f, bool background = true, float backgroundWidth = 0.10f, float backgroundHeight = 0.10f)
    {
        CCSGOViewModel? viewModel = player.EnsureCustomView(0);
        if (viewModel == null)
            return null;

        var pawnBase = player.Pawn.Value;
        if (pawnBase == null)
            return null;

        var observerServices = pawnBase.ObserverServices;
        if (observerServices == null)
            return null;

        CCSPlayerPawn? observerPawn = observerServices.ObserverTarget?.Value?.As<CCSPlayerPawn>();
        if (observerPawn == null || !observerPawn.IsValid)
            return null;

        CCSPlayerPawn pawn = observerPawn;

        return CreateWorldText(
            player,
            pawn,
            viewModel,
            text,
            size,
            color,
            font,
            shiftX,
            shiftY,
            background,
            backgroundHeight,
            backgroundWidth,
            true
        );
    }

    private static CPointWorldText CreateWorldText(
        CCSPlayerController effectiveOwner,
        CCSPlayerPawn pawn,
        CCSGOViewModel viewModel,
        string text,
        int size,
        Color? color,
        string font,
        float shiftX,
        float shiftY,
        bool drawBackground,
        float backgroundHeight,
        float backgroundWidth,
        bool isSpectating
    )
    {
        QAngle eyeAngles = pawn.EyeAngles;
        Vector forward = new(), right = new(), up = new();
        NativeAPI.AngleVectors(eyeAngles.Handle, forward.Handle, right.Handle, up.Handle);

        Vector offset = forward * 7 + right * shiftX + up * shiftY;

        QAngle angles = new()
        {
            Y = eyeAngles.Y + 270,
            Z = 90 - eyeAngles.X,
            X = 0
        };

        var finalPos = pawn.AbsOrigin! + offset + new Vector(0, 0, pawn.ViewOffset.Z);

        CPointWorldText worldText = Utilities.CreateEntityByName<CPointWorldText>("point_worldtext")!;
        worldText.MessageText = text;
        worldText.Enabled = true;
        worldText.FontSize = size;
        worldText.Fullbright = true;
        worldText.Color = color ?? Color.Aquamarine;
        worldText.WorldUnitsPerPx = (0.25f / 1050) * size;
        worldText.FontName = font;
        worldText.JustifyHorizontal = PointWorldTextJustifyHorizontal_t.POINT_WORLD_TEXT_JUSTIFY_HORIZONTAL_LEFT;
        worldText.JustifyVertical = PointWorldTextJustifyVertical_t.POINT_WORLD_TEXT_JUSTIFY_VERTICAL_CENTER;
        worldText.ReorientMode = PointWorldTextReorientMode_t.POINT_WORLD_TEXT_REORIENT_NONE;
        worldText.RenderMode = RenderMode_t.kRenderNormal;

        if (drawBackground)
        {
            worldText.DrawBackground = true;
            worldText.BackgroundBorderWidth = backgroundWidth;
            worldText.BackgroundBorderHeight = backgroundHeight;
        }

        worldText.DispatchSpawn();
        worldText.Teleport(finalPos, angles, null);
        worldText.AcceptInput("ClearParent");
        worldText.AcceptInput("SetParent", viewModel, null, "!activator");

        WorldTextOwners[worldText.Index] = effectiveOwner;

        return worldText;
    }

    internal static CCSGOViewModel? EnsureCustomView(this CCSPlayerController player, int index)
    {
        CCSPlayerPawnBase? pPawnBase = player.PlayerPawn.Value?.As<CCSPlayerPawnBase>();

        if (pPawnBase == null)
            return null;

        if (pPawnBase.LifeState == (byte)LifeState_t.LIFE_DEAD)
        {
            CCSPlayerPawn? playerPawn = player.PlayerPawn.Value;
            if (playerPawn == null || !playerPawn.IsValid)
                return null;

            CPlayer_ObserverServices? observerServices = playerPawn.ObserverServices;
            if (observerServices == null)
                return null;

            var observerPawn = observerServices.ObserverTarget?.Value?.As<CCSPlayerPawn>();
            if (observerPawn == null || !observerPawn.IsValid)
                return null;

            var observerController = observerPawn.OriginalController.Value;
            if (observerController == null || !observerController.IsValid)
                return null;

            pPawnBase = observerController.PlayerPawn.Value?.As<CCSPlayerPawnBase>();
            if (pPawnBase == null)
                return null;
        }
        var pawn = pPawnBase as CCSPlayerPawn;
        if (pawn == null)
            return null;

        if (pawn.ViewModelServices == null)
            return null;

        int offset = Schema.GetSchemaOffset("CCSPlayer_ViewModelServices", "m_hViewModel");
        IntPtr viewModelHandleAdress = (IntPtr)(pawn.ViewModelServices.Handle + offset + 4);

        var handle = new CHandle<CCSGOViewModel>(viewModelHandleAdress);
        if (!handle.IsValid)
        {
            CCSGOViewModel? viewModel = Utilities.CreateEntityByName<CCSGOViewModel>("predicted_viewmodel");
            if (viewModel == null)
                return null;

            viewModel.DispatchSpawn();
            handle.Raw = viewModel.EntityHandle.Raw;
            Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_pViewModelServices");
        }

        return handle.Value;
    }

    public static void RemoveHud(CPointWorldText worldText)
    {
        if (worldText != null && worldText.IsValid)
        {
            worldText.AcceptInput("Kill", worldText);
            worldText.Remove();
            if (WorldTextOwners.ContainsKey(worldText.Index))
            {
                WorldTextOwners.Remove(worldText.Index);
            }
        }
    }
}