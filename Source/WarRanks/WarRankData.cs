namespace WarRanks
{
    // one rung on the ladder: a level, the fallback (Unified) title, and how many relevant kills
    // it takes to earn. the actual stat buffs live in Defs/Hediffs/WarRank_Hediffs.xml - that's
    // what the game applies - so they're not duplicated here.
    public sealed class WarRank
    {
        public readonly int Level;
        public readonly string Title;
        public readonly int RequiredKills;

        public WarRank(int level, string title, int requiredKills)
        {
            Level = level;
            Title = title;
            RequiredKills = requiredKills;
        }

        // defName of the matching hediff, e.g. rank 4 -> "WarRank_Rank04".
        public string HediffDefName
        {
            get { return "WarRank_Rank" + Level.ToString("00"); }
        }

        // texture path under Textures/, e.g. "RankIcons/Rank04".
        public string IconPath
        {
            get { return "RankIcons/Rank" + Level.ToString("00"); }
        }
    }

    // the full 24-rank ladder, low to high. the kill requirement follows level x (level + 1):
    // a couple of kills for the first rung, climbing to 600 for the top. the lookup code leans on
    // RequiredKills being strictly increasing, so keep it sorted if you ever reorder rows.
    public static class WarRankData
    {
        public static readonly WarRank[] Ranks =
        {
            //          lvl  title                      kills
            new WarRank(  1, "Scout",                       2),
            new WarRank(  2, "Private",                     6),
            new WarRank(  3, "Corporal",                   12),
            new WarRank(  4, "Sergeant",                   20),
            new WarRank(  5, "Senior Sergeant",            30),
            new WarRank(  6, "Master Sergeant",            42),
            new WarRank(  7, "Stone Guard",                56),
            new WarRank(  8, "Blood Guard",                72),
            new WarRank(  9, "Knight",                     90),
            new WarRank( 10, "Knight-Lieutenant",         110),
            new WarRank( 11, "Knight-Captain",            132),
            new WarRank( 12, "Knight-Champion",           156),
            new WarRank( 13, "Centurion",                 182),
            new WarRank( 14, "Legionnaire",               210),
            new WarRank( 15, "Champion",                  240),
            new WarRank( 16, "Lieutenant Commander",      272),
            new WarRank( 17, "Commander",                 306),
            new WarRank( 18, "Lieutenant General",        342),
            new WarRank( 19, "General",                   380),
            new WarRank( 20, "Marshal",                   420),
            new WarRank( 21, "Field Marshal",             462),
            new WarRank( 22, "Warlord",                   506),
            new WarRank( 23, "High Warlord",              552),
            new WarRank( 24, "Grand High Warlord",        600),
        };
    }
}
