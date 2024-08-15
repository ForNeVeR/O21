# SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
#
# SPDX-License-Identifier: MIT

param (
    $SolutionRoot = "$PSScriptRoot/..",
    $TranslationsDirectory = "$SolutionRoot/O21.Game/Localization/Translations",

    $DefaultLanguage = 'english'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$defaultTranslation =
    Get-Content -LiteralPath "$TranslationsDirectory/$DefaultLanguage.json" `
    | ConvertFrom-Json -AsHashtable
$otherTranslations =
    Get-ChildItem -LiteralPath $TranslationsDirectory -Filter '*.json' `
    | Where-Object { $_.BaseName -ne $DefaultLanguage }

$errors = @()
foreach ($translationFile in $otherTranslations) {
    $translation = Get-Content -LiteralPath $translationFile | ConvertFrom-Json -AsHashtable
    $missing = $defaultTranslation.Keys | Where-Object { !($translation.Keys.Contains($_)) }
    if ($missing) {
        $errors += "Translation '$($translationFile.BaseName)' misses the keys: $($missing -join ', ')."
    }
    $leftover = $translation.Keys | Where-Object { !($defaultTranslation.Keys.Contains($_)) }
    if ($leftover) {
        $errors += "Translation '$($translationFile.BaseName)' has leftover keys: $($leftover -join ', ')."
    }
}

if ($errors) {
    throw $errors -join "`n"
}
