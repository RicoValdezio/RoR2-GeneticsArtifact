Basic Description
------------
This mod adds a genetics system to the monsters, with every stat being able to mutate in specific ways.
The core stats that are affected are: Health, Regen, MoveSpeed, Acceleration, Damage, AttackSpeed, and Armor (and Size if its enabled)
The goal of the system detailed below is to allow the game to adapt to the player, and in turn force the player to adapt to it.

How it Works
------------
- Every monster has seven (eight if Size is enabled) multipliers that are randomly assigned on spawn.
- If a monster manages to damage you, it will be awarded points based on how much damage it did.
- When the monster dies (or is despawned by a stage change), its performance is sent to the master that spawned it.
- Every so often, the master will adapt its core values to reflect its best performers.
- These masters are then used to determine how the multipliers are assigned in future spawns.
Also, this video is my attempt at explaining this along with how the config values can affect the artifact and its systems: Its a bit scatter-brained but covers all of it.
<https://youtu.be/V4uxtHPvQKY>

Known Issues/Planned Updates
------------
- A feedback system to let the player know how the mutations are trending
- Reimplement the size modifier once I figure out the networking
- If you find a case of the -infinity health bug, type !geneticsbughunt into the discord chat

Changelog
------------
1.4.5 - Housekeeping update, fixed the readme and added a video explanation
1.4.4 - Added even more NaN checks and security to the scoring system
1.4.3 - Added a NaN check for damage calculations in attempt to fix -infinty bug
1.4.2 - Added the body level to the optional logging to help cut out some guesswork
1.4.1 - Added some optional logging for the bug-hunters, and a bit a additional safety in the master/child relationship
1.4.0 - Brand new balance system in attempt to prevent the -infinity health bug, expect new bugs
1.3.0 - Added config option to apply the artifact to neutrals and player minions (*You Monster*)
1.2.1 - Added config options for per-generation deviation and re-enabling the Size modifier
1.2.0 - Refactored the hook registry to prevent behaviour's from spamming hooks, should greatly reduce lag issue
1.1.1 - Minor improvement to garbage collection, at this point I'm fighting with the engine's computational power
1.1.0 - Major change to genetic system, possible performance hit, but should be more consistent
1.0.1 - Minor optimization due to bug report, should almost halve the computations required
1.0.0 - Initial upload, expect day one bugs

Installation
------------
Place the .dll in Risk of Rain 2\BepInEx\plugins or use a mod manager.

Contact
------------
If you have issues/suggestions leave them on the github as an issue/suggestion or reach out to Rico#6416 on the modding Discord.
