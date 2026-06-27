namespace WarRanks
{
    // one rung on the ladder. the buff numbers here mirror what's baked into
    // Defs/Hediffs/WarRank_Hediffs.xml - the xml is what the game actually applies to a pawn,
    // but keeping the values next to the kill thresholds means the whole rank table reads in
    // one place, and it's what i regenerate the def file from whenever i touch balance.
    public sealed class WarRank
    {
        public readonly int Level;
        public readonly string Title;
        public readonly int RequiredKills;

        // mirrors of the xml stage values, kept for reference / regeneration (see note above).
        public readonly float SocialImpact;
        public readonly float PainShockThreshold;
        public readonly float MentalBreakThreshold;
        public readonly float ShootingAccuracy;
        public readonly float MeleeHitChance;
        public readonly float Manipulation;
        public readonly float Sight;

        public WarRank(int level, string title, int requiredKills,
            float socialImpact, float painShockThreshold, float mentalBreakThreshold,
            float shootingAccuracy, float meleeHitChance, float manipulation, float sight)
        {
            Level = level;
            Title = title;
            RequiredKills = requiredKills;
            SocialImpact = socialImpact;
            PainShockThreshold = painShockThreshold;
            MentalBreakThreshold = mentalBreakThreshold;
            ShootingAccuracy = shootingAccuracy;
            MeleeHitChance = meleeHitChance;
            Manipulation = manipulation;
            Sight = sight;
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

    // the full 24-rank ladder, low to high. the lookup code leans on RequiredKills being
    // strictly increasing down the list, so keep it sorted if you ever add or reorder rows.
    public static class WarRankData
    {
        public static readonly WarRank[] Ranks =
        {
            //          lvl  title                      kills  social pain    mental  shoot  melee  manip  sight
            new WarRank(  1, "Scout",                       5, 0.02f, 0f,     0f,     0f,    0f,    0f,    0f),
            new WarRank(  2, "Private",                    20, 0.03f, 0f,     0f,     0f,    0f,    0f,    0f),
            new WarRank(  3, "Corporal",                   45, 0.04f, 0f,     0f,     0f,    0f,    0f,    0f),
            new WarRank(  4, "Sergeant",                   80, 0.05f, 0.02f,  0f,     0f,    0f,    0f,    0f),
            new WarRank(  5, "Senior Sergeant",           125, 0.06f, 0.04f,  0f,     0f,    0f,    0f,    0f),
            new WarRank(  6, "Master Sergeant",           180, 0.08f, 0.06f,  0f,     0f,    0f,    0f,    0f),
            new WarRank(  7, "Stone Guard",               245, 0.09f, 0.08f, -0.01f,  0f,    0f,    0f,    0f),
            new WarRank(  8, "Blood Guard",               325, 0.10f, 0.10f, -0.02f,  0f,    0f,    0f,    0f),
            new WarRank(  9, "Knight",                    420, 0.12f, 0.12f, -0.03f,  0f,    0f,    0f,    0f),
            new WarRank( 10, "Knight-Lieutenant",         530, 0.13f, 0.14f, -0.04f,  0f,    0f,    0f,    0f),
            new WarRank( 11, "Knight-Captain",            660, 0.14f, 0.16f, -0.05f,  0f,    0f,    0f,    0f),
            new WarRank( 12, "Knight-Champion",           810, 0.16f, 0.19f, -0.06f,  0.02f, 0.02f, 0.01f, 0.01f),
            new WarRank( 13, "Centurion",                 980, 0.17f, 0.22f, -0.07f,  0.02f, 0.02f, 0.02f, 0.01f),
            new WarRank( 14, "Legionnaire",              1165, 0.18f, 0.25f, -0.08f,  0.03f, 0.03f, 0.02f, 0.02f),
            new WarRank( 15, "Champion",                 1365, 0.20f, 0.28f, -0.09f,  0.03f, 0.03f, 0.03f, 0.02f),
            new WarRank( 16, "Lieutenant Commander",     1575, 0.21f, 0.31f, -0.10f,  0.04f, 0.04f, 0.04f, 0.03f),
            new WarRank( 17, "Commander",                1790, 0.22f, 0.34f, -0.11f,  0.04f, 0.04f, 0.04f, 0.04f),
            new WarRank( 18, "Lieutenant General",       2000, 0.24f, 0.37f, -0.12f,  0.05f, 0.05f, 0.05f, 0.04f),
            new WarRank( 19, "General",                  2200, 0.25f, 0.40f, -0.13f,  0.06f, 0.06f, 0.06f, 0.05f),
            new WarRank( 20, "Marshal",                  2390, 0.27f, 0.43f, -0.14f,  0.07f, 0.07f, 0.07f, 0.06f),
            new WarRank( 21, "Field Marshal",            2565, 0.29f, 0.46f, -0.16f,  0.08f, 0.08f, 0.08f, 0.07f),
            new WarRank( 22, "Warlord",                  2725, 0.31f, 0.48f, -0.18f,  0.09f, 0.09f, 0.09f, 0.08f),
            new WarRank( 23, "High Warlord",             2875, 0.33f, 0.49f, -0.19f,  0.09f, 0.09f, 0.09f, 0.09f),
            new WarRank( 24, "Grand High Warlord",       3000, 0.35f, 0.50f, -0.20f,  0.10f, 0.10f, 0.10f, 0.10f),
        };
    }
}
