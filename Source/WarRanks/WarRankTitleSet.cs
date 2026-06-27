using System.Collections.Generic;

namespace WarRanks
{
    // one named set of rank titles - e.g. the plain military set, or a race-flavoured one.
    // purely cosmetic: the ladder, kill thresholds and buffs are identical no matter which set
    // is picked, only the words shown to the player change.
    public sealed class WarRankTitleSet
    {
        public readonly string Id;     // stable key saved in settings - don't rename once shipped
        public readonly string Label;  // what shows next to the radio button in mod options
        private readonly string[] titles; // 24 names, low rank to high; index = rank.Level - 1

        public WarRankTitleSet(string id, string label, string[] titles)
        {
            Id = id;
            Label = label;
            this.titles = titles;
        }

        // name for a given rank in this set. falls back to the rank's built-in name if the array
        // is short or missing, so a half-finished set can't crash anything.
        public string TitleFor(WarRank rank)
        {
            if (rank == null) return string.Empty;
            if (titles == null || rank.Level < 1 || rank.Level > titles.Length) return rank.Title;
            return titles[rank.Level - 1];
        }
    }

    // the registry of every title set, and the helpers everything else calls to turn a rank into
    // the right words. adding a set is a one-block job - see the note on the array below.
    public static class WarRankTitles
    {
        // ===========================================================================
        // to add a race: copy one of the blocks below, give it a unique Id (no spaces -
        // it's what gets saved), a Label (free text, shown in options), and 24 titles in
        // rank order from Scout up to Grand High Warlord. that's the whole job - the
        // settings screen and everything else pick it up automatically.
        // ===========================================================================
        public static readonly WarRankTitleSet[] All =
        {
            new WarRankTitleSet("Unified", "Unified", new[]
            {
                "Scout", "Private", "Corporal", "Sergeant", "Senior Sergeant", "Master Sergeant",
                "Stone Guard", "Blood Guard", "Knight", "Knight-Lieutenant", "Knight-Captain",
                "Knight-Champion", "Centurion", "Legionnaire", "Champion", "Lieutenant Commander",
                "Commander", "Lieutenant General", "General", "Marshal", "Field Marshal",
                "Warlord", "High Warlord", "Grand High Warlord"
            }),

            new WarRankTitleSet("Sindorei", "Sindorei", new[]
            {
                "Sun Scout", "Sunblade Recruit", "Sunblade Adept", "Spellguard", "Senior Spellguard",
                "Master Spellguard", "Phoenix Guard", "Bloodwarder", "Sun Knight", "Sun Lieutenant",
                "Sun Captain", "Sun Champion", "Magister", "Spellbreaker", "Dawn Champion",
                "Lieutenant Ranger", "Ranger Commander", "Dawn General", "Sun General", "Blood Marshal",
                "Phoenix Marshal", "Sun Warlord", "High Sun Warlord", "Grand Phoenix Warlord"
            }),

            new WarRankTitleSet("Kaldorei", "Kaldorei", new[]
            {
                "Moon Scout", "Sentinel Recruit", "Sentinel", "Glaive Sergeant", "Senior Sentinel",
                "Master Sentinel", "Grove Guard", "Moon Guard", "Moon Knight", "Moon Lieutenant",
                "Moon Captain", "Moon Champion", "Warden", "Sentinel Legionnaire", "Grove Champion",
                "Lieutenant Huntress", "Huntress Commander", "Moon General", "Sentinel General",
                "Moon Marshal", "Field Sentinel", "Grove Warlord", "High Moon Warlord", "Grand Moon Warlord"
            }),

            // ---- template: uncomment, rename, and fill in 24 titles to add your own ----
            // new WarRankTitleSet("Orcish", "Orcish", new[]
            // {
            //     "rank 1", "rank 2", "rank 3", "rank 4", "rank 5", "rank 6",
            //     "rank 7", "rank 8", "rank 9", "rank 10", "rank 11", "rank 12",
            //     "rank 13", "rank 14", "rank 15", "rank 16", "rank 17", "rank 18",
            //     "rank 19", "rank 20", "rank 21", "rank 22", "rank 23", "rank 24"
            // }),
        };

        // the set used when settings haven't loaded yet, or a saved id no longer exists. also the
        // one whose names match the def labels, so it's the natural default.
        public static WarRankTitleSet Default
        {
            get { return All[0]; }
        }

        public static WarRankTitleSet ById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                for (int i = 0; i < All.Length; i++)
                {
                    if (All[i].Id == id) return All[i];
                }
            }
            return Default;
        }

        // whatever the player currently has selected.
        public static WarRankTitleSet Current
        {
            get
            {
                if (WarRanksMod.Settings == null) return Default;
                return ById(WarRanksMod.Settings.TitleSetId);
            }
        }

        // the call the rest of the mod uses - resolves the current set and looks up the rank.
        public static string TitleFor(WarRank rank)
        {
            return Current.TitleFor(rank);
        }

        public static IEnumerable<WarRankTitleSet> AllSets
        {
            get { return All; }
        }
    }
}
