using System.Collections.Generic;
using Verse;

namespace WarRanks
{
    // the write side: make sure each eligible colonist carries exactly the one hediff for their
    // current rank, and strip ranks off anyone who no longer qualifies. announcing promotions is
    // NOT done here - that's the game component's job, so it can decide once, based on an actual
    // rank increase, rather than every time a hediff happens to get re-added.
    public static class WarRankHediffUtility
    {
        // bring a single pawn's rank hediff in line with their kill count.
        public static void SyncPawnRank(Pawn pawn)
        {
            if (!WarRankUtility.IsEligibleColonist(pawn))
            {
                RemoveAllWarRankHediffs(pawn);
                return;
            }

            WarRank desiredRank = WarRankUtility.CurrentRank(pawn);
            HediffDef desiredDef = desiredRank == null
                ? null
                : DefDatabase<HediffDef>.GetNamedSilentFail(desiredRank.HediffDefName);

            // one backward pass: hang on to the hediff we want if it's already there, drop every
            // other rank hediff. going backwards keeps RemoveHediff from shifting indices we've
            // yet to visit.
            Hediff existingDesired = null;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = hediffs.Count - 1; i >= 0; i--)
            {
                Hediff hediff = hediffs[i];
                if (!WarRankUtility.IsWarRankHediffDef(hediff.def)) continue;

                if (desiredDef != null && hediff.def == desiredDef && existingDesired == null)
                    existingDesired = hediff;
                else
                    pawn.health.RemoveHediff(hediff);
            }

            // add the wanted rank if it wasn't already on them.
            if (desiredDef != null && existingDesired == null)
                pawn.health.AddHediff(HediffMaker.MakeHediff(desiredDef, pawn));
        }

        public static bool HasWarRank(Pawn pawn)
        {
            return CurrentWarRankHediff(pawn) != null;
        }

        // first rank hediff on the pawn, or null. in normal play a pawn only ever has one.
        public static Hediff CurrentWarRankHediff(Pawn pawn)
        {
            if (pawn == null || pawn.health == null || pawn.health.hediffSet == null) return null;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                if (WarRankUtility.IsWarRankHediffDef(hediffs[i].def)) return hediffs[i];
            }
            return null;
        }

        private static void RemoveAllWarRankHediffs(Pawn pawn)
        {
            if (pawn == null || pawn.health == null || pawn.health.hediffSet == null) return;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = hediffs.Count - 1; i >= 0; i--)
            {
                if (WarRankUtility.IsWarRankHediffDef(hediffs[i].def))
                    pawn.health.RemoveHediff(hediffs[i]);
            }
        }
    }
}
