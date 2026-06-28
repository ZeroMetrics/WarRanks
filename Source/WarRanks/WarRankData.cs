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

    // the full 24-rank ladder, low to high. the kill requirement starts at a few for the first
    // rung and climbs on an accelerating curve to 1000 for the top - a real long-haul goal. the
    // lookup code leans on RequiredKills being strictly increasing, so keep it sorted if you ever
    // reorder rows.
    public static class WarRankData
    {
        public static readonly WarRank[] Ranks =
        {
            //          lvl  title                      kills
            new WarRank(  1, "Scout",                       3),
            new WarRank(  2, "Private",                    10),
            new WarRank(  3, "Corporal",                   20),
            new WarRank(  4, "Sergeant",                   35),
            new WarRank(  5, "Senior Sergeant",            50),
            new WarRank(  6, "Master Sergeant",            70),
            new WarRank(  7, "Stone Guard",                95),
            new WarRank(  8, "Blood Guard",               120),
            new WarRank(  9, "Knight",                    150),
            new WarRank( 10, "Knight-Lieutenant",         185),
            new WarRank( 11, "Knight-Captain",            220),
            new WarRank( 12, "Knight-Champion",           260),
            new WarRank( 13, "Centurion",                 305),
            new WarRank( 14, "Legionnaire",               350),
            new WarRank( 15, "Champion",                  400),
            new WarRank( 16, "Lieutenant Commander",      455),
            new WarRank( 17, "Commander",                 510),
            new WarRank( 18, "Lieutenant General",        570),
            new WarRank( 19, "General",                   635),
            new WarRank( 20, "Marshal",                   700),
            new WarRank( 21, "Field Marshal",             770),
            new WarRank( 22, "Warlord",                   845),
            new WarRank( 23, "High Warlord",              920),
            new WarRank( 24, "Grand High Warlord",       1000),
        };
    }
}
