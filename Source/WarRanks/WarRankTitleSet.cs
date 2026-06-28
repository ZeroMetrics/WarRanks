using System.Collections.Generic;

namespace WarRanks
{
    // one named set: the titles a player sees, optional flavour text per rank, and an optional
    // promotion message. purely cosmetic - the ladder, kill thresholds and buffs are identical no
    // matter which set is picked, only the words change.
    public sealed class WarRankTitleSet
    {
        public readonly string Id;     // stable key saved in settings - don't rename once shipped
        public readonly string Label;  // what shows in mod options

        private readonly string[] titles;        // 24, low rank to high; index = rank.Level - 1
        private readonly string[] descriptions;  // optional, same indexing; null = use the def's generic text
        private readonly string promotionTemplate; // optional; {PAWN} and {TITLE} get filled in

        public WarRankTitleSet(string id, string label, string[] titles,
            string[] descriptions = null, string promotionTemplate = null)
        {
            Id = id;
            Label = label;
            this.titles = titles;
            this.descriptions = descriptions;
            this.promotionTemplate = promotionTemplate;
        }

        // name for a rank in this set; falls back to the rank's built-in name if the array is
        // short or missing, so a half-finished set can't crash anything.
        public string TitleFor(WarRank rank)
        {
            if (rank == null) return string.Empty;
            if (titles == null || rank.Level < 1 || rank.Level > titles.Length) return rank.Title;
            return titles[rank.Level - 1];
        }

        // flavour text for a rank, or null if this set hasn't been written one (hediff keeps its
        // plain def description in that case).
        public string DescriptionFor(WarRank rank)
        {
            if (rank == null || descriptions == null) return null;
            if (rank.Level < 1 || rank.Level > descriptions.Length) return null;
            return descriptions[rank.Level - 1];
        }

        // the "so-and-so was promoted" line, faction-flavoured where a template is set.
        public string PromotionMessage(string pawnLabel, WarRank rank)
        {
            string title = TitleFor(rank);
            string template = string.IsNullOrEmpty(promotionTemplate)
                ? "{PAWN} has risen to the rank of {TITLE}."
                : promotionTemplate;
            return template.Replace("{PAWN}", pawnLabel).Replace("{TITLE}", title);
        }
    }

    // the registry of every title set and the helpers everything else calls.
    public static class WarRankTitles
    {
        // ===========================================================================
        // to add a set: copy a block, give it a unique Id (no spaces - it's what gets saved), a
        // Label, 24 titles in rank order, optionally 24 flavour lines, and optionally a promotion
        // template using {PAWN} and {TITLE}. everything else picks it up automatically.
        // ===========================================================================
        public static readonly WarRankTitleSet[] All =
        {
            // ---- Unified: the plain, no-lore military set. kept generic on purpose. ----
            new WarRankTitleSet("Unified", "Unified", new[]
            {
                "Scout", "Private", "Corporal", "Sergeant", "Senior Sergeant", "Master Sergeant",
                "Stone Guard", "Blood Guard", "Knight", "Knight-Lieutenant", "Knight-Captain",
                "Knight-Champion", "Centurion", "Legionnaire", "Champion", "Lieutenant Commander",
                "Commander", "Lieutenant General", "General", "Marshal", "Field Marshal",
                "Warlord", "High Warlord", "Grand High Warlord"
            },
            new[]
            {
                "A raw recruit sent ahead to watch and report, learning that staying alive is the first skill a soldier needs.",
                "Has taken a place in the line and learned to hold it, one of the many who do the hard work of war.",
                "Trusted to lead a fire team - the first taste of being responsible for the soldiers beside them.",
                "A backbone of the unit, keeping greener troops steady when the shooting starts.",
                "Years in the field have made this one's word as good as an order from above.",
                "A seasoned hand who has seen every way a fight can go wrong, and trains others to avoid them.",
                "Holds the line like a wall - the kind of soldier a defence is built around.",
                "Has bled for the colony more than once and keeps coming back to the fight.",
                "A proven warrior given the standing to match, equal to the toughest assignments.",
                "Now leads others into the fight, with the scars to prove they have earned it.",
                "Commands a company's worth of fighters and the respect that comes with keeping them alive.",
                "A champion of the field whose presence alone steadies a wavering line.",
                "Leads a hundred and is reckoned worth a hundred more in a hard fight.",
                "An elite veteran of countless engagements, the standard others are measured against.",
                "Few can match this one in open battle, and fewer still want to try.",
                "Trusted with the planning as well as the fighting - an officer who still leads from the front.",
                "Carries the weight of whole engagements and rarely loses one.",
                "A senior officer whose campaigns are studied by those who come after.",
                "Commands armies and the loyalty of everyone in them.",
                "Among the highest a soldier can rise, answerable only to the colony itself.",
                "Wins wars, not just battles, and has the record to prove it.",
                "A force of nature on the battlefield whose name alone breaks enemy morale.",
                "Stands at the very summit of the colony's warriors, all but unbeaten.",
                "A living legend of war - the soldier every recruit hopes one day to become."
            }),

            // ---- Kaldorei / Night Elf ----
            new WarRankTitleSet("Kaldorei", "Kaldorei / Night Elf", new[]
            {
                "Ashen Scout", "Grove Watcher", "Huntress", "Moonlit Outrider", "Silverwing Scout",
                "Grove Sentinel", "Silverleaf Sentinel", "Sentinel Lieutenant", "Sentinel Captain",
                "Shadowstalker", "Nightsaber Rider", "Ranger", "High Ranger", "Moonblade",
                "Moonblade Captain", "Warden Initiate", "Warden", "Archwarden", "Ancient Protector",
                "Wildhunt Champion", "Blade of Elune", "Wrath of the Moon", "Avatar of Vengeance",
                "Chosen of Elune"
            },
            new[]
            {
                "A young kaldorei only just come to the long watch, eyes still adjusting to the dark of the world.",
                "Set to guard the sacred groves, learning the patience that immortality demands.",
                "Rides to war on a nightsaber, glaive in hand, in the old tradition of the Sisterhood.",
                "Ranges far ahead under Elune's light, first to find the enemy and last to be seen.",
                "Carries word between the groves on swift wings, trusted with the army's eyes and ears.",
                "A sworn Sentinel now, standing the endless vigil over Kaldorei lands.",
                "Counted among the veteran guard, having held the line through more than one long night.",
                "Leads a wing of Sentinels, glaive and bow moving as one.",
                "Commands a company of the Sisterhood, a captain in Elune's service.",
                "Moves unseen through moonlit forest, striking before the enemy knows the hunt has begun.",
                "Bonded to a great cat and twice as deadly for it - cavalry of shadow and claw.",
                "A master of the bow whose arrows find their mark across impossible distance.",
                "Among the deadliest archers of the kaldorei, teaching the craft to those who follow.",
                "Wields the glaive like a falling crescent, blessed in the rites of the moon.",
                "Leads the Moonblades, the keenest edge of the Sentinel host.",
                "Has begun the harsh path of the Wardens, keepers of the most dangerous prisoners.",
                "A Warden in full - hunter of those who would escape justice, relentless and unyielding.",
                "Commands the Wardens themselves, the law of Kaldorei given blade and wing.",
                "A guardian as old as the forests, having watched empires rise and fall.",
                "Leads the wild hunt through the night, a terror to all who threaten the groves.",
                "Wields the moon goddess's fury made steel, blessed and feared in equal measure.",
                "Elune's anger walks the world in this one, and the despoiler does not stand before it.",
                "Vengeance itself given kaldorei form, sleepless and without mercy.",
                "Hand-picked by the moon goddess herself - the highest honour the kaldorei can know."
            },
            "Under Elune's light, {PAWN} rises to {TITLE}."),

            // ---- Sin'dorei / Blood Elf ----
            new WarRankTitleSet("Sindorei", "Sin'dorei / Blood Elf", new[]
            {
                "Scout", "Arcane Initiate", "Sunblade Recruit", "Spellguard", "Farstrider",
                "Veteran Farstrider", "Sunfury Adept", "Blood Knight", "Blood Knight Captain",
                "Blood Knight Champion", "Magister's Elite", "Sunreaver", "Phoenix Guard",
                "Phoenix Elite", "Sunwell Defender", "Arcane Champion", "High Spellbreaker",
                "Dawnblade Commander", "Sunfury Commander", "Blood Knight Lord", "Magister Ascendant",
                "Phoenix Lord", "Sunwell's Chosen", "Phoenix Ascendant"
            },
            new[]
            {
                "A young sin'dorei taking their first steps in service to Quel'Thalas, hungry to prove their worth.",
                "Has begun to drink from the well of magic that defines their people, for better or worse.",
                "Trains with the Sunblades, learning to weave spell and steel into one.",
                "Stands guard over the magisters, blade in one hand and arcane ward in the other.",
                "A ranger of the eternal forests, carrying on the proud tradition of Quel'Thalas.",
                "Has walked the borderlands for years, bow never far from hand.",
                "Commands real arcane power now, the Sunfury's fire answering their call.",
                "Has bent the Light to their will by force - a knight who takes rather than prays.",
                "Leads a band of Blood Knights, the Light wielded like a weapon at their command.",
                "A champion of the order, master of stolen radiance and martial skill alike.",
                "Counted among the magisters' chosen guard, trusted with their secrets and their lives.",
                "Serves the Sunreavers, blending diplomacy and raw magic in equal measure.",
                "Burns like the phoenix, rising again and again to defend the sun-kissed lands.",
                "Among the finest of the Phoenix Guard - fire made discipline.",
                "Sworn to protect the restored Sunwell, the heart of all sin'dorei power.",
                "A master of the arcane whose duels are studied and feared.",
                "Turns enemy magic against itself, a living ward against any caster.",
                "Leads the Dawnblades, the keen edge of Quel'Thalas at first light.",
                "Commands the Sunfury host, arcane fire marshalled like an army.",
                "Stands at the head of the Blood Knights, the Light wholly mastered.",
                "A magister of the highest order, power radiating from them like heat from the sun.",
                "Reborn from ash too many times to count, undying defender of the sin'dorei.",
                "Blessed directly by the Sunwell, carrying a fragment of its eternal light.",
                "Has become the phoenix entire - the deathless pinnacle of Quel'Thalas's might."
            },
            "For the glory of Quel'Thalas, {PAWN} is raised to {TITLE}."),

            // ---- Forsaken / Undead ----
            new WarRankTitleSet("Forsaken", "Forsaken / Undead", new[]
            {
                "Grave Fodder", "Rotguard", "Deathguard", "Dark Ranger", "Blightcaller",
                "Grave Warden", "Apothecary's Hand", "Plague Lord", "Banshee's Guard", "Dreadblade",
                "Deathdealer", "Crypt Champion", "Deathstalker", "Royal Apothecary", "Banshee Champion",
                "Plague Marshal", "Dread Commander", "Deathguard Lord", "Shadow Apothecary",
                "Lord of Blight", "Banshee's Chosen", "Champion of the Forsaken",
                "Herald of the Banshee Queen", "Hand of Ruin"
            },
            new[]
            {
                "Newly freed from the Scourge's grip, clinging to a will of their own and little else.",
                "Stands the wall for the Forsaken, indifferent to wounds that would fell the living.",
                "A sworn soldier of the Dark Lady, patrolling the ruins the living abandoned.",
                "Raised to serve with bow and banshee-cold aim, an undead echo of an elven ranger.",
                "Has learned to call the plague to hand, spreading rot with a gesture.",
                "Keeps watch over the Forsaken's dead and undead alike, trusted with grim duty.",
                "Assists the Royal Apothecary Society, with no question of conscience to slow the work.",
                "Commands diseases the way others command soldiers, and just as ruthlessly.",
                "Stands among the Dark Lady's personal guard, sworn to her cause beyond death.",
                "A blade that knows neither fear nor fatigue, carving through the living tirelessly.",
                "Deals death wholesale and counts it the only honest trade left to the dead.",
                "A champion risen from the crypts, stronger in undeath than they ever were in life.",
                "An assassin and spy of the Forsaken, ending threats before they are even seen.",
                "A full member of the Society, brewing plagues that could unmake the living world.",
                "Carries the Banshee Queen's wrath into battle, a wail of vengeance given form.",
                "Marshals the Forsaken's plagues across whole battlefields, turning the air to poison.",
                "Commands by dread as much as by orders, and the dead obey without hesitation.",
                "Stands at the head of the Deathguard, the Dark Lady's will made iron.",
                "Master of the darkest arts of the Society - secrets even other apothecaries fear.",
                "Holds dominion over the blight itself, withering the living at a word.",
                "Hand-picked by the Dark Lady, trusted with her most ruthless designs.",
                "Embodies the cold defiance of the undead, fighting for a people death could not claim.",
                "Speaks with the Dark Lady's voice, and the Forsaken move at the sound of it.",
                "The Banshee Queen's ruin given a single pair of hands, and they are never still."
            },
            "The Dark Lady's will lifts {PAWN} to {TITLE}."),

            // ---- Orc / Horde ----
            new WarRankTitleSet("Orc", "Orc / Horde", new[]
            {
                "Blooded", "Warscout", "Warrior", "Raider", "Wolf Rider", "Clan Guard",
                "Blademaster's Disciple", "Berserker", "Warsong Outrider", "Warbringer", "Clan Champion",
                "Kor'kron", "Blademaster", "War Captain", "Horde Champion", "War Commander",
                "Chieftain's Blade", "Warchief's Guard", "Conqueror", "Overlord", "Warlord",
                "High Overlord", "High Warlord", "Hand of the Warchief"
            },
            new[]
            {
                "Has tasted battle and survived it, earning the right to be called a warrior at all.",
                "Ranges ahead of the war band, reading the land for the best ground to spill blood on.",
                "A true warrior of the clan now, axe earned and honour intact.",
                "Rides with the raiding parties - fast and merciless, gone before the alarm is raised.",
                "Bonded to a dire wolf and twice as dangerous for it.",
                "Trusted to guard the clan's own, a wall of muscle and fury.",
                "Has begun the brutal training of the blademasters, learning the way of the blade.",
                "Gives themselves to the battle-rage, a storm of axes the enemy cannot weather.",
                "Rides with the Warsong, carrying the clan's war-cry deep into enemy ranks.",
                "Brings the war to the enemy's door - an orc the clans send when they mean to win.",
                "Has bested all challengers within the clan, its honour carried on their shoulders.",
                "Chosen for the Warchief's elite guard, the Kor'kron - the best the Horde can field.",
                "A master of the blade in full, a single warrior worth a war band.",
                "Leads a war band into battle and brings most of them home with their honour intact.",
                "Fights as a champion of the whole Horde, not just one clan.",
                "Commands the warriors of many clans, binding them into one fist.",
                "Acts as a chieftain's own blade, sent where the clan most needs blood spilled.",
                "Stands at the Warchief's side, the last line between the leader and the enemy.",
                "Has taken ground and held it, expanding the Horde's reach by sheer force.",
                "Rules over warriors and won territory alike, an overlord in deed as much as title.",
                "A warlord of the Horde, commanding armies and the fierce loyalty within them.",
                "Stands above other warlords, their word carrying the weight of conquest.",
                "Among the mightiest the Horde has ever raised, all but unstoppable in the field.",
                "Acts with the Warchief's own authority - the Horde's will made into a single warrior."
            },
            "Lok'tar! {PAWN} earns the rank of {TITLE}."),

            // ---- Human / Alliance ----
            new WarRankTitleSet("Human", "Human / Alliance", new[]
            {
                "Militia Scout", "Footman", "Shieldbearer", "Sergeant", "Veteran Footman",
                "Knight Aspirant", "Knight", "Knight-Lieutenant", "Knight-Captain", "Knight-Champion",
                "Lionguard", "Stormwind Champion", "Lieutenant Commander", "Commander", "Lord Commander",
                "Marshal", "High Marshal", "Field Marshal", "Grand Marshal", "Lionheart Commander",
                "King's Shield", "Alliance Vanguard", "Champion of Stormwind", "Lion of the Alliance"
            },
            new[]
            {
                "A levied townsfolk handed a tabard and a task, learning soldiering the hard way.",
                "Stands in the shield wall of Stormwind, the steadfast heart of any Alliance army.",
                "Holds the line behind a tower shield, trusted to keep the rank from breaking.",
                "Drills the footmen and keeps them in order when the charge comes.",
                "Has stood in a dozen shield walls and broken none of them.",
                "Has begun the long road to knighthood, squiring under a knight of the realm.",
                "Dubbed a knight of Stormwind, sworn to the crown and the Light.",
                "Leads a lance of knights - the first command on the road to greater things.",
                "Commands a company of the realm's knights with honour and skill.",
                "A champion among the knighthood, the king's banner safe in their hands.",
                "Counted among the Lionguard, the elite protectors of Stormwind's heart.",
                "Fights as a champion of the city itself, its lion crest worn with pride.",
                "Trusted with command of a wing of the army, planning as well as fighting.",
                "Carries whole engagements on their shoulders and rarely lets the line break.",
                "A lord of the army now, answerable to the crown for the lives in their charge.",
                "One of Stormwind's marshals, entrusted with the defence of the realm.",
                "Stands among the highest marshals, shaping the wars the Alliance fights.",
                "Wins campaigns in the field, the crown's trust earned in blood and ground held.",
                "Among the most decorated commanders the Alliance has ever raised.",
                "Leads with a lion's heart, the kind of officer soldiers will follow anywhere.",
                "The crown's own shield, the last guard between the realm and ruin.",
                "Spearheads the Alliance's greatest assaults, first through the breach.",
                "The city's chosen champion, its honour and its hope carried in one hand.",
                "A living symbol of the Alliance itself - the highest a child of Stormwind can rise."
            },
            "By the Light and the crown, {PAWN} is promoted to {TITLE}."),

            // ---- Illidari / Demon Hunter ----
            new WarRankTitleSet("Illidari", "Illidari / Demon Hunter", new[]
            {
                "Outcast", "Initiate Hunter", "Felblade", "Glaivebearer", "Demon Stalker", "Felblooded",
                "Soulhunter", "Illidari Adept", "Illidari Guard", "Illidari Veteran", "Fel Champion",
                "Demonbane", "Warglaive Master", "Chaos Reaver", "Nether Stalker", "Fel Commander",
                "Slayer of Demons", "Illidari Champion", "Vengeful Blade", "Herald of Chaos", "Wrathbound",
                "Chosen of the Betrayer", "Lord of the Hunt", "The Betrayer's Wrath"
            },
            new[]
            {
                "Shunned by their own for the dark path they have chosen, with only the hunt left to them.",
                "Has taken the first oath of the Illidari, fel beginning to burn behind their eyes.",
                "Wields fel-forged steel, the demon's own fire turned against it.",
                "Has earned the twin warglaives, learning to make them an extension of their fury.",
                "Hunts demons across the broken places of the world, relentless and unseen.",
                "Has bound demonic power into their own flesh, paying the price the hunt demands.",
                "Devours the souls of demons to grow stronger, a terrible bargain freely made.",
                "A true adept of the Illidari, fel magic and blade mastered in equal measure.",
                "Stands guard over the Illidari's strongholds, eyes that never truly close.",
                "Has survived hunts that claimed many, scarred by fel and experience alike.",
                "A champion of the Illidari, fel-fire answering their every strike.",
                "The bane of demonkind, named for the sheer number they have unmade.",
                "Wields the warglaives as few ever have - a whirlwind of green fire and steel.",
                "Reaves through enemy ranks with chaotic fury, leaving ruin in their wake.",
                "Slips through the nether itself to strike, here and gone like a bad dream.",
                "Leads the Illidari into battle, fel-blind eyes seeing more than any could.",
                "Has slain demons beyond counting, the very purpose of the Illidari made flesh.",
                "Stands as a champion of the whole order, the Betrayer's vision carried forward.",
                "Vengeance given an edge, striking for every wrong the Legion ever dealt.",
                "Heralds the chaos the Illidari wield, a storm of fel that breaks armies.",
                "Bound utterly to the wrath of the hunt, beyond fear and beyond turning back.",
                "Marked as one of Illidan's chosen, trusted with the hardest hunts of all.",
                "Commands the Illidari's greatest hunts, the demon-hunter others are measured against.",
                "The Betrayer's own wrath given form, the price of victory paid in full and willingly."
            },
            "{PAWN} embraces the demon within and rises to {TITLE}."),

            // ---- Scourge / Death Knight ----
            new WarRankTitleSet("Scourge", "Scourge / Death Knight", new[]
            {
                "Gravebound", "Boneguard", "Runeblade", "Frostblade", "Deathcharger", "Acherus Guard",
                "Plague Knight", "Blood Knight", "Frost Knight", "Unholy Champion", "Runeblade Champion",
                "Deathbringer", "Scourge Captain", "Reaper", "Bonelord", "Deathlord's Hand",
                "Frostbound Commander", "Champion of Acherus", "Rune Lord", "Lord of Bones",
                "Herald of Death", "High Deathlord", "Wrath of Acherus", "The Lich King's Champion"
            },
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
            },
            "The Lich King raises {PAWN} to {TITLE}."),

            // ---- Black Empire / Old Gods / Void ----
            new WarRankTitleSet("BlackEmpire", "Black Empire / Old Gods / Void", new[]
            {
                "Whispered One", "Void-Touched", "Faceless Initiate", "Whispered", "Mindbreaker",
                "Mawguard", "Black Blade", "Darkheart", "Herald's Fang", "Void Champion", "Faceless One",
                "Watcher of the Deep", "High Speaker", "Eye of Madness", "Harbinger of Whispers",
                "Maw of the Void", "Black Empire Champion", "Wrath of N'Zoth", "Chosen of C'Thun",
                "Yogg-Saron's Hand", "Herald of the Black Empire", "Avatar of Madness",
                "The Thousandth Eye", "Void Incarnate"
            },
            new[]
            {
                "Has begun to hear the whispers from below, and has made the mistake of listening.",
                "The void has left its mark, a cold that never quite leaves the bones.",
                "Takes the first steps toward becoming faceless, self slowly given over to the deep.",
                "The whispers are constant now, and the world above seems thin and far away.",
                "Has learned to crack open the minds of others and pour the void inside.",
                "Stands guard at the threshold of the deep, where the Old Gods stir.",
                "A blade in service to the Black Empire, sharpened on the screams of the sane.",
                "The heart has gone dark and cold, beating only for the will of the Old Gods.",
                "The fang of a greater herald, sent to draw the first blood of madness.",
                "A champion of the void, reality bending uneasily around them.",
                "Has become faceless at last, a servant of the Old Gods without name or self.",
                "Keeps the long watch over the sleeping horrors, patient as stone.",
                "Speaks the words that carry the Old Gods' will to lesser thralls.",
                "An eye through which the Black Empire sees, and through which madness leaks back out.",
                "Heralds the coming of the whispers, madness spreading in their wake.",
                "A mouth for the void's hunger, devouring will and sanity alike.",
                "Fights as a champion of the Black Empire entire, the old order made new.",
                "Carries the corruptor's wrath - N'Zoth's malice given a single set of hands.",
                "Hand-picked by the god of a thousand eyes, trusted with its oldest designs.",
                "Acts as the hand of the god of death, madness and ruin its only purpose.",
                "Heralds the empire's return, the world above already beginning to crack.",
                "Madness itself wearing flesh, and the flesh remembers nothing of who it was.",
                "The final eye of C'Thun, seeing all and concealing the deeper horror behind it.",
                "The void made flesh and will - the Black Empire's victory walking the waking world."
            },
            "The whispers name {PAWN} {TITLE}."),
        };

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

        public static string TitleFor(WarRank rank)
        {
            return Current.TitleFor(rank);
        }

        public static string DescriptionFor(WarRank rank)
        {
            return Current.DescriptionFor(rank);
        }

        public static string PromotionMessage(string pawnLabel, WarRank rank)
        {
            return Current.PromotionMessage(pawnLabel, rank);
        }

        public static IEnumerable<WarRankTitleSet> AllSets
        {
            get { return All; }
        }
    }
}
