# War Ranks

RimWorld 1.6 mod that gives colonists a 24-rank military prestige ladder based on their kills.
They start as a Scout and grind their way up to Grand High Warlord. It's a riff on the old
PvP rank titles.

## How it works

Ranks come straight off each pawn's `KillsHumanlikes` and `KillsMechanoids` records, so it just
uses numbers the game already keeps. Animal kills don't count, so nobody ranks up from hunting
or butchering.

Higher ranks are hediffs with gradually bigger buffs (social, pain resistance, mental break
resistance, and from rank 12 up a little shooting/melee/manipulation/sight). Rank 24 tops out at
Social Impact +35%, Pain Shock +50%, Mental Break -20%, Shooting/Melee +10%, Manip/Sight +10%.

A small rank icon shows above a colonist while they're selected or drafted (hover it for the
title and kill count), and you get a message when someone earns a promotion.

It works on existing saves too. When you load in, veterans get whatever rank they've already
earned without spamming the log.

## Settings

There are three name sets you can pick in mod options: Unified (plain military), Sindorei and
Kaldorei. Purely cosmetic, just changes the displayed titles, everything else stays the same.

## Install

Put the `WarRanks` folder in your RimWorld `Mods` folder, then in the mod list enable Harmony
above War Ranks.

## Building

Source is in `Source/WarRanks`. Build `WarRanks.csproj` (Release) and the dll drops into
`Assemblies/` on its own. If RimWorld or Harmony aren't in the default Steam paths you can pass
them in:

```
msbuild Source/WarRanks/WarRanks.csproj /p:RimWorldDir="D:\Games\RimWorld" /p:HarmonyDir="...\2009463077\Current"
```

Note: the defs in `Defs/Hediffs/WarRank_Hediffs.xml` are kept in sync with the table in
`WarRankData.cs`. Change buffs in one, change them in the other.
