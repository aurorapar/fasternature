using System;
using HarmonyLib;
using Vintagestory.GameContent;

namespace FasterNature.Patches;

[HarmonyPatch(typeof(BlockEntityBloomery), "TryIgnite")]
static class BloomeryPatch
{
    [HarmonyPostfix]
    static void TryIgnite_Postfix(BlockEntityBloomery __instance, ref double ___burningUntilTotalDays)
    {
        // this.burningUntilTotalDays = this.Api.World.Calendar.TotalDays + 5.0 / 12.0;
        // this was changed to 10.0 / 24.0 in 1.22
        ___burningUntilTotalDays = __instance.Api.World.Calendar.TotalDays - 10 / 24.0;
        double newTime = 10 / 24.0 * FasterNatureModSystem.config.BloomerySpeedRate;
        ___burningUntilTotalDays = __instance.Api.World.Calendar.TotalDays + newTime;
    }
}