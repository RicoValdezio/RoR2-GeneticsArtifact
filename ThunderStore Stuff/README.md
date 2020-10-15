Basic Description
------------
This mod adds a genetics system to the monsters, with every stat being able to mutate in specific ways.
Health, Size, and Move Speed are all directly related, with bigger monsters having more health and moving faster.
Acceleration is inversely tied to Move Speed, with faster monsters being less able to turn quickly.
Armor and Regen are inversely related, with heavily armored monsters having reduced regen rates.
Damage and Attack Speed are also inversely related, with heavy hitters attacking less often and slower than normal.

As of 1.1.0 the stats are no longer tied to each other. Instead a balance system that penalizes Health when other stats are too high has been implemented to prevent monsters from getting too strong.
This system can be disabled, but doing so removes all balance controls from the artifact.
Another side-effect of this system is that I decided to cut the Size modification due to networking difficulties.

As of 1.4.0 the health stat is no longer the only stat penalized using the new system, as all stats can be reduced to bring the product down to a reasonable value.
This can be configured, and with a Maximum Mutation Product set to 10, the average multiplier on each stat will equate to 1.4x.

How it Works
------------
This is kinda complicated. I plan on adding a video that can give the system a proper explanation, but for right now the main idea is that the longer a monster lives or the more damage it deals to you, the more likely its stats are to be repeated on new monsters.
The actual scoring system for mutations is 1 point per second of being alive, and 1 point for every point of damage it deals. Heavy hitters rack up points faster, but tanks can soak up more points just for being alive.
Note that the monsters aren't counted towards the genepool until they either die or the stage ends, so if a monster survives the entire stage it won't polute the future genepool.

Known Issues/Planned Updates
------------
- A feedback system to let the player know how the mutations are trending
- Improve this readme to make more sense, and to add a code walkthrough for those interested
- Reimplement the size modifier once I figure out the networking
- If you find a case of the -infinity health bug, type !geneticsbughunt into the discord chat

Changelog
------------
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
