O21 Contributor Guide
=====================

Development Prerequisites
-------------------------

To develop O21, you'll need [.NET 7 SDK][dotnet] (or a later version).

Build the Project
-----------------

Execute the following shell command:

```console
$ dotnet build
```

Code formatter
--------------

We use [Fantomas][fantomas] to format F# code automatically. To install tool locally use command:

```console
$ dotnet tools restore
```

Play the Original
-----------------

So far, the simplest way to play the original game is from [PlayMiniGames][playminigames.u95].

If the game sound is too loud and annoys you, you can disable it via the main menu. Unfortunately, PlayMiniGames' version of Windows 3.x doesn't seem to support Cyrillic, so you won't be able to read the actual names of the menu items. So, just open the first item of the main menu, and disable all the checkboxes there if you want to completely disable the in-game sound. 

Download the Original
---------------------

[Download it from the Internet Archive][archive.u95].

SHA256 hash of the `U_95.rar` file is `2F85B004B8C9BE21BE7327BD677370433DBD04B65F3B909561FE17EA6B43534A`.

See [the documentation on the original resources][docs.resources].

Extract the Original Resources
------------------------------

To run image export from a NE file (DLL or EXE), run the game with the following arguments:

```console
$ dotnet run O21.Game -- export <path-to-ne-file> <path-to-output-dir>
```

This will create a bunch of `.bmp` files in the output directory containing the game sprites. 

[archive.u95]: https://archive.org/details/u-95_20230304
[docs.resources]: docs/resources.md
[dotnet]: https://dot.net/
[playminigames.u95]: https://playminigames.net/game/u95
[fantomas]: https://fsprojects.github.io/fantomas/
