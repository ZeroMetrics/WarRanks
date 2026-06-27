using Verse;

namespace WarRanks
{
    // persisted player choices. just which title set is selected for now, stored by its string id
    // so adding/removing sets later doesn't shuffle a saved value out from under the player.
    public class WarRanksSettings : ModSettings
    {
        public string TitleSetId = "Unified";

        public override void ExposeData()
        {
            Scribe_Values.Look(ref TitleSetId, "titleSet", "Unified");
            base.ExposeData();
        }
    }
}
