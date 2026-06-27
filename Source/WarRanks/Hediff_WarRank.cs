using Verse;

namespace WarRanks
{
    // the hediff class behind every rank. the only thing it bothers to customise is its display
    // name: rather than the static def label it shows the title for whichever title set the
    // player picked in mod settings, so flipping to e.g. the Sindorei names updates the health
    // tab on the spot. the matching def carries no stage label, which is why this is the single
    // source of the shown name.
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
    }
}
