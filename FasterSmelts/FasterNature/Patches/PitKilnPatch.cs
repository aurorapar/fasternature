using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace FasterSmelts.Patches;

[HarmonyPatch(typeof(BlockEntityPitKiln), "TryIgnite")]
public class PitKilnPatch
{
    [HarmonyPostfix]
    static void TryIgnite_Postfix(IPlayer byPlayer, BlockEntityPitKiln __instance)
    {
        __instance.BurningUntilTotalHours = __instance.Api.World.Calendar.TotalHours;
        double totalBurnTime = __instance.BurnTimeHours * FasterSmeltsModSystem.config.PitKilnSpeedRate;
        __instance.BurningUntilTotalHours += totalBurnTime;

       var bh = __instance.GetBehavior<BEBehaviorBurning>();
       __instance.Lit = true;
       bh.OnFirePlaced(__instance.Pos.UpCopy(), __instance.Pos.Copy(), byPlayer?.PlayerUID);

       __instance.Api.World.BlockAccessor.ExchangeBlock(__instance.Block.Id, __instance.Pos); // Forces a relight of this block

       __instance.MarkDirty(true);
    }
}
