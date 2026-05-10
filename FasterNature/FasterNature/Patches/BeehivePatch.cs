using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace FasterNature.Patches;

[HarmonyPatch(typeof(BlockEntityBeehive), "OnScanComplete")]
public class BeehivePatch
{
    [HarmonyPatch("Initialize")]
    [HarmonyPostfix]
    public static void Initialize_Postfix(BlockEntityBeehive __instance)
    {
        MethodInfo onScanForEmptySkep = typeof(BlockEntityBeehive).GetMethod("OnScanForEmptySkep", BindingFlags.NonPublic | BindingFlags.Instance);
        var onScanForEmptySkepAction = (Action<float>) onScanForEmptySkep.CreateDelegate(typeof(Action<float>), __instance);
        
        MethodInfo testHarvestable = typeof(BlockEntityBeehive).GetMethod("TestHarvestable", BindingFlags.NonPublic | BindingFlags.Instance);
        var testHarvestableAction = (Action<float>) testHarvestable.CreateDelegate(typeof(Action<float>), __instance);
        
        MethodInfo spawnBeeParticles = typeof(BlockEntityBeehive).GetMethod("SpawnBeeParticles", BindingFlags.NonPublic | BindingFlags.Instance);
        var spawnBeeParticlesAction = (Action<float>) spawnBeeParticles.CreateDelegate(typeof(Action<float>), __instance);
        
        __instance.UnregisterAllTickListeners();
        __instance.RegisterGameTickListener(onScanForEmptySkepAction, (int) ((__instance.Api.World.Rand.Next(5000) + 30000) * FasterNatureModSystem.config.BeehiveSpeedRate));
        __instance.RegisterGameTickListener(new Action<float>(testHarvestableAction), 3000);
        if (__instance.Api.Side == EnumAppSide.Client)
            __instance.RegisterGameTickListener(new Action<float>(spawnBeeParticlesAction), 300);
    }
    
    [HarmonyPatch("OnBlockPlaced")]
    [HarmonyPostfix]
    public static void OnBlockPlaced_Postfix(BlockEntityBeehive __instance, ref double ___harvestableAtTotalHours, ItemStack byItemStack = null)
    {
        ___harvestableAtTotalHours = __instance.Api.World.Calendar.TotalHours + 12.0 * (3.0 + __instance.Api.World.Rand.NextDouble() * 8.0) * FasterNatureModSystem.config.BeehiveSpeedRate;
    }
    
    [HarmonyPatch("TestHarvestable")]
    [HarmonyPrefix]
    static bool TestHarvestable_Prefix(float dt, BlockEntityBeehive __instance, ref double ___lastCheckedAtTotalHours,
        ref float ___roomness, ref float ___activityLevel, ref double ___harvestableAtTotalHours, ref double ___cooldownUntilTotalHours,
        ref double ___beginPopStartTotalHours, ref  bool ___isWildHive, ref EnumHivePopSize ___hivePopSize)
    {
        double num = (__instance.Api.World.Calendar.TotalHours - ___lastCheckedAtTotalHours) * FasterNatureModSystem.config.BeehiveSpeedRate;
        float temperature = __instance.Api.World.BlockAccessor.GetClimateAt(__instance.Pos, EnumGetClimateMode.ForSuppliedDate_TemperatureOnly, __instance.Api.World.Calendar.TotalDays).Temperature;
        if ((double) ___roomness > 0.0)
            temperature += 5f;
        ___activityLevel = GameMath.Clamp(temperature / 5f, 0.0f, 1f);
        
        if ((double) temperature <= 0.0)
        {
            ___harvestableAtTotalHours += num;
            ___cooldownUntilTotalHours += num;
            ___beginPopStartTotalHours += num;
        }
        ___lastCheckedAtTotalHours = __instance.Api.World.Calendar.TotalHours;
        if ((double) temperature <= -10.0)
        {
            ___harvestableAtTotalHours = __instance.Api.World.Calendar.TotalHours + 12.0 * (3.0 + __instance.Api.World.Rand.NextDouble() * 8.0) * FasterNatureModSystem.config.BeehiveSpeedRate;
            ___cooldownUntilTotalHours = __instance.Api.World.Calendar.TotalHours + 48.0 * FasterNatureModSystem.config.BeehiveSpeedRate;
        }
        if (__instance.Harvestable || ___isWildHive || __instance.Api.World.Calendar.TotalHours <= ___harvestableAtTotalHours || ___hivePopSize <= EnumHivePopSize.Poor)
            return false;
        __instance.Harvestable = true;
        __instance.MarkDirty(true);

        return false;
    }
    
    [HarmonyPatch("OnScanComplete")]
    [HarmonyPrefix]
    static bool OnScanComplete_Prefix(BlockEntityBeehive __instance, ref int ___quantityNearbyFlowers, 
        ref int ___scanQuantityNearbyFlowers, ref List<BlockPos> ___emptySkeps, ref List<BlockPos> ___scanEmptySkeps,
        ref BlockPos ___skepToPop, ref EnumHivePopSize ___hivePopSize, ref int ___quantityNearbyHives,
        ref double ___beginPopStartTotalHours, ref float ___popHiveAfterHours, ref double ___cooldownUntilTotalHours)
    {
        ___quantityNearbyFlowers = ___scanQuantityNearbyFlowers;
        ___emptySkeps = new List<BlockPos>(___scanEmptySkeps);

        if (___emptySkeps.Count == 0)
        {
            ___skepToPop = null;
        }

        ___hivePopSize = (EnumHivePopSize)GameMath.Clamp(___quantityNearbyFlowers - 3 * ___quantityNearbyHives, 0, 2);

        __instance.MarkDirty();


        if (3 * ___quantityNearbyHives + 3 > ___quantityNearbyFlowers)
        {
            ___skepToPop = null;
            __instance.MarkDirty(false);
            return false;
        }

        if (___skepToPop != null &&__instance.Api.World.Calendar.TotalHours > ___beginPopStartTotalHours + ___popHiveAfterHours)
        {
            object? originalInstance = (object)__instance;
            MethodInfo tryPopCurrentSkep = __instance.GetType().GetMethod("TryPopCurrentSkep", BindingFlags.NonPublic | BindingFlags.Instance);
            tryPopCurrentSkep.Invoke(originalInstance, new object[] { });
            
            ___cooldownUntilTotalHours = __instance.Api.World.Calendar.TotalHours + (4 / 2 * 24) * FasterNatureModSystem.config.BeehiveSpeedRate;
            __instance.MarkDirty(false);
            return false;
        }

        // Default Spread speed: Once every 4 in game days * factor
        // Don't spread at all if 3 * livinghives + 3 > flowers

        // factor = Clamped(livinghives / Math.Sqrt(flowers - 3 * livinghives - 3), 1, 1000)
        // After spreading: 4 extra days cooldown

        float swarmability = GameMath.Clamp(___quantityNearbyFlowers - 3 - 3 * ___quantityNearbyHives, 0, 20) / 5f;
        // We want to translate the swarmability value 0..4
        // into swarm days 12..0
        float swarmInDays = (float) ((4 - swarmability) * 2.5f * FasterNatureModSystem.config.BeehiveSpeedRate);

        if (swarmability <= 0) ___skepToPop = null;

        if (___skepToPop != null)
        {
            float newPopHours = 24 * swarmInDays;
            ___popHiveAfterHours = (float)(0.75 * ___popHiveAfterHours + 0.25 * newPopHours);

            if (!___emptySkeps.Contains(___skepToPop))
            {
                ___skepToPop = null;
                __instance.MarkDirty(false);
            }

        } else
        {
            ___popHiveAfterHours = 24 * swarmInDays;

            ___beginPopStartTotalHours = __instance.Api.World.Calendar.TotalHours;

            float mindistance = 999;
            BlockPos closestPos = null;
            foreach (BlockPos pos in ___emptySkeps)
            {
                float dist = pos.DistanceTo(__instance.Pos);
                if (dist < mindistance)
                {
                    mindistance = dist;
                    closestPos = pos;
                }
            }

            ___skepToPop = closestPos;
        }

        return false;
    }
}