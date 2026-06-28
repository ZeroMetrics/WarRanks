using RimWorld;
using Verse;

namespace WarRanks
{
    // pure read-side helpers: how many "war" kills a pawn has, which rank that maps to, and
    // whether a pawn should be carrying ranks at all. nothing in here mutates a pawn.
    public static class WarRankUtility
    {
        // every rank hediff is named "WarRank_Rank" + a two-digit level. we recognise our own
        // hediffs straight off that, so detection can never depend on some lookup table having
        // been built at the right moment.
        private const string HediffDefPrefix = "WarRank_Rank";

        // the two vanilla kill records we count. resolved once and held - GetNamedSilentFail is a
        // dictionary lookup we'd otherwise repeat for every pawn, every pass.
        private static RecordDef killsHumanlikes;
        private static RecordDef killsMechanoids;
        private static bool recordsResolved;

        private static void EnsureRecords()
        {
            // don't latch "resolved" until we actually found something, so an early call before the
            // core defs are ready can't poison the cache.
            if (recordsResolved) return;
            killsHumanlikes = DefDatabase<RecordDef>.GetNamedSilentFail("KillsHumanlikes");
            killsMechanoids = DefDatabase<RecordDef>.GetNamedSilentFail("KillsMechanoids");
            recordsResolved = killsHumanlikes != null || killsMechanoids != null;
        }

        // humanlike + mechanoid kills straight off the pawn's record card. animals are left out on
        // purpose so hunting and butchering for the larder don't quietly inflate ranks.
        public static int RelevantKills(Pawn pawn)
        {
            if (pawn == null || pawn.records == null) return 0;
            EnsureRecords();
            return ReadRecord(pawn, killsHumanlikes) + ReadRecord(pawn, killsMechanoids);
        }

        // highest rank whose kill requirement is met, or null while still below the first rung.
        public static WarRank RankForKills(int kills)
        {
            WarRank result = null;
            WarRank[] ranks = WarRankData.Ranks;
            for (int i = 0; i < ranks.Length; i++)
            {
                if (kills >= ranks[i].RequiredKills) result = ranks[i];
                else break; // list is sorted, so nothing past here can qualify either
            }
            return result;
        }

        public static WarRank CurrentRank(Pawn pawn)
        {
            return RankForKills(RelevantKills(pawn));
        }

        // is this hediff one of ours? answered purely from the defName, no state involved.
        public static bool IsWarRankHediffDef(HediffDef hediffDef)
        {
            return hediffDef != null && hediffDef.defName != null
                && hediffDef.defName.StartsWith(HediffDefPrefix, System.StringComparison.Ordinal);
        }

        // map a rank hediff back to its rank by reading the level out of the defName.
        public static WarRank RankForHediffDef(HediffDef hediffDef)
        {
            if (!IsWarRankHediffDef(hediffDef)) return null;

            int level;
            if (!int.TryParse(hediffDef.defName.Substring(HediffDefPrefix.Length), out level)) return null;

            WarRank[] ranks = WarRankData.Ranks;
            // fast path: the table is in level order, so the index usually lines up.
            if (level >= 1 && level <= ranks.Length && ranks[level - 1].Level == level)
                return ranks[level - 1];
            // fallback in case the table is ever reordered.
            for (int i = 0; i < ranks.Length; i++)
                if (ranks[i].Level == level) return ranks[i];
            return null;
        }

        // who's allowed to hold ranks: a spawned, living, humanlike member of the player faction
        // who isn't a prisoner. slaves, quest lodgers and guests drop out on the faction check.
        public static bool IsEligibleColonist(Pawn pawn)
        {
            return pawn != null
                && pawn.Spawned
                && !pawn.Dead
                && pawn.health != null
                && pawn.RaceProps != null
                && pawn.RaceProps.Humanlike
                && pawn.Faction == Faction.OfPlayer
                && !pawn.IsPrisoner;
        }

        // not used by the runtime loop - handy one-liner for the debug log / dev inspector.
        public static string RankSummary(Pawn pawn)
        {
            int kills = RelevantKills(pawn);
            WarRank rank = RankForKills(kills);
            if (rank == null) return "War rank: none (" + kills + " relevant kills)";
            return "War rank: " + WarRankTitles.TitleFor(rank) + " (" + kills + " relevant kills)";
        }

        private static int ReadRecord(Pawn pawn, RecordDef recordDef)
        {
            if (recordDef == null) return 0;
            return (int)pawn.records.GetValue(recordDef);
        }
    }
}
