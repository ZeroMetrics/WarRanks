using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WarRanks
{
    // the write side: make sure each eligible colonist carries exactly the one hediff for their
    // current rank, and strip ranks off anyone who no longer qualifies.
    public static class WarRankHediffUtility
    {
        // bring a single pawn's rank hediff in line with their kill count.
        // announce=false is used for the silent catch-up right after a save loads, so veterans
        // don't flood the message log with "promotions" they actually earned hours ago.
        public static void UpdatePawnRank(Pawn pawn, bool announce)
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

            // remember what they had first, so we can tell a real promotion from a no-op.
            Hediff oldHediff = CurrentWarRankHediff(pawn);
            WarRank oldRank = oldHediff == null ? null : WarRankUtility.RankForHediffDef(oldHediff.def);

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
            {
                pawn.health.AddHediff(HediffMaker.MakeHediff(desiredDef, pawn));
                if (announce) NotifyRankChanged(pawn, oldRank, desiredRank);
            }
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

        // only shout about genuine promotions. demotions can't really happen (kills never go
        // down), but the guard also covers the odd case where we already knew about an
        // equal-or-higher rank and shouldn't re-announce it.
        private static void NotifyRankChanged(Pawn pawn, WarRank oldRank, WarRank newRank)
        {
            if (pawn == null || newRank == null) return;
            if (oldRank != null && oldRank.Level >= newRank.Level) return;

            string title = WarRankTitles.TitleFor(newRank);
            Messages.Message(
                pawn.LabelShortCap + " has reached rank " + newRank.Level + ": " + title + ".",
                pawn, MessageTypeDefOf.PositiveEvent, true);
        }
    }
}
