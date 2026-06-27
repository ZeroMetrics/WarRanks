using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WarRanks
{
    // pure read-side helpers: how many "war" kills a pawn has, which rank that maps to, and
    // whether a pawn should be carrying ranks at all. nothing in here mutates a pawn.
    public static class WarRankUtility
    {
        // the two vanilla kill records we count. resolved once and held - GetNamedSilentFail
        // is a dictionary lookup we'd otherwise repeat for every pawn, every pass.
        private static RecordDef killsHumanlikes;
        private static RecordDef killsMechanoids;
        private static bool recordsResolved;

        // hediffDef -> rank, plus a quick "is this one of ours" set. both are built lazily the
        // first time they're needed (well after defs have finished loading) so we can map a
        // hediff straight back to its rank without re-scanning the 24-row table each call.
        private static Dictionary<HediffDef, WarRank> rankByHediff;
        private static HashSet<HediffDef> warRankHediffDefs;

        private static void EnsureLookups()
        {
            if (rankByHediff != null) return;

            rankByHediff = new Dictionary<HediffDef, WarRank>();
            warRankHediffDefs = new HashSet<HediffDef>();

            for (int i = 0; i < WarRankData.Ranks.Length; i++)
            {
                WarRank rank = WarRankData.Ranks[i];
                HediffDef def = DefDatabase<HediffDef>.GetNamedSilentFail(rank.HediffDefName);
                if (def == null) continue; // def file missing this row - skip it rather than blow up
                rankByHediff[def] = rank;
                warRankHediffDefs.Add(def);
            }
        }

        private static void EnsureRecords()
        {
            if (recordsResolved) return;
            killsHumanlikes = DefDatabase<RecordDef>.GetNamedSilentFail("KillsHumanlikes");
            killsMechanoids = DefDatabase<RecordDef>.GetNamedSilentFail("KillsMechanoids");
            recordsResolved = true;
        }

        // humanlike + mechanoid kills straight off the pawn's record card. animals are left out
        // on purpose so hunting and butchering for the larder don't quietly inflate ranks.
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

        public static WarRank RankForHediffDef(HediffDef hediffDef)
        {
            if (hediffDef == null) return null;
            EnsureLookups();
            WarRank rank;
            return rankByHediff.TryGetValue(hediffDef, out rank) ? rank : null;
        }

        public static bool IsWarRankHediffDef(HediffDef hediffDef)
        {
            if (hediffDef == null) return false;
            EnsureLookups();
            return warRankHediffDefs.Contains(hediffDef);
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
