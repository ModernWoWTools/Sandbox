# Sandbox
9.2.7 WoW sandbox. 

Code donated by Arctium for local modding purposes only, do not ask them for support. 

## Usage
Use with a correctly set up 9.2.7 client as well as the launcher available from [here](https://github.com/ModernWoWTools/Launcher).

## Available commands
|Command          |Description              |
|-----------------|-------------------------|
|!fly `on`/`off`|
|!runspeed `value` | 1-1000 |
|!flightspeed `value` | 1-1000 | 
|!swimspeed `value` | 1-1000 |
|!tele `x` `y` `z` `o` `mapid`|Teleports to location, `o` (orientation) is optional  |
|!tele `name`|Teleports to location name (added with !loc) |
|!loc `name` | Prints location, adding a `name` adds it to locations.txt for !tele |
|!addnpc `creatureId`| 
|!delnpc `currentSelection` |
|!additem `itemIds` | You can use multiple IDs separated by spaces |
|!additem `itemId,version` | Where `version` is normal, heroic, mythic or lfr. |
|!emote `id` |
|!commentator `on`/`off` |Detaches the camera from the character. Change speed with /script C_Commentator.SetMoveSpeed(40)

## License
MIT, see LICENSE file for additional notes on this specific release.