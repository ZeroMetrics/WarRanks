using HarmonyLib;
using UnityEngine;
using Verse;

namespace WarRanks
{
    // mod entry point: holds the settings instance and gets harmony wired up at startup.
    public class WarRanksMod : Mod
    {
        public const string PackageId = "zerometrics.warranks";

        public static WarRanksSettings Settings;

        public WarRanksMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<WarRanksSettings>();
            // patches everything tagged in this assembly - which is just the overlay draw. the
            // rank logic itself lives on a game component, no patch involved.
            new Harmony(PackageId).PatchAll();
        }

        public override string SettingsCategory()
        {
            return "War Ranks";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Label("Rank title set");
            foreach (WarRankTitleSet titleSet in WarRankTitles.AllTitleSets)
            {
                // RadioButton returns true only on the click, so we only write settings then.
                if (listing.RadioButton(WarRankTitles.LabelFor(titleSet), Settings.TitleSet == titleSet))
                {
                    Settings.TitleSet = titleSet;
                    Settings.Write();
                }
            }

            listing.Gap();
            listing.Label("This only changes displayed rank names. Kill thresholds, icons, and buffs are unchanged.");
            listing.End();
        }
    }
}
