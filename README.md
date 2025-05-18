<!--
SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>

SPDX-License-Identifier: MIT
-->

O21 [![Status Enfer][status-enfer]][andivionian-status-classifier]
===

O21 is a modern free and open-source remake of [U95][old-games.u95], a game by [NIKITA][nikita] company, released in 1995.

Implementation Status
---------------------
Some of the game engine features have been implemented, but the game is not in a playable state.

Visit [the issue tracker][issues] if you want to know more.

The screenshot of the currently implemented version (mind that not every element from the screenshot might be functional yet):
![Screenshot][screenshot]

Usage
-----

Currently, O21 is only suitable for development mode. To start the game, run the following shell command:

```console
$ dotnet run --project O21.Game start <downloaded-data-directory>
```

See [the documentation on command line arguments][docs.command-line].

Game History
------------

U95 (also called as _U-95_ on the title screen) is a game well known in the 1990's Russian-speaking gaming community, since it was one of the first games published there.

It was created in 1995 by one of the first Russian game development companies, NIKITA (currently renamed to Nikita Online), for Windows 3.x.

### O21

Since the original game is probably named after a [German submarine U-95][wikipedia.u-95], that was sunk by Dutch [O 21][wikipedia.o21], the authors decided to name the remake project O21, as a (wishfully) superior project (at least in some aspects).

License and Legal Disclaimers
-----------------------------

The project is distributed under the terms of [the MIT license][docs.license]
(unless a particular file states otherwise).

The license indication in the project's sources is compliant with the [REUSE specification v3.3][reuse.spec].

_The following section is maintained by me, the O21 project lead, [Friedrich von Never][fornever]. The statements made in first person are all mine, do not hesitate to contact me if you have any comments._

This program is designed to use resources of the U-95 game by [NIKITA][nikita]. This project is not affiliated with or endorsed by NIKITA in any way. The project is non-commercial. The source code is available for free and always will be.

This is a blackbox re-implementation project. The code in this project was written based on reading data files and observing the game running. In some cases, the code was written based on specs available on the Internet.

I believe this puts the project in the clear, legally speaking. If someone disagrees, please reach me.

The distribution license of the original game is unknown, so the game has an unclear de jure status. De facto, it is [abandonware][] nowadays.

No assets from the original game are included in this software (though it may allow the user to use any assets downloaded under the user's informed consent).

Versioning Notes
----------------
This project's versioning follows the [Semantic Versioning 2.0.0][semver] specification.

When considering compatible changes, we take the input and output data format into account, such as:
- the command-line arguments,
- the data pack format (the original file layout),
- the output data (such as table of leaders) format.

Note that any particular non-zero executable exit codes are not considered part of the public API.

Documentation
-------------
- [Changelog][docs.changelog]
- [Command-Line Interface][docs.command-line]
- [Contributor Guide][docs.contributing]
  - [Original Game Resources][docs.resources]
- [Maintainer Guide][docs.maintaining]
- [Code of Conduct (adapted from the Contributor Covenant)][docs.code-of-conduct]

Acknowledgments
---------------
- Thanks to [Nikita Online][nikita] for making such a great game!
- Thanks to the [OpenSAGE][open-sage] project for some ideas on the wording in the **Legal Disclaimers** section.

[abandonware]: https://en.wikipedia.org/wiki/Abandonware
[andivionian-status-classifier]: https://github.com/ForNeVeR/andivionian-status-classifier#status-enfer-
[docs.changelog]: CHANGELOG.md
[docs.code-of-conduct]: CODE_OF_CONDUCT.md
[docs.command-line]: docs/command-line.md
[docs.contributing]: CONTRIBUTING.md
[docs.license]: LICENSE.md
[docs.maintaining]: MAINTAINING.md
[docs.resources]: docs/resources.md
[fornever]: https://github.com/ForNeVeR/
[issues]: https://github.com/ForNeVeR/O21/issues
[nikita]: https://en.wikipedia.org/wiki/Nikita_Online
[old-games.u95]: https://www.old-games.ru/game/4676.html
[open-sage]: https://github.com/OpenSAGE/OpenSAGE
[reuse.spec]: https://reuse.software/spec-3.3/
[screenshot]: docs/screenshot.png
[semver]: https://semver.org/spec/v2.0.0.html
[status-enfer]: https://img.shields.io/badge/status-enfer-orange.svg
[wikipedia.o21]: https://en.wikipedia.org/wiki/HNLMS_O_21
[wikipedia.u-95]: https://en.wikipedia.org/wiki/German_submarine_U-95_(1940)
