using System;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.Common.Collectible.Block;
using Vintagestory.GameContent;

namespace FasterNature.Patches;

[HarmonyPatch(typeof(BlockEntityStoneCoffin), "onServerTick3s")]
static class CementationFurnacePatch
{
    [HarmonyPrefix]
    private static void onServerTick3s_Prefix(BlockEntityStoneCoffin __instance, ref bool ___receivesHeat, 
        ref double ___progress, ref double ___totalHoursLastUpdate, ref bool ___processComplete, ref BlockStoneCoffinSection ___blockScs,
        ref MultiblockStructure ___ms, ref MultiblockStructure ___msOpp, ref InventoryStoneCoffin ___inv)
    {
        var totalHours = __instance.Api.World.Calendar.TotalHours;
        var num1 = float.MaxValue;
        foreach (var fuelPosition in __instance.FuelPositions)
        {
            var val2 = 0.0f;
            var blockEntity = __instance.Api.World.BlockAccessor.GetBlockEntity(fuelPosition);
            if (blockEntity == null && __instance.Api.World.BlockAccessor.GetChunkAtBlockPos(fuelPosition) == null)
                return;
            if (blockEntity is IExternalTickable externalTickable)
                externalTickable.SetExternallyTicked();
            if (blockEntity is BlockEntityCoalPile blockEntityCoalPile && blockEntityCoalPile.IsBurning)
                val2 = blockEntityCoalPile.GetHoursLeft(totalHours);
            num1 = Math.Min(num1, val2);
        }

        var num2 = ___receivesHeat ? 1 : 0;
        var structureComplete = __instance.StructureComplete;
        if (!___receivesHeat)
            ___totalHoursLastUpdate = totalHours;
        ___receivesHeat = num1 > 0.0;
        
        __instance.StructureComplete = false;
        if (___ms.InCompleteBlockCount(__instance.Api.World, __instance.Pos) == 0)
            __instance.StructureComplete = true;
        else if (___msOpp.InCompleteBlockCount(__instance.Api.World, __instance.Pos.AddCopy(___blockScs.Orientation.Opposite)) == 0)
            __instance.StructureComplete = true;

        var num3 = ___receivesHeat ? 1 : 0;
        if (num2 != num3 || structureComplete != __instance.StructureComplete)
            __instance.MarkDirty();
        
        var hasLid = __instance.Api.World.BlockAccessor.GetBlockAbove(__instance.Pos, layer: 1).FirstCodePart() == "stonecoffinlid" 
                     && __instance.Api.World.BlockAccessor.GetBlockAbove(__instance.Pos.AddCopy(___blockScs.Orientation.Opposite), layer: 1).FirstCodePart() == "stonecoffinlid";
        if (!___processComplete && __instance.IsFull && hasLid && __instance.StructureComplete)
        {
            if (___receivesHeat)
            {
                var a = totalHours - ___totalHoursLastUpdate;
                var progress = Math.Max(0.0f, GameMath.Min((float)a, num1)) / 160.0;
                var newProgress = progress * (1 / (float)FasterNatureModSystem.config.CementationFurnaceSpeedRate);

                float temperature = ___inv[1].Itemstack.Collectible.GetTemperature(__instance.Api.World, ___inv[1].Itemstack);
                float num4 = (float) (a * 500.0) + temperature;
                float newTemp = num4 * (1 / (float)FasterNatureModSystem.config.CementationFurnaceSpeedRate);
                ___inv[1].Itemstack.Collectible.SetTemperature(__instance.Api.World, ___inv[1].Itemstack, Math.Min(800f, newTemp));
                ___progress += newProgress;
            }
        }
    }
}