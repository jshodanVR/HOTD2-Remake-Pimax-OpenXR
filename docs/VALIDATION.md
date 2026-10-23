# Release validation

## Scope

Reference game: Windows Steam app 3376690, build 24017736, Unity 2022.3.62f3. Exact reference hashes and preset values are recorded in SUPPORTED_BUILD.json.

The original FPS_FIX archive was supplied by the user as Astien's distributed package. Its VR plugin matches the untouched v1.0.0 binary used for the local modifications.

## Checked

- Both companion plugins compile against the installed game and upstream BepInEx/Harmony assemblies.
- The calibration tool verifies the original vector initializers before changing four float operands.
- The calibrated DLL matches the locally configured VR mod.
- OpenXR/OpenXRLoader, finite ammunition, all weapons, pistol x3, other weapons x2 and L3 are present in the release presets.
- Native code paths for starting ammunition, reloading, ammunition display, weapon availability and next-weapon input were inspected.
- The complete ZIP is compared with the upstream archive: untouched upstream payloads and original license texts must match byte for byte.
- No game executable, game Assembly-CSharp.dll, save, local log, local username or credential is added to the package.
- A release SHA256 checksum and per-file package manifest are provided.

## Remaining hardware validation

A fresh-install playthrough with the packaged release has not been performed in this session. The user has confirmed the chosen angle in the local setup; the latest combined build still requires confirmation of all controls, magazine counts and alignment in the headset.

This release is a beta. It does not claim certification or testing across every Pimax headset/controller/runtime combination.
