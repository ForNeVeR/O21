<!--
SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>

SPDX-License-Identifier: MIT
-->

# Command-line Interface

### Base commands
* [`start`](#command-start) - Start the game
* [`export`](#command-export) - Export resources
* [`helpFile`](#command-helpfile) - Parse a WinHelp file and extract all the information from it

### Common syntax
* `--no-logo` - Suppress banner message
* [`-h` | `--help`] - Show CLI help info or command help info

### Command `start`
```console
$ O21.Game start <GAME_DIR> [--screenSizes <WIDTH> <HEIGHT>]
```
**Arguments:**
1. `GAME_DIR` - The directory where the game will be loaded
2. *(Option)* `--screenSizes` - Set up the sizes of window (width and height)

### Command `export`
```console
$ O21.Game export <RESOURCES_DIR> [-o|--out <OUTPUT_DIR>]
```
**Arguments:**
1. `RESOURCES_DIR` - The file from which the resources will be exported
2. *(Option)* `--out` - Directory where resources will be stored

### Command `helpFile`
```console
$ O21.Game helpFile <HELP_FILE> [-o|--out <OUTPUT_DIR>]
```
**Arguments:**
1. `HELP_FILE` - Path to the input .hlp file.
2. *(Option)* `--out` - Path to the output directory
