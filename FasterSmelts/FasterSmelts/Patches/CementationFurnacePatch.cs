using System;
using HarmonyLib;
using Vintagestory.GameContent;

namespace FasterSmelts.Patches;

[HarmonyPatch(typeof(BlockEntityStoneCoffin), "onServerTick3s")]
static class CementationFurnacePatch
{
    [HarmonyPostfix]
    static void onServerTick3s_Postfix(BlockEntityStoneCoffin __instance)
    {
        
    }
}