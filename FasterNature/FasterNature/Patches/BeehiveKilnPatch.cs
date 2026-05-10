using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace FasterNature.Patches;

[HarmonyPatch(typeof(BlockEntityBeeHiveKiln), "UpdateGroundStorage")]
internal static class BeehiveKilnPatch
{
    [HarmonyPrefix]
    private static void UpdateGroundStorage_Prefix(BlockEntityBeeHiveKiln __instance, float hoursHeatReceived, ref BlockPos[] ___particlePositions)
    {
        hoursHeatReceived *= 1 / (float) FasterNatureModSystem.config.BeehiveKilnSpeedRate;
        List<ItemSlot?> itemsToUpdate = new List<ItemSlot?>();

        for (var index = 0; index < 9; ++index)
        for (var length = 1; length < 4; ++length)
        {
            var blockEntity =
                __instance.Api.World.BlockAccessor.GetBlockEntity<BlockEntityGroundStorage>(___particlePositions[index]
                    .UpCopy(length));
            if (blockEntity != null)
                for (var slotId = 0; slotId < blockEntity.Inventory.Count; ++slotId)
                {
                    var itemSlot = blockEntity.Inventory[slotId];
                    if (!itemSlot.Empty)
                    {
                        itemsToUpdate.Add(itemSlot);
                    }
                }
        }

        if (!itemsToUpdate.Any())
            return;

        foreach (var itemSlot in itemsToUpdate)
        {
            var temperature =
                itemSlot.Itemstack.Collectible.GetTemperature(__instance.Api.World, itemSlot.Itemstack,
                    hoursHeatReceived);
            var num2 = hoursHeatReceived * BlockEntityBeeHiveKiln.ItemTemperatureGainPerHour;
            var num3 = (BlockEntityBeeHiveKiln.ItemBurnTemperature - temperature) /
                       BlockEntityBeeHiveKiln.ItemTemperatureGainPerHour;
            if (num3 < 0.0)
                num3 = 0.0f;
            
            temperature = GameMath.Min(BlockEntityBeeHiveKiln.ItemMaxTemperature, temperature + num2);

            var num4 = hoursHeatReceived - num3;
            if ((double)temperature >= (double)BlockEntityBeeHiveKiln.ItemBurnTemperature && (double)num4 > 0.0)
            {
                var newTime = num4 * (1 / (float) FasterNatureModSystem.config.BeehiveKilnSpeedRate);
                float num1 = itemSlot.Itemstack.Attributes.GetFloat(nameof(hoursHeatReceived)) + newTime;
                itemSlot.Itemstack.Attributes.SetFloat(nameof(hoursHeatReceived), num1);
            }
            
        }
    }
}