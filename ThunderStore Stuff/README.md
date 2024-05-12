Basic Description
------------
- This mod adds a genetics system to the monsters, with every stat being able to mutate in specific ways
- The core stats that are affected are: Health, MoveSpeed, Damage, and AttackSpeed
- The goal of the system detailed below is to allow the game to adapt to the player, and in turn force the player to adapt to it

How it Works
------------
- Every monster has four genes (Health, MoveSpeed, Damage, and AttackSpeed)
- Every minute (or after 40 monster deaths), the algorithm learns from the fallen monsters and adjusts it's "strategy"
- This learning is primarily done through a weighted average system that attempts to judge the "combat effectiveness" of a mutation

Video Explanations
------------
- 1.4.5 - 30-minute overview (outdated, but still semi-accurate)
	- https://youtu.be/V4uxtHPvQKY
- 2.4.1 - "Rapid Mutation" overview (outdated, this functionality hasn't been re-added yet)
	- https://youtu.be/odUNUfmcHg4
- 4.1.0 - Challenge Portal Promo
	- https://youtu.be/wCFA7I3Oblk
- 4.4.0 - 5-minute overview for 4.X versions
    - https://youtu.be/MZ9iuUv4RU4

Known Issues/Planned Updates
------------
- A feedback system (visual or other) to let the player know how the mutations are trending
- I need to make some actual documentation/architecture to explain how it works from a dev point-of-view
- Is GeneticsVariantsPatch coming back? Yep! Check it out here: https://thunderstore.io/package/Rico/GeneticVariantsPatch/
- Is there anything else to add? Maybe. I'm considering making a patch for Artifacts of Doom and Risk of Chaos (\*wink\*)
- Are you a dev who has questions? Make an issue on GitHub and flag it as a question, I'm always happy to help

Changelog
-----------
```
4.5.3
- Added compatibility for RiskOfOptions
  - Gone are the days of restarting the game to update a config value
  - (Technically already could do this with DebugToolkit and saving over the config file)

4.5.2
- Publicized the monster copy adaption method, for dev usage
  - Should`ve done this a long time ago, but now devs can force specific mutations ad-hoc
  - Monster copy methods do not trigger RecalculateStats on their own, to avoid duplicated effort
- Added explicit RecalculateStats call on monster copy first generation
  - Monster spawn already does this iirc, but this guarantees genes take effect in fringe cases

4.5.1
- No new functionality, just refactored to use the Nuget packages instead of local libraries
  - Should make remote development much easier if I ever need to do that
  
4.5.0
- Gene-specific cap and floor overrides are back, and without config bloat this time
  - Two new config options, one to opt into using the overrides and the other to actually define them
  - Override syntax is "Gene1,FloorForGene1,CapForGene1|Gene2,FloorForGene2,CapForGene2"
    - Current valid Gene names are MaxHealth, MoveSpeed, AttackSpeed, and AttackDamage
	- Floors/caps need to be parseable into floats, and should follow the same rules as shared floor/cap

4.4.1
- Bugfix for the token calculation and application methods
  - Stat modifications should more accurately and consistently represent the monster`s genes
  - Previously was bugged to represent the difference from the master copy instead sometimes
- Also added public events to most major steps in the genetic engine, for dev usage
  - These include: master copy creation and mutation, monster copy mutation and scoring, and global engine learning

4.4.0
- Added mode that allows stat modifications to apply even if the artifact is disabled
  - This means no sudden drops in difficulty if you disable it mid-run
  - There`s a config option for this, and it`s disabled by default (ie the old behaviour)

4.3.2
- Re-Added a configuration file for this
  - Tweak-able values include: when and how the algorithm decides to learn, mutation rates and limits
  - Safety limits might be a bit strict for now, but I want testing a feedback before lifting them

4.3.1
- Fix for Patch 1.2.3 since there are items in here
  - Converted to the new ArtifactCodeAPI methods since they work now
  - Also converted items to ContentAddition methods, expect harmless warnings

4.3.0
- Updated for SOTV and new R2API build
  - Also fixed a fringe case nullref bug that could happen in the Bulwark

4.2.0
- Added a GeneBlocker item for use in GeneticVariantsPatch that prevents stat mutations from effecting the holder
  - This item also prevents the holder from scoring points and participating in the fitness evaluation

4.1.0
- Added portal code, compound, and challenge for the artifact (thanks to ArtifactCodeAPI)
  - The Genetics challenge will not be required to unlock it for new runs, but...
  - The challenge will instead provide a dramatic mutation rate while in the Bulwark (20%-500% instead of base 90%-110%)
  - Portal code - in left-right top-bottom order - is TDTCGCTDT

4.0.7
- Fixed the random assignment math that`s been broken since 4.0.0 (oops)
  - Monsters now have the chance to get stronger instead of just atrophying away
  - Random range is now 90%-110% instead of 90%-90%

4.0.6
- Adjusted/fixed the math in the RecalculateStatsAPI stat hook
  - Stat modifications should work like all non-4.0.5 versions now

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
