using HarmonyLib;
using Verse;

namespace WarRanks
{
    // the one spot we genuinely need harmony: there's no clean hook for drawing extra art over a
    // pawn, so we postfix the vanilla overlay pass and tack our rank pip on after the name label.
    // everything else (handing out ranks) runs off GameComponent_WarRanks, no patch needed.
    [HarmonyPatch(typeof(Pawn), "DrawGUIOverlay")]
    public static class Pawn_DrawGUIOverlay_Patch
    {
        public static void Postfix(Pawn __instance)
        {
            WarRankOverlayDrawer.DrawFor(__instance);
        }
    }
}
