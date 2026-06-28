using Verse;

namespace WarRanks
{
    // the hediff class behind every rank. it customises two things, both so the selected title set
    // drives what the player sees:
    //   - LabelBase: the displayed name (e.g. "Gravebound" instead of the static def label)
    //   - GetTooltip: swaps the generic def description for the set's flavour text, when it has one
    // the matching def carries no stage label, which is why this is the single source of the name.
    public class Hediff_WarRank : HediffWithComps
    {
        public override string LabelBase
        {
            get
            {
                WarRank rank = WarRankUtility.RankForHediffDef(def);
                return rank == null ? base.LabelBase : WarRankTitles.TitleFor(rank);
            }
        }

        public override string GetTooltip(Pawn pawn, bool showHediffsDebugInfo)
        {
            string tip = base.GetTooltip(pawn, showHediffsDebugInfo);

            WarRank rank = WarRankUtility.RankForHediffDef(def);
            if (rank == null) return tip;

            string flavour = WarRankTitles.DescriptionFor(rank);
            if (flavour.NullOrEmpty()) return tip; // this set hasn't been written one - leave the def text

            // the base tooltip embeds def.description verbatim, so swap that paragraph out. if the
            // format ever changes and we can't find it, just append the flavour so it still shows.
            if (!def.description.NullOrEmpty() && tip.Contains(def.description))
                return tip.Replace(def.description, flavour);
            return tip + "\n\n" + flavour;
        }
    }
}
