# Darth Maul Plugin v0.5.0

* Darth Maul mode
* One controller support
* In-game toggles
* Auto-detect functions that enables you to join/split the sabers during a map.
* Built-in unrandomizer for no arrow mode when darth maul mode on.
* Switchable main controller.

# Configuration

Run the game once to generate the settings, they are in Beat Saber/UserData/modprefs.ini.

```ini
[Darth Maul Plugin]
DMSerparation=15
DMDarthMode=0
DMOneHanded=0
DMAutoDetect=0

#  0 for left, 1 for right
DMMainController=1

# 0 - no random, 1 - only randomize loners, 2 - default value used by game. randomize lines with 2 or less blocks.
DMNoArrowRandLv=2
```

# Leaderboards
Turnning on Darth Maul Mode results in a separated leaderboard. 1 hand & 2 hand mode will have different leaderboards .

AutoDetect now only works in no-fail/party mode for this particular reason.

It's compatible with HiddenBlocks, meaning 
* DarthMaulMode on is one leaderborad.
* HiddenBlocks on is another.
* DarthMaulMode and HiddenBlocks both on yet another brand-new leaderboard.


# Downloads
[ModSaber](https://www.modsaber.ml/mod/darthmaul/0.4.0)
