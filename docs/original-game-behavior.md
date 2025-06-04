<!--
SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>

SPDX-License-Identifier: MIT
-->

Original Game Behavior
======================
This documents the behavior we observe in the original game.

Player
----------------

Player can move left and right, and can also move up and down when not collides any brick.

When player dies, it freezes for a 6 ticks (0.6 seconds) and cannot move or shoot during this time. But during this time, the player can still be hit by enemies.

Initial Fish Spawn
------------------
Restarted the original level 1 for 30 times, got the following number of enemies on screen:

| Enemy Number | Count of Times |
|-------------:|---------------:|
|            1 |              5 |
|            2 |              4 |
|            3 |             11 |
|            4 |              8 |
|            5 |              2 |

On a separate experiment, it was zero enemies initially.

Enemies never initially spawn on the same height as the submarine (to prevent sudden deaths on loading or entering the level, probably). Enemies can spawn on the same line when spawning from an enter or an exit, but so far we never observed them to spawn over player directly right from the start.

Since the original level (1-1) has 807 eligible cells (951 empty total - 48*3 cells on the player's line), this gives about 0.363% probability for an empty cell to spawn an enemy on loading a level. 

When restarting rapidly, it's easy to get the same random seed multiple times — probably the seed depends on the current time in seconds, though we aren't going to port that part.

Off-Screen Fish Spawn
---------------------
During a minute on level 1-1, 5 enemies were spawn (on unequal intervals).

During a minute on level 1-2, 9 enemies were spawn (on unequal intervals).

Level 1-1 has 15-cell exit, while level 1-2 has two of these.

Thus, the current hypothesis will be that each cell on the level's boundary has a 0.056 probability of spawning an enemy each tick (10 times per second), provided there's enough space for the enemy to enter the field.

Fish Enemy Behavior
-------------------
When hitting a vertical wall and unable to proceed further, a fish will try to move upwards if possible, or downwards if upwards is impossible.

### Fish wall collision algorithm

When a fish cannot move forward due to a wall or obstacle:
- The fish checks both upward and downward directions (whether there is an open path and the wall length in those directions).
- If both directions are blocked:
    - If the wall above is of length 0 and the wall below is long (more than 2 blocks), the fish turns around.
    - If both the upper and lower walls are short (≤2 blocks), the fish turns around.
    - Otherwise, the fish tries to move upward.
- If at least one direction is open and at least one wall is long (more than 2 blocks):
    - If the upward path is shorter and open, the fish moves upward.
    - Otherwise, the fish moves downward.
- In all other cases, the direction is Up or Down (short wall, take an open path).

#### Wall length calculation (wallAheadInfo)
- For the chosen direction (up or down), the fish counts the number of consecutive wall blocks.
- If there is an open path after the wall, the function returns the wall length and a flag indicating the path is open.
- If the path is blocked, the function returns the wall length and a flag indicating the path is closed.

This algorithm allows the fish to bypass long walls, turn around in dead ends, and choose the optimal path when encountering obstacles.

The fish has varying speed: some are fast, and some are slow, about 50/50 (hard to measure precisely). Sometimes, the fish change their movement speed after hitting a wall.

The "slow" ones touch the wall on 1-1 in 15 seconds after spawning on exit, the "fast" in 12 seconds. Which makes their speed about 4 or 5 pixels per tick.

Speed of vertical fish movement seems to be the same as for the mines, 20 pixels per second. 

Bomb/Medusa Behavior
--------------------
When a player enters a column with a bomb or medusa (actually, about 5-pixel radius around them — meaning they are possible to trigger even without being in any danger if acted accurately), they start moving upwards with a speed of about 20 pixels per second.

The sprite seems to be picked by random among 3 octopi, a medusa and a bomb.
