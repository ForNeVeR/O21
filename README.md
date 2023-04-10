O21 [![Status Zero][status-zero]][andivionian-status-classifier]
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

Legal Disclaimers
-----------------

- This project is not affiliated with or endorsed by Nikita Online in any way.
- This project is non-commercial. The source code is available for free and always will be.
- This is a blackbox re-implementation project. The code in this project was written based on reading data files, and observing the game running. In some cases the code was written based on specs available on the Internet.
  
  I believe this puts the project in the clear, legally speaking. If someone disagrees, please reach the lead maintainer, [Friedrich von Never][fornever].

- No assets from the original game are included in this repo.

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

[andivionian-status-classifier]: https://github.com/ForNeVeR/andivionian-status-classifier#status-zero-
[book.windows-undocumented-file-formats]: https://a.co/d/dq5fCoj
[docs.code-of-conduct]: CODE_OF_CONDUCT.md
[docs.contributing]: CONTRIBUTING.md
[docs.license]: LICENSE.md
[docs.resources]: docs/resources.md
[docs.winhelp]: http://www.oocities.org/mwinterhoff/helpfile.htm
[fornever]: https://github.com/ForNeVeR/
[helpdeco]: https://github.com/pmachapman/helpdeco
[issues]: https://github.com/ForNeVeR/O21/issues
[nikita]: https://en.wikipedia.org/wiki/Nikita_Online
[old-games.u95]: https://www.old-games.ru/game/4676.html
[open-sage]: https://github.com/OpenSAGE/OpenSAGE
[status-zero]: https://img.shields.io/badge/status-zero-lightgrey.svg
[wikipedia.o21]: https://en.wikipedia.org/wiki/HNLMS_O_21
[wikipedia.u-95]: https://en.wikipedia.org/wiki/German_submarine_U-95_(1940)
[docs.inter-font-license]: O21.Game/Resources/LICENSE.txt
