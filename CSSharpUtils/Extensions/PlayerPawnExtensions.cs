using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;

namespace CSSharpUtils.Extensions;

public static class PlayerPawnExtensions
{
    private static void SetMoveType(this CCSPlayerPawn pawn, MoveType_t moveType)
    {
        pawn.MoveType = moveType;
        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_MoveType");
        Schema.GetRef<MoveType_t>(pawn.Handle, "CBaseEntity", "m_nActualMoveType") = moveType;
    }
    
    public static void Freeze(this CCSPlayerPawn pawn) => pawn.SetMoveType(MoveType_t.MOVETYPE_OBSOLETE);
    public static void Unfreeze(this CCSPlayerPawn pawn) => pawn.SetMoveType(MoveType_t.MOVETYPE_WALK);
}