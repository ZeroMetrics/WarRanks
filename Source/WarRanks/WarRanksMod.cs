using System.Collections.Generic;
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
            return "WarRanks";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Label("Rank title set");

            // a dropdown rather than a row of radio buttons - with nine sets a list would eat the
            // whole window. the button shows the current set; clicking it drops a menu of the rest.
            if (listing.ButtonText(WarRankTitles.Current.Label))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach (WarRankTitleSet titleSet in WarRankTitles.AllSets)
                {
                    WarRankTitleSet chosen = titleSet; // capture a copy for the closure below
                    options.Add(new FloatMenuOption(chosen.Label, delegate
                    {
                        Settings.TitleSetId = chosen.Id;
                        Settings.Write();
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }

            listing.Gap();
            listing.Label("This only changes displayed rank names. Kill thresholds, icons, and buffs are unchanged.");
            listing.End();
        }
    }
}
