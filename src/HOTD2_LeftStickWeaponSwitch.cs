using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace HOTD2LeftStickWeaponSwitch {
 [BepInPlugin("local.hotd2.leftstickweaponswitch", "HOTD2 Left Stick Weapon Switch", "1.0.0")]
 public sealed class Plugin : BaseUnityPlugin {
  private static ConfigEntry<bool> featureEnabled;
  private static ManualLogSource log;
  private Harmony harmony;
  private const string BindingPath = "<Gamepad>/leftStickPress";
  private void Awake() {
   log = Logger;
   featureEnabled = Config.Bind<bool>("Controls", "LeftStickChangesWeapon", true, "Press the left stick (L3) once to select the next weapon.");
   if (!featureEnabled.Value) return;
   harmony = new Harmony("local.hotd2.leftstickweaponswitch");
   try {
    ConstructorInfo constructor = AccessTools.Constructor(typeof(MP_InputActions), Type.EmptyTypes);
    if (constructor == null) throw new MissingMethodException("MP_InputActions", ".ctor");
    harmony.Patch(constructor, postfix: new HarmonyMethod(typeof(Plugin), "AddLeftStickBinding"));
    Logger.LogInfo("Left stick weapon switch installed: L3 -> next weapon.");
   } catch (Exception ex) {
    harmony.UnpatchSelf();
    Logger.LogError("Left stick weapon switch could not be installed: " + ex);
   }
  }
  private static void AddLeftStickBinding(MP_InputActions __instance) {
   if (!featureEnabled.Value) return;
   try {
    InputAction action = __instance.asset.FindAction("Gameplay/ChangeWeaponToNext", true);
    foreach (InputBinding binding in action.bindings) {
     if (binding.path == BindingPath && binding.groups == "Gamepad") return;
    }
    InputActionSetupExtensions.AddBinding(action, BindingPath, interactions: "Press(behavior=0)", groups: "Gamepad");
    log.LogInfo("Gameplay binding added: left stick click -> next weapon (once per press).");
   } catch (Exception ex) {
    log.LogError("Could not add left stick weapon binding: " + ex);
   }
  }
  private void OnDestroy() { if (harmony != null) harmony.UnpatchSelf(); }
 }
}
