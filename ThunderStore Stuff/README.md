Basic Description
------------
This mod adds a genetics system to the monsters, with every stat being able to mutate in specific ways.
The core stats that are affected are: Health, Regen, MoveSpeed, Damage, AttackSpeed, and Armor (and Size if its enabled)
The goal of the system detailed below is to allow the game to adapt to the player, and in turn force the player to adapt to it.

How it Works
------------
- Every monster has six (seven if Size is enabled) multipliers that are randomly assigned on spawn.
- If a monster manages to damage you, it will be awarded points based on how much damage it did.
- When the monster dies (or is despawned by a stage change), its performance is sent to the master that spawned it.
- Every so often, the master will adapt its core values to reflect its best performers.
- These masters are then used to determine how the multipliers are assigned in future spawns.

Video Explanations
------------
1.4.5 - 30-minute overview
	https://youtu.be/V4uxtHPvQKY
2.4.1 - "Rapid Mutation" overview
	https://youtu.be/odUNUfmcHg4

Known Issues/Planned Updates
------------
- A feedback system to let the player know how the mutations are trending
- Reimplement the size modifier once I figure out the networking
- If you find a case of the -infinity health bug, type !geneticsbughunt into the discord chat
- Make a video for devs to explain how to use the genetic system for their own mods
	- The custom event flag system is loosely explained on the Git wiki

Changelog
-----------
2.5.1 - Fixed chat bug where the rapid mutation messages would display even if the artifact was disabled
2.5.0 - Added new custom event flag system to allow mods to define their own "Rapid Mutation" events
2.4.2 - Fixed the RogueWisp conflict, turns out that hook load order was the problem
2.4.1 - Housekeeping update, now unregisters hooks on game close and a new explanation video
2.4.0 - Added optional "Rapid Mutation" mode that can be triggered by Teleporter events, the final stage, or both (and a bit of polish)
2.3.1 - Changed master assignment in order to resolve bug where generic mode would fail to find a valid master
2.3.0 - Artifact can now be enabled/disabled/paused during a run (for use with KingEnderBrine-ArtifactsRandomizer)
2.2.0 - Major refactor of underlying mutation and balance system, each stat now has its own configurable cap and floor (manual installs will need to delete the old config)
2.1.1 - Moved component activation to prevent a visual bug with teleporter healthbars when Swarm is also active
2.1.0 - Minor refactor of stat application, fixed Armor and Regen not working in some cases, and cut Acceleration since MoveSpeed dominates it
2.0.0 - Major refactor of the underlying systems, and publicized most of the functionality for potential use by other mods
1.5.0 - Switched the stat application from the On space to the IL space (help from ThinkInvis and Rein)
1.4.7 - Switched logging system as per Harb's request
1.4.6 - The link broke in the readme for some reason, fixed that
1.4.5 - Housekeeping update, fixed the readme and added a video explanation
1.4.4 - Added even more NaN checks and security to the scoring system
1.4.3 - Added a NaN check for damage calculations in attempt to fix -infinty bug
1.4.2 - Added the body level to the optional logging to help cut out some guesswork
1.4.1 - Added some optional logging for the bug-hunters, and a bit a additional safety in the master/child relationship
1.4.0 - Brand new balance system in attempt to prevent the -infinity health bug, expect new bugs
1.3.0 - Added config option to apply the artifact to neutrals and player minions
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
