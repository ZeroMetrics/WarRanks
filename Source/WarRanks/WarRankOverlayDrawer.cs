using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WarRanks
{
    // draws the little rank pip floating above a colonist. it only shows while the pawn is
    // selected or drafted, so the map isn't peppered with icons the rest of the time.
    public static class WarRankOverlayDrawer
    {
        // icon textures are pulled from disk once and kept around, keyed by rank level.
        private static readonly Dictionary<int, Texture2D> IconCache = new Dictionary<int, Texture2D>();

        // pip size and how far above the name label it sits, in screen pixels. tuned by eye.
        private const float IconSize = 16f;
        private const float VerticalOffset = 33f;

        public static void DrawFor(Pawn pawn)
        {
            if (!WarRankUtility.IsEligibleColonist(pawn)) return;

            // selected or drafted only - skip the draw entirely otherwise.
            bool selected = Find.Selector != null && Find.Selector.IsSelected(pawn);
            if (!pawn.Drafted && !selected) return;

            Hediff hediff = WarRankHediffUtility.CurrentWarRankHediff(pawn);
            WarRank rank = WarRankUtility.RankForHediffDef(hediff == null ? null : hediff.def);
            if (rank == null) return;

            Texture2D icon = IconFor(rank);
            if (icon == null) return;

            // same anchor vanilla uses for the name tag, nudged up so the pip sits above it.
            Vector2 labelPos = GenMapUI.LabelDrawPosFor(pawn, -0.75f);
            Rect rect = new Rect(labelPos.x - IconSize / 2f, labelPos.y - VerticalOffset, IconSize, IconSize);

            GUI.color = Color.white;
            GUI.DrawTexture(rect, icon);
            TooltipHandler.TipRegion(rect, WarRankTitles.TitleFor(rank) + "\n" + WarRankUtility.RelevantKills(pawn) + " relevant kills");
        }

        public static Texture2D IconFor(WarRank rank)
        {
            Texture2D icon;
            if (IconCache.TryGetValue(rank.Level, out icon)) return icon;

            // reportFailure=false: a missing icon just means no pip, not a red error every frame.
            icon = ContentFinder<Texture2D>.Get(rank.IconPath, false);
            IconCache[rank.Level] = icon;
            return icon;
        }
    }
}
