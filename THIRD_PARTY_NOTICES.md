# Third-party notices

## Astien VR mod and FPS fix

The release includes the full FPS_FIX distribution supplied by Astien, with its original license files. The VR plugin is modified only for the documented Pimax calibration.

The original license, reproduced in licenses/VR MOD README.txt and preserved in the release under BepInEx/Licenses/, permits free modification and redistribution for noncommercial use, requires the original license notices, and prohibits paid redistribution. The FPS fix has corresponding conditions in that same file. These components are not relicensed by this repository's MIT license.

## Bundled dependencies

BepInEx, Harmony-related components, Unity XR libraries, SteamVR, ViGEmBus, Newtonsoft.Json, bHaptics, ProTubeVR and assets retain the upstream terms. Unity libraries remain part of the full VR mod distribution. ProTubeVR's bundled binary remains within this full mod release, following the distribution scope described by the upstream author.

All original BepInEx/Licenses/ and BepInEx/redist/ notices are retained without edits. Refer to the original VR MOD README.txt for component-specific references.

## New source files

src/, tools/ and Build.ps1 are jshodanVR's additions and are covered by the root MIT license. The license does not grant rights to game files or change any upstream conditions.

## Source and package separation

Game assemblies are used only as local compilation references. The release contains the files from Astien's provided mod archive, the calibrated VR plugin, our two companion plugins, presets and documentation. It does not add the game executable, Assembly-CSharp.dll, personal saves or local logs.
