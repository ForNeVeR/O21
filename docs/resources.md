Original Game Resources
=======================

Here we'll discuss the resources of [the game version available from the Internet Achive][archive.u95] (`U_95.rar` file, SHA256 hash: `2F85B004B8C9BE21BE7327BD677370433DBD04B65F3B909561FE17EA6B43534A`). It is unknown yet whether there are any other versions available in the web.

Archive Content List
--------------------

- `U95_0_13.DAT`
- `U95_0_19.DAT`
- `U95_0_20.DAT`
- `U95_0_21.DAT`
- `U95_0_22.DAT`
- `U95_0_25.DAT`
- `U95_0_26.DAT`
- `U95_0_27.DAT`
- `U95_0_4.DAT`
- `U95_0_47.DAT`
- `U95_0_48.DAT`
- `U95_0_49.DAT`
- `U95_0_5.DAT`
- `U95_0_7.DAT`
- `U95_0.WAV`
- `U95_1_1.DAT`
- `U95_1_10.DAT`
- `U95_1_11.DAT`
- `U95_1_12.DAT`
- `U95_1_13.DAT`
- `U95_1_14.DAT`
- `U95_1_15.DAT`
- `U95_1_16.DAT`
- `U95_1_17.DAT`
- `U95_1_18.DAT`
- `U95_1_19.DAT`
- `U95_1_2.DAT`
- `U95_1_20.DAT`
- `U95_1_21.DAT`
- `U95_1_22.DAT`
- `U95_1_23.DAT`
- `U95_1_24.DAT`
- `U95_1_25.DAT`
- `U95_1_26.DAT`
- `U95_1_27.DAT`
- `U95_1_28.DAT`
- `U95_1_29.DAT`
- `U95_1_3.DAT`
- `U95_1_30.DAT`
- `U95_1_31.DAT`
- `U95_1_32.DAT`
- `U95_1_33.DAT`
- `U95_1_34.DAT`
- `U95_1_35.DAT`
- `U95_1_36.DAT`
- `U95_1_37.DAT`
- `U95_1_38.DAT`
- `U95_1_39.DAT`
- `U95_1_4.DAT`
- `U95_1_40.DAT`
- `U95_1_41.DAT`
- `U95_1_42.DAT`
- `U95_1_43.DAT`
- `U95_1_44.DAT`
- `U95_1_45.DAT`
- `U95_1_46.DAT`
- `U95_1_47.DAT`
- `U95_1_48.DAT`
- `U95_1_49.DAT`
- `U95_1_5.DAT`
- `U95_1_6.DAT`
- `U95_1_7.DAT`
- `U95_1_8.DAT`
- `U95_1_9.DAT`
- `U95_1.SCR`
- `U95_1.WAV`
- `U95_10.SCR`
- `U95_10.WAV`
- `U95_2_14.DAT`
- `U95_2_15.DAT`
- `U95_2_16.DAT`
- `U95_2_17.DAT`
- `U95_2_18.DAT`
- `U95_2_19.DAT`
- `U95_2_21.DAT`
- `U95_2_22.DAT`
- `U95_2_23.DAT`
- `U95_2_24.DAT`
- `U95_2_25.DAT`
- `U95_2_26.DAT`
- `U95_2_27.DAT`
- `U95_2_28.DAT`
- `U95_2_30.DAT`
- `U95_2_31.DAT`
- `U95_2_32.DAT`
- `U95_2_36.DAT`
- `U95_2_38.DAT`
- `U95_2_41.DAT`
- `U95_2_42.DAT`
- `U95_2_47.DAT`
- `U95_2_48.DAT`
- `U95_2_49.DAT`
- `U95_2_7.DAT`
- `U95_2.SCR`
- `U95_2.WAV`
- `U95_3_36.DAT`
- `U95_3_38.DAT`
- `U95_3_7.DAT`
- `U95_3.SCR`
- `U95_3.WAV`
- `U95_4_36.DAT`
- `U95_4_38.DAT`
- `U95_4.SCR`
- `U95_4.WAV`
- `U95_5_36.DAT`
- `U95_5_38.DAT`
- `U95_5.SCR`
- `U95_5.WAV`
- `U95_6.SCR`
- `U95_6.WAV`
- `U95_7.SCR`
- `U95_7.WAV`
- `U95_8.BMP`
- `U95_8.SCR`
- `U95_8.WAV`
- `U95_9.SCR`
- `U95_9.WAV`
- `U95_BRIC.DLL`
- `U95_G.SCR`
- `U95_PIC.DLL`
- `U95_T.SCR`
- `U95_W.SCR`
- `U95.EXE`
- `U95.HLP`
- `U95.MID`

Game Resources
--------------

- `*.DAT` — game level files in ASCII, see below.
- `*.WAV` — various sound effects. See below for the function of each file.
- `*.SCR` — BMP (can be opened by [ImageGlass][image-glass] at least) files with level backgrounds. Seem to be in the BMP format. See [#17][issue.17] on the current progress on figuring out the details.
- `U95_BRIC.DLL` — a 16-bit DLL (aka "[Win16 NE][win16-ne]") file with brick graphics in its embedded resources.
- `U95_PIC.DLL` — a 16-bit DLL file with enemy graphics in its embedded resources
- `U95.EXE` — the main game executable file. Also a 16-bit NE file containing some game graphics (UI elements, player sprite, bullet sprites, mines and certain enemies, bonus items).
- `U95.HLP` — game help file. We were unable to open it so far. See [#19][issue.19] for the current progress on presenting it in the game.
- `U95.MID` — a MIDI file with the in-game music. See [#20][issue.20] for the current progress on playing it.

.DAT Files
----------

The `.DAT` files contain the game levels in ASCII. There are three category of characters there:

- `1`–`9`: number of brick sprite from `U95_BRIC.DLL`. Resources have _names_ `Bitmap/2`–`Bitmap/10`, and their indices in bitmap section correspond exactly to their number in the `.DAT` file.
- `a`: random choice between certain bonus items:
    - bottle
    - candy
    - medal
    - treasure chest
    - floppy
    - canned fish
    - wrench
    - light bulb
    - seashell
    - compass
- `b`: random choice between bomb and four kinds of octopuses.

Sprite Format
-------------

16-bit NE files (`.DLL` and `.EXE`) that contain sprites can be opened by [eXeScope][exe-scope].

Every sprite contained in the NE files to consist of two bitmaps: one for the color data and another one for the transparency mask.

There's also a resource `15/1` in each file that seems to contain some kind of meta-information about the files (the file names corresponding to resources, at least).

See [#21][issue.21] for the current progress on sprite decoding.

Game Sounds
-----------

- `U95_0.WAV`: game started
- `U95_1.WAV`: game over
- `U95_2.WAV`: after game over, the player has made a new record
- `U95_3.WAV`: life taken
- `U95_4.WAV`: lifebuoy taken
- `U95_5.WAV`: treasure taken
- `U95_6.WAV`: a treasure or a lifebuoy destroyed by a bullet
- `U95_7.WAV`: shot
- `U95_8.WAV`: stationary enemy (an octopus or a mine) destroyed
- `U95_9.WAV`: swimming enemy destroyed
- `U95_10.WAV`: player lost a life

[archive.u95]: https://archive.org/details/u-95_20230304
[exe-scope]: http://www.filefacts.com/exescope-info
[image-glass]: https://imageglass.org/
[issue.12]: https://github.com/ForNeVeR/O21/issues/12
[issue.17]: https://github.com/ForNeVeR/O21/issues/17
[issue.19]: https://github.com/ForNeVeR/O21/issues/19
[issue.20]: https://github.com/ForNeVeR/O21/issues/20
[issue.21]: https://github.com/ForNeVeR/O21/issues/21
[issue.24]: https://github.com/ForNeVeR/O21/issues/24
[win16-ne]: https://jeffpar.github.io/kbarchive/kb/065/Q65122/
