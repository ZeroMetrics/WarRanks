using Verse;

namespace WarRanks
{
    // persisted player choices. just the flavour-name set for now, but this is the place to bolt
    // on anything else that should survive a restart.
    public class WarRanksSettings : ModSettings
    {
        public WarRankTitleSet TitleSet = WarRankTitleSet.Unified;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref TitleSet, "titleSet", WarRankTitleSet.Unified);
            base.ExposeData();
        }
    }
}
