O21 [![Status Enfer][status-enfer]][andivionian-status-classifier]
===

O21 is a modern free and open-source remake of [U95][old-games.u95], a game by [NIKITA][nikita] company, released in 1995.

Implementation Status
---------------------

Currently, we are still at the exploration stage; scope of work is to be determined.

Visit [the issue tracker][issues] if you want to know more.

Usage
-----

Currently O21 is only suitable for development mode. To start the game, run the following shell command:

```console
$ dotnet run --project O21.Game -- <downloaded-data-directory>
```

Game History
------------

U95 (also called as _U-95_ on the title screen) is a game well known in 1990's Russian-speaking gaming community, since it was one of the first games published there.

It was created in 1995 by one of the first Russian game development companies, NIKITA (currently renamed to Nikita Online), for Windows 3.x.

### O21

Since the original game is probably named after a [German submarine U-95][wikipedia.u-95], that was sunk by Dutch [O 21][wikipedia.o21], the authors decided to name the remake project O21, as a (wishfully) superior project (at least in some aspects).

License and Legal Disclaimers
-----------------------------

_This section is maintained by me, the O21 project lead, [Friedrich von Never][fornever]. The statements made in first person are all mine, do not hesitate to contact me if you have any comments._

All the content and code in this program (except the `O21.Game/Translations` folder and the fonts, see below) is covered by the [MIT license][docs.license].

This program is designed to use resources of the U-95 game by [NIKITA][nikita]. This project is not affiliated with or endorsed by NIKITA in any way. The project is non-commercial. The source code is available for free and always will be.

This is a blackbox re-implementation project. The code in this project was written based on reading data files, and observing the game running. In some cases the code was written based on specs available on the Internet.

I believe this puts the project in the clear, legally speaking. If someone disagrees, please reach me.

The distribution license of the original game is unknown, so the game has an unclear de jure status. De facto, it is [abandonware][] nowadays.

The `O21.Game/Translations` folder contains translations of some of the original game content, legally distributed under the same terms as the original game content. As no user should access this content without agreeing with the original distribution license, I consider this as a proper distribution. 

Other than the translations described above, no assets from the original game are included in this software (though it may allow the user to use any assets downloaded under the user's informed consent).

The software also bundles the Inter font, covered by the [SIL Open Font License][docs.inter-font-license].

Documentation
-------------

- [Contributor Guide][docs.contributing]
  - [Original Game Resources][docs.resources]
- [License (MIT)][docs.license]
- [Inter Font License (SIL)][docs.inter-font-license]
- [Code of Conduct (adapted from the Contributor Covenant)][docs.code-of-conduct]

Acknowledgments
---------------

- Thanks to [Nikita Online][nikita] for making such a great game!
- Thanks to the [OpenSAGE][open-sage] project for some ideas on the wording in the **Legal Disclaimers** section.
- For the documentation on WinHelp, we'd like to thank:
  - Pete Davis and Mike Wallace, the authors of [Windows Undocumented File Formats][book.windows-undocumented-file-formats],
  - Manfred Winterhoff, the author of [the documentation][docs.winhelp],
  - Paul Wise and other contributors of [helpdeco][].

[abandonware]: https://en.wikipedia.org/wiki/Abandonware
[andivionian-status-classifier]: https://github.com/ForNeVeR/andivionian-status-classifier#status-enfer-
[book.windows-undocumented-file-formats]: https://a.co/d/dq5fCoj
[docs.code-of-conduct]: CODE_OF_CONDUCT.md
[docs.contributing]: CONTRIBUTING.md
[docs.inter-font-license]: O21.Game/Resources/LICENSE.txt
[docs.license]: LICENSE.md
[docs.resources]: docs/resources.md
[docs.winhelp]: http://www.oocities.org/mwinterhoff/helpfile.htm
[fornever]: https://github.com/ForNeVeR/
[helpdeco]: https://github.com/pmachapman/helpdeco
[issues]: https://github.com/ForNeVeR/O21/issues
[nikita]: https://en.wikipedia.org/wiki/Nikita_Online
[old-games.u95]: https://www.old-games.ru/game/4676.html
[open-sage]: https://github.com/OpenSAGE/OpenSAGE
[status-enfer]: https://img.shields.io/badge/status-enfer-orange.svg
[wikipedia.o21]: https://en.wikipedia.org/wiki/HNLMS_O_21
[wikipedia.u-95]: https://en.wikipedia.org/wiki/German_submarine_U-95_(1940)
