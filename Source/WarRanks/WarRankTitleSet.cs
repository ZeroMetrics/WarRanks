using System.Collections.Generic;

namespace WarRanks
{
    // which set of flavour names to show. purely cosmetic - the ladder, thresholds and buffs are
    // identical across all three, only the words change.
    public enum WarRankTitleSet
    {
        Unified,   // the plain military spread used as the def labels
        Sindorei,  // blood-elf flavour
        Kaldorei   // night-elf flavour
    }

    // maps a rank (and the chosen set) to the string the player actually sees. each array is the
    // 24 titles in rank order; index = rank.Level - 1.
    public static class WarRankTitles
    {
        private static readonly Dictionary<WarRankTitleSet, string[]> Titles =
            new Dictionary<WarRankTitleSet, string[]>
        {
            {
                WarRankTitleSet.Unified, new[]
                {
                    "Scout", "Private", "Corporal", "Sergeant", "Senior Sergeant", "Master Sergeant",
                    "Stone Guard", "Blood Guard", "Knight", "Knight-Lieutenant", "Knight-Captain",
                    "Knight-Champion", "Centurion", "Legionnaire", "Champion", "Lieutenant Commander",
                    "Commander", "Lieutenant General", "General", "Marshal", "Field Marshal",
                    "Warlord", "High Warlord", "Grand High Warlord"
                }
            },
            {
                WarRankTitleSet.Sindorei, new[]
                {
                    "Sun Scout", "Sunblade Recruit", "Sunblade Adept", "Spellguard", "Senior Spellguard",
                    "Master Spellguard", "Phoenix Guard", "Bloodwarder", "Sun Knight", "Sun Lieutenant",
                    "Sun Captain", "Sun Champion", "Magister", "Spellbreaker", "Dawn Champion",
                    "Lieutenant Ranger", "Ranger Commander", "Dawn General", "Sun General", "Blood Marshal",
                    "Phoenix Marshal", "Sun Warlord", "High Sun Warlord", "Grand Phoenix Warlord"
                }
            },
            {
                WarRankTitleSet.Kaldorei, new[]
                {
                    "Moon Scout", "Sentinel Recruit", "Sentinel", "Glaive Sergeant", "Senior Sentinel",
                    "Master Sentinel", "Grove Guard", "Moon Guard", "Moon Knight", "Moon Lieutenant",
                    "Moon Captain", "Moon Champion", "Warden", "Sentinel Legionnaire", "Grove Champion",
                    "Lieutenant Huntress", "Huntress Commander", "Moon General", "Sentinel General",
                    "Moon Marshal", "Field Sentinel", "Grove Warlord", "High Moon Warlord", "Grand Moon Warlord"
                }
            }
        };

        // title using whatever set the player has selected (or Unified if settings aren't up yet).
        public static string TitleFor(WarRank rank)
        {
            WarRankTitleSet set = WarRanksMod.Settings == null
                ? WarRankTitleSet.Unified
                : WarRanksMod.Settings.TitleSet;
            return TitleFor(rank, set);
        }

        public static string TitleFor(WarRank rank, WarRankTitleSet titleSet)
        {
            if (rank == null) return string.Empty;

            string[] titles;
            // fall back to the rank's built-in name if the set is somehow missing or short.
            if (!Titles.TryGetValue(titleSet, out titles) || titles == null
                || rank.Level < 1 || rank.Level > titles.Length)
                return rank.Title;

            return titles[rank.Level - 1];
        }

        // label shown next to the radio button in mod settings.
        public static string LabelFor(WarRankTitleSet titleSet)
        {
            switch (titleSet)
            {
                case WarRankTitleSet.Sindorei: return "Sindorei";
                case WarRankTitleSet.Kaldorei: return "Kaldorei";
                default: return "Unified";
            }
        }

        public static IEnumerable<WarRankTitleSet> AllTitleSets
        {
            get
            {
                yield return WarRankTitleSet.Unified;
                yield return WarRankTitleSet.Sindorei;
                yield return WarRankTitleSet.Kaldorei;
            }
        }
    }
}
