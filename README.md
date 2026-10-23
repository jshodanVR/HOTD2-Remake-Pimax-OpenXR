# THE HOUSE OF THE DEAD 2 Remake — Pimax / OpenXR preset

**Built on the VR mod by Astien (GitHub: [Astienth/VR-Mods-Projects](https://github.com/Astienth/VR-Mods-Projects)).** The original VR conversion, framework and bundled dependencies are credited to their original authors. This project adds jshodanVR's Pimax calibration, controller binding and weapon options.

[Download v1.0.0-beta.1](https://github.com/jshodanVR/HOTD2-Remake-Pimax-OpenXR/releases/tag/v1.0.0-beta.1) · [Italiano](README_IT.md) · [Credits](CREDITS.md)

## Included preset

| Setting | Default |
| --- | --- |
| VR API | OpenXR, OpenXRLoader |
| Next weapon | Click the left stick (L3), once per press |
| Weapon pitch | 27 degrees upward relative to Astien's original profile |
| Pistol position | 4 cm forward and 4 cm up in the weapon holder's local axes |
| Weapons | All available while the companion plugin is active |
| Pistol magazine | 3 times its base capacity; ordinary bonuses are added once |
| Other magazines | 2 times normal capacity, including ordinary bonuses |
| Infinite ammunition | Off; firing consumes ammunition and reloading is required |
| Performance fix | Original FPS_FIX component supplied with Astien's archive |

This is a community preset tuned for the local Pimax/OpenXR setup. Hardware-specific alignment can vary. The first public release is a **beta**: compilation, calibration values, package contents and dependency integrity were checked; a fresh-install headset playthrough has not been completed for every Pimax model. See [validation](docs/VALIDATION.md).

## Install on the base game

1. Own and install the Windows Steam edition of **THE HOUSE OF THE DEAD 2 Remake**. Reference build: Steam app **3376690**, build **24017736**, Unity **2022.3.62f3**.
2. Download the **HOTD2-Remake-Pimax-OpenXR-v1.0.0-beta.1.zip** release asset. GitHub's automatic source ZIP is for developers.
3. Close the game. If updating an existing mod installation, keep a backup of its BepInEx folder and configuration.
4. Extract the release ZIP's contents into the game directory, alongside **THE HOUSE OF THE DEAD 2 Remake.exe**. Merge the BepInEx and game Data folders when asked. The full Astien FPS_FIX mod and its dependencies are included.
5. Have a working OpenXR runtime configured for your Pimax headset. The bundled configuration already selects OpenXR.
6. The upstream VR mod uses ViGEmBus for controller emulation. If this driver is missing, its original installer is included under **BepInEx/redist/**; complete that installation and any requested restart yourself.
7. Start the game and click the left controller's stick to cycle through the available weapons.

Choose Italian or English using the game's own language menu. No personal save is included or needed for weapon availability.

## Configuration

Edit these files with the game closed, then restart it:

- **BepInEx/config/local.hotd2.playeroptions.cfg**: AllWeapons = true, InfiniteAmmo = false, MagazineMultiplier = 2, PistolBaseMagazineMultiplier = 3.
- **BepInEx/config/local.hotd2.leftstickweaponswitch.cfg**: LeftStickChangesWeapon = true.
- **BepInEx/config/UnityVR_Bepinex.cfg**: VRMode = OpenXRLoader and vrApi = OpenXR.

The pistol override uses base capacity × 3 + ordinary bonuses. The other-weapon multiplier retains the previous ×2 behavior. These are finite magazines. Setting the multipliers to 1 restores normal capacity; set AllWeapons to false to use the game's normal unlock state. Training modes retain their native weapon restrictions.

For troubleshooting, inspect the BepInEx output in the game's Player.log for **HOTD2 Player Options** and **HOTD2 Left Stick Weapon Switch**. Avoid posting complete logs containing personal paths.

## Build from source

Windows with .NET Framework's C# compiler is sufficient. You need your own installed game and Astien's original FPS_FIX ZIP. Game assemblies are referenced locally during compilation and are not included in this source repository.

Run from PowerShell:

    .\Build.ps1 -GameDirectory 'D:\SteamLibrary\steamapps\common\THE HOUSE OF THE DEAD 2 Remake' -UpstreamArchive 'D:\Downloads\TheHouseOfTheDead2_VR_FPS_FIX.zip'

The script checks the supported game assembly and original mod hash, compiles both companion plugins, applies the four documented calibration values, preserves the original distribution and licenses, and creates the complete release ZIP plus SHA256SUMS.txt under dist/.

The upstream VR mod's source is not supplied by this repository. The calibration patch tool and the companion plugins are included in src/ and tools/.

## Credits and licenses

- **Astien / Astienth** — original VR mod and distribution: [VR-Mods-Projects](https://github.com/Astienth/VR-Mods-Projects).
- **CustomForLovemakerz / Lovemakerz** — FPS fix, as credited in the original VR MOD README.txt.
- **jshodanVR** — Pimax/OpenXR preset, companion plugins and release integration.
- **BepInEx, Harmony, Unity, SteamVR, ViGEmBus, Newtonsoft.Json, bHaptics, ProTubeVR and the original asset authors** — their respective components.

Our source additions use the MIT license. Bundled upstream components retain their own licenses, including Astien's and the FPS fix's **free redistribution / modification, noncommercial** conditions. Distribute the combined package free of charge and retain all notices. Read [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) and the included BepInEx/Licenses/ directory in the release.
