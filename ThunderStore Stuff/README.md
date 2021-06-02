Basic Description
------------
This mod adds a genetics system to the monsters, with every stat being able to mutate in specific ways
The core stats that are affected are: Health, MoveSpeed, Damage, and AttackSpeed
The goal of the system detailed below is to allow the game to adapt to the player, and in turn force the player to adapt to it

How it Works
------------
- Every monster has four genes (Health, MoveSpeed, Damage, and AttackSpeed)
- Every minute (or after 40 monster deaths), the algorithm learns from the fallen monsters and adjusts it's "strategy"
- This learning is primarily done through a weighted average system that attempts to judge the "combat effectiveness" of a mutation

Video Explanations
------------
1.4.5 - 30-minute overview (outdated, but still semi-accurate)
	https://youtu.be/V4uxtHPvQKY
2.4.1 - "Rapid Mutation" overview (outdated, this functionality hasn't been re-added yet)
	https://youtu.be/odUNUfmcHg4

Known Issues/Planned Updates
------------
- A feedback system (visual or other) to let the player know how the mutations are trending
- 4.0.0 hasn't been stress-tested, if you find a breaking bug let me know in the #tech-support channel

Changelog
-----------
```
4.0.5
- Rebuilt the hidden item stats using RecalculateStatsAPI from R2API v3.0.43
  - This should help prevent future conflicts with other stat-adjusting mods

4.0.4
- Adjusted TakeDamage hook to avoid certain nullrefs
  - This should prevent Glacial Wisps from infinitely exploding when they die
- Adjusted learning algorithm to round to 2 digits before spawning new bodies
  - Should prevent fringe cases where items are given when they shouldn`t be

4.0.3
- Algorithm will no longer attempt to give genes to bodies that lack inventories (Barrels, Pots, Vagrant Bombs, Urchins, etc.)
  - This would cause nullrefs for these bodies when it tried to allocate their genes
  - Mods that give inventories to these special bodies shouldn`t re-break them

4.0.2
- Fixed a potential divide-by-zero error that could happen during the learning cycle
  - This would only happen if a given BodyIndex had no scoring children

4.0.1
- Removed caching that was causing some major conflicts with certain lobby mods
  - This should fix not being able to start a run in most cases

4.0.0
- Reduced number of genes to 4 (Health, MoveSpeed, AttackSpeed, and AttackDamage)
  - Regen and Armor either didn`t apply to most monsters, or were used by the algorithm to "cheat-out" extra points in other stats
  - Size never really worked they way I wanted, and balancing it doesn`t make much sense
  - The remaining stats should keep the algorithm learning and guessing, instead of just sticking with one stat
- Max and Min gene values have been greatly expanded (0.01 to 10.00, starting at 1.00)
  - Max Product has also been adjusted (from 10 down to 1.5)
  - This should prevent the algorithm from maxing multiple stats, instead pushing for a more balanced build
- Scoring algorithm has been adjusted to focus on time spent in combat instead of just dps
  - Should prevent mega-tanks and glass-cannons, instead pushing for solid combatants
- Learning algorithm has been revamped to use new scores and more data
  - Instead of every 10 seconds, the learn only runs every 60 seconds or once enough data (40 monsters) is available
  - This may seem significantly slower than before, but the extra data means that it can be "smarter" with it
- Networking is now managed through hidden items
  - Host is now the source of truth for the algorithm, no more work for clients
- Rapid Mutation and Infection have been removed
  - Infection was unstable at best and buggy more often than it was worth
  - Rapid Mutation might come back if demand is high enough, but the new algorithm benefits from having more data
```

Installation
------------
Place the .dll in Risk of Rain 2\BepInEx\plugins or use a mod manager.

Contact
------------
If you have issues/suggestions leave them on the github as an issue/suggestion or reach out to Rico#6416 on the modding Discord.
