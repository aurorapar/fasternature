using System.Runtime.CompilerServices;
using HarmonyLib;
using Vintagestory.GameContent;

namespace FasterSmelts.Patches;

[HarmonyPatch(typeof(BlockEntityBloomery), "TryIgnite")]
static class BloomeryPatch
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "TryIgnite")]
    static extern string GetUnsafeMethod(BlockEntityBloomery accessor);
    
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_burningUntilTotalDays")]
    static extern ref double GetBurningUntilTotalDays(BlockEntityBloomery accessor);
    
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_burningUntilTotalDays")]
    static extern ref double SetBurningUntilTotalDays(BlockEntityBloomery accessor, double value);
    
    [HarmonyPostfix]
    static void TryIgnite_Postfix(BlockEntityBloomery __instance)
    {
        var accessor = __instance;
        var current = GetBurningUntilTotalDays(accessor);
        SetBurningUntilTotalDays(accessor, current * FasterSmeltsModSystem.config.BloomerySpeedRate);
    }
}