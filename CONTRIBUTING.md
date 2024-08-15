<!--
SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>

SPDX-License-Identifier: MIT
-->

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

Play the Original
-----------------

So far, the simplest way to play the original game is from [PlayMiniGames][play-mini-games.u95].

If the game sound is too loud and annoys you, you can disable it via the main menu. Unfortunately, PlayMiniGames' version of Windows 3.x doesn't seem to support Cyrillic, so you won't be able to read the actual names of the menu items. So, just open the first item of the main menu, and disable all the checkboxes there if you want to completely disable the in-game sound. 

Download the Original
---------------------

[Download it from the Internet Archive][archive.u95].

SHA256 hash of the `U-95.zip` file is `7F884B5FCCF65198F510905C41AF06BE1715E3482B123BBB8149D51FAE6A1460`.

See [the documentation on the original resources][docs.resources].

Extract the Original Resources
------------------------------

To run image export from a NE file (DLL or EXE), run the game with the following arguments:

```console
$ dotnet run O21.Game -- export <path-to-ne-file> <path-to-output-dir>
```

This will create a bunch of `.bmp` files in the output directory containing the game sprites. 

License Automation
------------------
If the CI asks you to update the file licenses, follow one of these:
1. Update the headers manually (look at the existing files), something like this:
   ```csharp
   // SPDX-FileCopyrightText: %year% %your name% <%your contact info, e.g. email%>
   //
   // SPDX-License-Identifier: MIT
   ```
   (accommodate to the file's comment style if required).
2. Alternately, use [REUSE][reuse] tool:
   ```console
   $ reuse annotate --license MIT --copyright '%your name% <%your contact info, e.g. email%>' %file names to annotate%
   ```

(Feel free to attribute the changes to "O21 contributors <https://github.com/ForNeVeR/O21>" instead of your name in a multi-author file, or if you don't want your name to be mentioned in the project's source: this doesn't mean you'll lose the copyright.)

File Encoding Changes
---------------------
If the automation asks you to update the file encoding (line endings or UTF-8 BOM) in certain files, run the following PowerShell script ([PowerShell Core][powershell] is recommended to run this script):
```console
$ pwsh -File Scripts/Test-Encoding.ps1 -AutoFix
```

The `-AutoFix` switch will automatically fix the encoding issues, and you'll only need to commit and push the changes.

[archive.u95]: https://archive.org/details/u-95_20230304
[docs.resources]: docs/resources.md
[dotnet]: https://dot.net/
[play-mini-games.u95]: https://playminigames.net/game/u95
[powershell]: https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell
[reuse]: https://reuse.software/
