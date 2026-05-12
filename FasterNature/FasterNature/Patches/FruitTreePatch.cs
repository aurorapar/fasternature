using System.Collections.Generic;
using HarmonyLib;
using Vintagestory.GameContent;

namespace FasterNature.Patches;

[HarmonyPatch(typeof(FruitTreeRootBH))]
public class FruitTreePatch
{
    [HarmonyPatch("RegisterTreeType")]
    [HarmonyPostfix]
    public static void RegisterTreeType_HarmonyPostfix(string treeType, FruitTreeRootBH __instance)
    {
        if (string.IsNullOrEmpty(treeType))
            return;
        
        foreach (KeyValuePair<string, FruitTreeProperties> keyValuePair in __instance.propsByType)
        {
            FruitTreeProperties props = keyValuePair.Value;
            props.GrowthStepDays *= (float) FasterNatureModSystem.config.FruitTreeSpeedRate;
        }
    }
}