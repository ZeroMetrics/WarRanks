using System.Collections.Generic;
using Verse;

namespace WarRanks
{
    // drives the mod at runtime. instead of harmony-patching the tick loop (which fires on every
    // single game tick and trips a lot of profilers) we ride the game's own component tick, then
    // only do real work on a slow interval. rimworld instantiates this automatically for every
    // game because it has the (Game) constructor.
    public class GameComponent_WarRanks : GameComponent
    {
        // ~16 seconds of game time at 1x. ranks creep up over hundreds of kills, so checking
        // more often would just burn cpu for no visible difference.
        private const int UpdateIntervalTicks = 1000;

        // the first sweep after a game loads runs silently, so a colony full of veterans doesn't
        // dump two dozen "promotion" messages the moment you press play. not saved on purpose -
        // we want this to happen fresh on every load.
        private bool caughtUp;

        // reused between sweeps so we're not allocating a list every interval. only ever touched
        // on the main thread.
        private static readonly List<Pawn> scratch = new List<Pawn>();

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

            for (int m = 0; m < maps.Count; m++)
            {
                Map map = maps[m];
                if (map == null || map.mapPawns == null) continue;

                // refresh ranks for the current colonists. we copy the list first because adding
                // or removing a hediff can poke the map's pawn lists, and mutating the very list
                // you're iterating is how you earn an InvalidOperationException.
                scratch.Clear();
                scratch.AddRange(map.mapPawns.FreeColonistsSpawned);
                for (int i = 0; i < scratch.Count; i++)
                    WarRankHediffUtility.UpdatePawnRank(scratch[i], announce);

                // second pass strips ranks off anyone who used to be a colonist but isn't now
                // (captured, sold, turned prisoner...) yet is still wearing a rank hediff.
                // eligible colonists were handled above, so the guard skips them here.
                scratch.Clear();
                scratch.AddRange(map.mapPawns.SpawnedPawnsWithAnyHediff);
                for (int i = 0; i < scratch.Count; i++)
                {
                    Pawn pawn = scratch[i];
                    if (!WarRankUtility.IsEligibleColonist(pawn) && WarRankHediffUtility.HasWarRank(pawn))
                        WarRankHediffUtility.UpdatePawnRank(pawn, announce);
                }
            }

            scratch.Clear(); // don't leave pawn refs pinned in the static between sweeps
        }
    }
}
