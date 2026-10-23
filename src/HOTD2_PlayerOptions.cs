using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace HOTD2PlayerOptions {
 [BepInPlugin("local.hotd2.playeroptions", "HOTD2 Player Options", "1.2.0")]
 public sealed class Plugin : BaseUnityPlugin {
  internal static ConfigEntry<bool> InfiniteAmmo;
  internal static ConfigEntry<bool> AllWeapons;
  internal static ConfigEntry<int> MagazineMultiplier;
  internal static ConfigEntry<int> PistolBaseMagazineMultiplier;
  private Harmony harmony;
  private void Awake() {
   InfiniteAmmo = Config.Bind<bool>("Gameplay", "InfiniteAmmo", false, "Use the game's native infinite ammunition cheat.");
   AllWeapons = Config.Bind<bool>("Gameplay", "AllWeapons", true, "Make all weapons available in gameplay and weapon availability checks.");
   MagazineMultiplier = Config.Bind<int>("Gameplay", "MagazineMultiplier", 2, new ConfigDescription("Multiplier for finite magazine capacity, including bonuses.", new AcceptableValueRange<int>(1, 10)));
   PistolBaseMagazineMultiplier = Config.Bind<int>("Gameplay", "PistolBaseMagazineMultiplier", 3, new ConfigDescription("Multiplier for the pistol base capacity. Ordinary bonuses are added once. Other weapons use MagazineMultiplier.", new AcceptableValueRange<int>(1, 10)));
   harmony = new Harmony("local.hotd2.playeroptions");
   try {
    foreach (string name in new string[] { "IsCheatActive", "GetCheatState", "IsCheatUnlocked" }) {
     MethodInfo method = AccessTools.Method(typeof(CR_Cheats), name, new Type[] { typeof(CheatType) });
     if (method == null || method.ReturnType != typeof(bool)) throw new MissingMethodException(typeof(CR_Cheats).FullName, name);
     harmony.Patch(method, prefix: new HarmonyMethod(typeof(Plugin), "CheatQueryPrefix"));
    }
    MethodInfo available = AccessTools.Method(typeof(CR_AdditionalModesPersistentData), "IsWeaponAvailable", new Type[] { typeof(PlayerWeapon) });
    if (available == null || available.ReturnType != typeof(bool)) throw new MissingMethodException("CR_AdditionalModesPersistentData", "IsWeaponAvailable");
    harmony.Patch(available, prefix: new HarmonyMethod(typeof(Plugin), "WeaponAvailablePrefix"));
    MethodInfo locker = AccessTools.Method(typeof(CR_LockerItem), "IsUnlocked", Type.EmptyTypes);
    if (locker == null || locker.ReturnType != typeof(bool)) throw new MissingMethodException("CR_LockerItem", "IsUnlocked");
    harmony.Patch(locker, prefix: new HarmonyMethod(typeof(Plugin), "LockerUnlockedPrefix"));
    MethodInfo maximum = AccessTools.Method(typeof(CR_Weapon), "MaxAmmoWithBonuses", Type.EmptyTypes);
    if (maximum == null || maximum.ReturnType != typeof(int)) throw new MissingMethodException("CR_Weapon", "MaxAmmoWithBonuses");
    MethodInfo bullets = AccessTools.Method(typeof(CR_AmmoController), "spawnAllBullets", Type.EmptyTypes);
    if (bullets == null) throw new MissingMethodException("CR_AmmoController", "spawnAllBullets");
    harmony.Patch(maximum, postfix: new HarmonyMethod(typeof(Plugin), "MagazineCapacityPostfix"));
    harmony.Patch(bullets, transpiler: new HarmonyMethod(typeof(Plugin), "UseMagazineCapacityForAmmoDisplay"));
    Logger.LogInfo("Player options loaded. InfiniteAmmo=" + InfiniteAmmo.Value + ", AllWeapons=" + AllWeapons.Value + ", MagazineMultiplier=" + MagazineMultiplier.Value + ", PistolBaseMagazineMultiplier=" + PistolBaseMagazineMultiplier.Value);
   } catch (Exception ex) {
    harmony.UnpatchSelf();
    Logger.LogError("Player options could not be installed: " + ex);
   }
  }
  private static bool CheatQueryPrefix(CheatType __0, ref bool __result) {
   if ((__0 == CheatType.InfiniteAmmo && InfiniteAmmo.Value) || (__0 == CheatType.AllWeaponsUnlocked && AllWeapons.Value)) {
    __result = true;
    return false;
   }
   return true;
  }
  private static bool WeaponAvailablePrefix(PlayerWeapon __0, ref bool __result) {
   if (AllWeapons.Value && __0 != PlayerWeapon.Unknown && Enum.IsDefined(typeof(PlayerWeapon), __0)) {
    __result = true;
    return false;
   }
   return true;
  }
  private static bool LockerUnlockedPrefix(CR_LockerItem __instance, ref bool __result) {
   PlayerWeapon weapon = __instance.WeaponID;
   if (AllWeapons.Value && weapon != PlayerWeapon.Unknown && Enum.IsDefined(typeof(PlayerWeapon), weapon)) {
    __result = true;
    return false;
   }
   return true;
  }
  private static void MagazineCapacityPostfix(CR_Weapon __instance, ref int __result) {
   if (__result <= 0 || __result >= 9999) return;
   if (__instance.WeaponType == PlayerWeapon.Pistol && __instance.MaxAmmo > 0 && __instance.MaxAmmo < 9999) {
    long ordinaryBonuses = (long)__result - __instance.MaxAmmo;
    __result = (int)Math.Max(1L, Math.Min((long)__instance.MaxAmmo * PistolBaseMagazineMultiplier.Value + ordinaryBonuses, 9998L));
   } else if (MagazineMultiplier.Value > 1) {
    __result = (int)Math.Min((long)__result * MagazineMultiplier.Value, 9998L);
   }
  }
  private static IEnumerable<CodeInstruction> UseMagazineCapacityForAmmoDisplay(IEnumerable<CodeInstruction> instructions) {
   FieldInfo rawCapacity = AccessTools.Field(typeof(CR_Weapon), "MaxAmmo");
   MethodInfo fullCapacity = AccessTools.Method(typeof(CR_Weapon), "MaxAmmoWithBonuses", Type.EmptyTypes);
   int replaced = 0;
   foreach (CodeInstruction instruction in instructions) {
    if (instruction.opcode == OpCodes.Ldfld && Object.Equals(instruction.operand, rawCapacity)) {
     instruction.opcode = OpCodes.Callvirt;
     instruction.operand = fullCapacity;
     replaced++;
    }
    yield return instruction;
   }
   if (replaced != 1) throw new InvalidOperationException("Unexpected ammunition display implementation; expected one raw capacity read.");
  }
  private void OnDestroy() { if (harmony != null) harmony.UnpatchSelf(); }
 }
}
