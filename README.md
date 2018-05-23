# BTAccelerator

Forked from [Ciaphas01](https://github.com/ciaphas01/BTAccelerator). Original Description :

Quick and really dirty app to adjust movement speeds in Hare Brained Schemes' BattleTech. Based on information from the [Reddit thread](https://www.reddit.com/r/Battletechgame/comments/8f6b3l/psa_how_to_really_speed_movement_up_not_from/), posted by user thegerbilest.


New maintainer here : 

I know there's now some debug options (which is why Ciaphas [stopped maintaining](https://www.reddit.com/r/Battletechgame/comments/8f6b3l/psa_how_to_really_speed_movement_up_not_from/dy2hhip) this nice program), but still, I like to just press a button.
Also added :
- sound delay modification ([from this reddit thread](https://www.reddit.com/r/Battletechgame/comments/8f6b3l/psa_how_to_really_speed_movement_up_not_from/))
- backuping every modified files
- Restore original values (hardcoded value for sound modification, restoring backup for movement)
- Adding an "Accelerated" property to modified json files, so that restoring backup should not modify files replaced by an update
- Minor UI enhancements