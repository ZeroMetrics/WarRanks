using System.Collections.Generic;

namespace WarRanks
{
    // one named set of rank titles - e.g. the plain military set, or a race-flavoured one.
    // purely cosmetic: the ladder, kill thresholds and buffs are identical no matter which set
    // is picked, only the words shown to the player change.
    public sealed class WarRankTitleSet
    {
        public readonly string Id;     // stable key saved in settings - don't rename once shipped
        public readonly string Label;  // what shows next to the radio button in mod options
        private readonly string[] titles; // 24 names, low rank to high; index = rank.Level - 1
        private readonly string[] descriptions; // optional, same indexing as titles; null = use the def's generic text

        public WarRankTitleSet(string id, string label, string[] titles, string[] descriptions = null)
        {
            Id = id;
            Label = label;
            this.titles = titles;
            this.descriptions = descriptions;
        }

        // name for a given rank in this set. falls back to the rank's built-in name if the array
        // is short or missing, so a half-finished set can't crash anything.
        public string TitleFor(WarRank rank)
        {
            if (rank == null) return string.Empty;
            if (titles == null || rank.Level < 1 || rank.Level > titles.Length) return rank.Title;
            return titles[rank.Level - 1];
        }

        // flavour text for a rank in this set, or null if this set hasn't been written one yet
        // (in which case the hediff just keeps its plain def description).
        public string DescriptionFor(WarRank rank)
        {
            if (rank == null || descriptions == null) return null;
            if (rank.Level < 1 || rank.Level > descriptions.Length) return null;
            return descriptions[rank.Level - 1];
        }
    }

    // the registry of every title set, and the helpers everything else calls to turn a rank into
    // the right words. adding a set is a one-block job - see the note on the array below.
    public static class WarRankTitles
    {
        // ===========================================================================
        // to add a set: copy one of the blocks below, give it a unique Id (no spaces -
        // it's what gets saved), a Label (free text, shown in options), and 24 titles in
        // rank order from rank 1 up to rank 24. the settings screen and everything else
        // pick it up automatically, nothing else to touch.
        // ===========================================================================
        public static readonly WarRankTitleSet[] All =
        {
            new WarRankTitleSet("Unified", "Unified", new[]
            {
                "Scout", "Private", "Corporal", "Sergeant", "Senior Sergeant", "Master Sergeant",
                "Stone Guard", "Blood Guard", "Knight", "Knight-Lieutenant", "Knight-Captain",
                "Knight-Champion", "Centurion", "Legionnaire", "Champion", "Lieutenant Commander",
                "Commander", "Lieutenant General", "General", "Marshal", "Field Marshal",
                "Warlord", "High Warlord", "Grand High Warlord"
            }),

            new WarRankTitleSet("Kaldorei", "Kaldorei / Night Elf", new[]
            {
                "Ashen Scout", "Grove Watcher", "Huntress", "Moonlit Outrider", "Silverwing Scout",
                "Grove Sentinel", "Silverleaf Sentinel", "Sentinel Lieutenant", "Sentinel Captain",
                "Shadowstalker", "Nightsaber Rider", "Ranger", "High Ranger", "Moonblade",
                "Moonblade Captain", "Warden Initiate", "Warden", "Archwarden", "Ancient Protector",
                "Wildhunt Champion", "Blade of Elune", "Wrath of the Moon", "Avatar of Vengeance",
                "Chosen of Elune"
            }),

            new WarRankTitleSet("Sindorei", "Sin'dorei / Blood Elf", new[]
            {
                "Scout", "Arcane Initiate", "Sunblade Recruit", "Spellguard", "Farstrider",
                "Veteran Farstrider", "Sunfury Adept", "Blood Knight", "Blood Knight Captain",
                "Blood Knight Champion", "Magister's Elite", "Sunreaver", "Phoenix Guard",
                "Phoenix Elite", "Sunwell Defender", "Arcane Champion", "High Spellbreaker",
                "Dawnblade Commander", "Sunfury Commander", "Blood Knight Lord", "Magister Ascendant",
                "Phoenix Lord", "Sunwell's Chosen", "Phoenix Ascendant"
            }),

            new WarRankTitleSet("Forsaken", "Forsaken / Undead", new[]
            {
                "Grave Fodder", "Rotguard", "Deathguard", "Dark Ranger", "Blightcaller",
                "Grave Warden", "Apothecary's Hand", "Plague Lord", "Banshee's Guard", "Dreadblade",
                "Deathdealer", "Crypt Champion", "Deathstalker", "Royal Apothecary", "Banshee Champion",
                "Plague Marshal", "Dread Commander", "Deathguard Lord", "Shadow Apothecary",
                "Lord of Blight", "Banshee's Chosen", "Champion of the Forsaken",
                "Herald of the Banshee Queen", "Hand of Ruin"
            }),

            new WarRankTitleSet("Orc", "Orc / Horde", new[]
            {
                "Blooded", "Warscout", "Warrior", "Raider", "Wolf Rider", "Clan Guard",
                "Blademaster's Disciple", "Berserker", "Warsong Outrider", "Warbringer", "Clan Champion",
                "Kor'kron", "Blademaster", "War Captain", "Horde Champion", "War Commander",
                "Chieftain's Blade", "Warchief's Guard", "Conqueror", "Overlord", "Warlord",
                "High Overlord", "High Warlord", "Hand of the Warchief"
            }),

            new WarRankTitleSet("Human", "Human / Alliance", new[]
            {
                "Militia Scout", "Footman", "Shieldbearer", "Sergeant", "Veteran Footman",
                "Knight Aspirant", "Knight", "Knight-Lieutenant", "Knight-Captain", "Knight-Champion",
                "Lionguard", "Stormwind Champion", "Lieutenant Commander", "Commander", "Lord Commander",
                "Marshal", "High Marshal", "Field Marshal", "Grand Marshal", "Lionheart Commander",
                "King's Shield", "Alliance Vanguard", "Champion of Stormwind", "Lion of the Alliance"
            }),

            new WarRankTitleSet("Illidari", "Illidari / Demon Hunter", new[]
            {
                "Outcast", "Initiate Hunter", "Felblade", "Glaivebearer", "Demon Stalker", "Felblooded",
                "Soulhunter", "Illidari Adept", "Illidari Guard", "Illidari Veteran", "Fel Champion",
                "Demonbane", "Warglaive Master", "Chaos Reaver", "Nether Stalker", "Fel Commander",
                "Slayer of Demons", "Illidari Champion", "Vengeful Blade", "Herald of Chaos", "Wrathbound",
                "Chosen of the Betrayer", "Lord of the Hunt", "The Betrayer's Wrath"
            }),

            new WarRankTitleSet("Scourge", "Scourge / Death Knight", new[]
            {
                "Gravebound", "Boneguard", "Runeblade", "Frostblade", "Deathcharger", "Acherus Guard",
                "Plague Knight", "Blood Knight", "Frost Knight", "Unholy Champion", "Runeblade Champion",
                "Deathbringer", "Scourge Captain", "Reaper", "Bonelord", "Deathlord's Hand",
                "Frostbound Commander", "Champion of Acherus", "Rune Lord", "Lord of Bones",
                "Herald of Death", "High Deathlord", "Wrath of Acherus", "The Lich King's Champion"
            },
            // the scourge arc: a corpse dragged back into service that slowly remembers how to
            // kill, masters the death knight disciplines, then climbs to command the dead outright.
            new[]
            {
                "Dragged back from death and bound in unliving service. No thought yet, only the Lich King's cold pull and a blade that never tires.",
                "The first sparks of obedience. Set to guard the dead halls of Acherus, this one has learned to hold a line and to feel nothing while doing it.",
                "Granted a runeblade and the first whispers of how to wield it. The weapon drinks what it kills, and slowly, so does its bearer.",
                "Frost answers the call now. Wounds knit slower in those who face this one, the cold gnawing at them long after the strike.",
                "Given a deathcharger and the freedom to range ahead of the host. Speed and a steel-shod skull make for an ugly first meeting.",
                "Trusted to defend the Ebon Hold itself. Recognised among the dead now, if not yet feared by the living.",
                "Has learned to carry the blight, spreading sickness with every blow and letting it do the slow killing afterward.",
                "A student of the crimson arts, tearing the strength from the living to knit their own ruined flesh mid-battle.",
                "Frost has become a discipline rather than a trick. Storms of ice herald this one's arrival, and the cold stays behind.",
                "Commands the diseased and the risen. Where this knight walks, the freshly fallen stand back up to serve.",
                "The runeblade is now an extension of the arm. Few among the Scourge can match this one's bladework.",
                "No longer merely a soldier of death but a deliverer of it, sent wherever the Lich King wants a field emptied.",
                "Given command over the lesser dead, a voice the ghouls and abominations obey without hesitation.",
                "Counts the living as a harvest. Whole companies have been cut down and raised again under this one's watch.",
                "Holds dominion over the bones of the slain, weaving them into guardians and weapons with a thought.",
                "Trusted to act in a deathlord's stead. Orders this knight carries land with the deathlord's full weight behind them.",
                "Leads frost-wreathed columns of the dead. Battlefields freeze solid where this commander chooses to deploy.",
                "The chosen blade of the Ebon Hold, sent to settle the matters lesser knights cannot.",
                "Master of the runes that bind power into steel and flesh alike. Even other death knights step aside.",
                "An army of the dead answers this lord; the fallen of a hundred fields march at a single gesture.",
                "Arrives ahead of the host as a warning given flesh. To see this herald is to learn the Scourge has already chosen you.",
                "Among the highest of the Lich King's commanders, with whole legions of the risen at their command.",
                "The Ebon Hold's fury made manifest, loosed only when the Lich King means to leave nothing standing.",
                "The Frozen Throne's own chosen blade. No higher honour in death, and none colder, their will is the Lich King's, and the world ends where they point."
            }),

            new WarRankTitleSet("BlackEmpire", "Black Empire / Old Gods / Void", new[]
            {
                "Whispered One", "Void-Touched", "Faceless Initiate", "Whispered", "Mindbreaker",
                "Mawguard", "Black Blade", "Darkheart", "Herald's Fang", "Void Champion", "Faceless One",
                "Watcher of the Deep", "High Speaker", "Eye of Madness", "Harbinger of Whispers",
                "Maw of the Void", "Black Empire Champion", "Wrath of N'Zoth", "Chosen of C'Thun",
                "Yogg-Saron's Hand", "Herald of the Black Empire", "Avatar of Madness",
                "The Thousandth Eye", "Void Incarnate"
            }),
        };

        // the set used when settings haven't loaded yet, or a saved id no longer exists. also the
        // one whose names match the def labels, so it's the natural default.
        public static WarRankTitleSet Default
        {
            get { return All[0]; }
        }

        public static WarRankTitleSet ById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                for (int i = 0; i < All.Length; i++)
                {
                    if (All[i].Id == id) return All[i];
                }
            }
            return Default;
        }

        // whatever the player currently has selected.
        public static WarRankTitleSet Current
        {
            get
            {
                if (WarRanksMod.Settings == null) return Default;
                return ById(WarRanksMod.Settings.TitleSetId);
            }
        }

        // the call the rest of the mod uses - resolves the current set and looks up the rank.
        public static string TitleFor(WarRank rank)
        {
            return Current.TitleFor(rank);
        }

        // flavour text for the current set, or null to fall back to the def's generic description.
        public static string DescriptionFor(WarRank rank)
        {
            return Current.DescriptionFor(rank);
        }

        public static IEnumerable<WarRankTitleSet> AllSets
        {
            get { return All; }
        }
    }
}
