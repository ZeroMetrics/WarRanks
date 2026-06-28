using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WarRanks
{
    // drives the mod at runtime. instead of harmony-patching the tick loop (which fires on every
    // single game tick and trips a lot of profilers) we ride the game's own component tick, then
    // only do real work on a slow interval. rimworld instantiates this automatically for every
    // game because it has the (Game) constructor.
    public class GameComponent_WarRanks : GameComponent
    {
        // ~16 seconds of game time at 1x. ranks creep up over hundreds of kills, so checking more
        // often would just burn cpu for no visible difference.
        private const int UpdateIntervalTicks = 1000;

        // the first sweep after a game loads runs silently, so a colony full of veterans doesn't
        // dump two dozen "promotion" messages the moment you press play. not saved on purpose - we
        // want this to happen fresh on every load.
        private bool caughtUp;

        // each colonist's last-known rank level (0 = none). this is what decides whether to
        // announce: we only shout when a pawn's level actually goes UP from what we last saw, so a
        // hediff that gets stripped and re-added by some other mod can't re-trigger the message.
        // in-memory only - it's rebuilt silently every load.
        private static readonly Dictionary<Pawn, int> lastLevel = new Dictionary<Pawn, int>();

        // reused between sweeps so we're not allocating every interval. main-thread only.
        private static readonly List<Pawn> scratch = new List<Pawn>();
        private static readonly List<Pawn> stale = new List<Pawn>();

        public GameComponent_WarRanks(Game game)
        {
        }

        // also catch up here, so ranks and overlay pips are right even when the player loads
        // straight into a paused game and never lets a tick go by before clicking around.
        public override void FinalizeInit()
        {
            if (caughtUp) return;
            if (Find.Maps == null || Find.Maps.Count == 0) return; // no maps yet - let the tick handle it
            UpdateAllColonists(false);
            caughtUp = true;
        }

        public override void GameComponentTick()
        {
            if (!caughtUp)
            {
                UpdateAllColonists(false);
                caughtUp = true;
                return;
            }

            if (Find.TickManager.TicksGame % UpdateIntervalTicks != 0) return;
            UpdateAllColonists(true);
        }

        private static void UpdateAllColonists(bool announce)
        {
            List<Map> maps = Find.Maps;
            if (maps == null) return;

            ForgetDeadPawns();

            for (int m = 0; m < maps.Count; m++)
            {
                Map map = maps[m];
                if (map == null || map.mapPawns == null) continue;

                // refresh ranks for current colonists. copy the list first because adding or
                // removing a hediff can poke the map's pawn lists, and mutating the list you're
                // iterating is how you earn an InvalidOperationException.
                scratch.Clear();
                scratch.AddRange(map.mapPawns.FreeColonistsSpawned);
                for (int i = 0; i < scratch.Count; i++)
                    ProcessColonist(scratch[i], announce);

                // strip ranks off anyone who used to be a colonist but isn't now (captured, sold,
                // turned prisoner...) yet is still wearing a rank hediff. eligible colonists were
                // handled above, so the guard skips them here.
                scratch.Clear();
                scratch.AddRange(map.mapPawns.SpawnedPawnsWithAnyHediff);
                for (int i = 0; i < scratch.Count; i++)
                {
                    Pawn pawn = scratch[i];
                    if (!WarRankUtility.IsEligibleColonist(pawn) && WarRankHediffUtility.HasWarRank(pawn))
                        WarRankHediffUtility.SyncPawnRank(pawn);
                }
            }

            scratch.Clear(); // don't leave pawn refs pinned in the static between sweeps
        }

        private static void ProcessColonist(Pawn pawn, bool announce)
        {
            WarRank rank = WarRankUtility.CurrentRank(pawn);
            int newLevel = rank == null ? 0 : rank.Level;

            int prevLevel;
            bool known = lastLevel.TryGetValue(pawn, out prevLevel);

            // announce only on a real promotion, and only for pawns we were already tracking - a
            // pawn we're seeing for the first time (just loaded, or freshly recruited with kills
            // already on their record) is baselined silently so we don't trumpet old history.
            if (announce && known && newLevel > prevLevel)
                AnnouncePromotion(pawn, rank);

            lastLevel[pawn] = newLevel;
            WarRankHediffUtility.SyncPawnRank(pawn);
        }

        private static void AnnouncePromotion(Pawn pawn, WarRank rank)
        {
            if (pawn == null || rank == null) return;
            // the message itself is faction-flavoured by the current title set.
            string text = WarRankTitles.PromotionMessage(pawn.LabelShortCap, rank);
            Messages.Message(text, pawn, MessageTypeDefOf.PositiveEvent, true);
        }

        // keep the tracking dictionary from pinning dead/destroyed pawns in memory forever.
        private static void ForgetDeadPawns()
        {
            stale.Clear();
            foreach (KeyValuePair<Pawn, int> entry in lastLevel)
            {
                Pawn pawn = entry.Key;
                if (pawn == null || pawn.Destroyed || pawn.Dead) stale.Add(pawn);
            }
            for (int i = 0; i < stale.Count; i++) lastLevel.Remove(stale[i]);
            stale.Clear();
        }
    }
}
