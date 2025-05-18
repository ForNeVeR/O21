<!--
SPDX-FileCopyrightText: 2024-2025 Friedrich von Never <friedrich@fornever.me>

SPDX-License-Identifier: MIT
-->

Maintainer Guide
================

Publish a New Version
---------------------
1. Update the project status in the `README.md` file, if required.
2. Update the copyright statement in the `LICENSE.md` file, if required.
3. Prepare a corresponding entry in the `CHANGELOG.md` file (usually by renaming the "Unreleased" section).
4. Update the `Version` field in the `Directory.Build.props` file. 
5. Merge the aforementioned changes via a pull request.
6. Push a tag in form of `v<VERSION>`, e.g. `v0.0.0`. GitHub Actions will do the rest.
