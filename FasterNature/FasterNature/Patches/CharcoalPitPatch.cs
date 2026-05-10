using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace FasterNature.Patches;

[HarmonyPatch(typeof(BlockEntityCharcoalPit))]
public class CharcoalPitPatch
{
    [HarmonyPatch("IgniteNow")]
    [HarmonyPrefix]
    static void IgniteNow_Prefix(BlockEntityCharcoalPit __instance, ref double ___startingAfterTotalHours)
    {
        if (__instance.Lit)
          return;
        Traverse.Create(__instance).Property("Lit").SetValue(true);
        ___startingAfterTotalHours = __instance.Api.World.Calendar.TotalHours + 0.5 * FasterNatureModSystem.config.CharcoalPitSpeedRate;
        __instance.MarkDirty(true);
        if (__instance.Api.Side != EnumAppSide.Client)
          return;
        
        object? originalInstance = (object)__instance;
        MethodInfo updateSmokeLocations = __instance.GetType().GetMethod("UpdateSmokeLocations", BindingFlags.NonPublic | BindingFlags.Instance);
        updateSmokeLocations.Invoke(originalInstance, new object[] { });
    }
  
    [HarmonyPatch("OnServerTick")]
    [HarmonyPrefix]
    static void OnServerTick_Postfix(float dt, BlockEntityCharcoalPit __instance, ref double ___startingAfterTotalHours, 
      ref EnumCharcoalPitState ___state, ref double ___finishedAfterTotalHours, ref string ___startedByPlayerUid,
      ref float ___BurnHours)
    {
      if (!__instance.Lit)
        return;
      if (___startingAfterTotalHours <= __instance.Api.World.Calendar.TotalHours &&  ___state == EnumCharcoalPitState.Warmup)
      {
        ___finishedAfterTotalHours = __instance.Api.World.Calendar.TotalHours + (double)___BurnHours * FasterNatureModSystem.config.CharcoalPitSpeedRate;
        ___state = EnumCharcoalPitState.Sealed;
        __instance.MarkDirty();
      }

      if (___state == EnumCharcoalPitState.Warmup)
        return;
      
      object? originalInstance = (object)__instance;
      MethodInfo findHolesInPit = __instance.GetType().GetMethod("FindHolesInPit", BindingFlags.NonPublic | BindingFlags.Instance);
      HashSet<BlockPos> holesInPit = (HashSet<BlockPos>) findHolesInPit.Invoke(originalInstance, new object[] { });
      
      if (holesInPit == null)
        return;
      EnumCharcoalPitState state = ___state;
      if (holesInPit.Count > 0)
      {
        ___state = EnumCharcoalPitState.Unsealed;
        ___finishedAfterTotalHours = __instance.Api.World.Calendar.TotalHours + (double)___BurnHours * FasterNatureModSystem.config.CharcoalPitSpeedRate;
        float num = Math.Clamp((float)(1.0 - 0.10000000149011612 * (double)(holesInPit.Count - 1)), 0.5f, 1f);
        foreach (BlockPos pos in holesInPit)
        {
          BlockPos blockPos = pos.Copy();
          Block block = __instance.Api.World.BlockAccessor.GetBlock(pos);
          EntityPlayer byEntity = __instance.Api.World.PlayerByUid(___startedByPlayerUid)?.Entity ?? __instance.Api.World
            .NearestPlayer((double)__instance.Pos.X, (double)__instance.Pos.InternalY, (double)__instance.Pos.Z)?.Entity;
          IIgnitable ignitable = block.GetInterface<IIgnitable>(__instance.Api.World, pos);
          bool flag1 = byEntity != null;
          if (flag1)
          {
            EnumIgniteState? nullable = ignitable?.OnTryIgniteBlock((EntityAgent)byEntity, pos, 10f);
            bool flag2;
            if (nullable.HasValue)
            {
              switch (nullable.GetValueOrDefault())
              {
                case EnumIgniteState.Ignitable:
                case EnumIgniteState.IgniteNow:
                  flag2 = true;
                  goto label_16;
              }
            }

            flag2 = false;
            label_16:
            flag1 = flag2;
          }

          if (flag1)
          {
            if (__instance.Api.World.Rand.NextDouble() < (double)num)
            {
              EnumHandling handling = EnumHandling.PassThrough;
              ignitable.OnTryIgniteBlockOver((EntityAgent)byEntity, pos, 10f, ref handling);
            }
          }
          else if (block.BlockId != 0 && block.BlockId != __instance.charcoalPitId)
          {
            foreach (BlockFacing fromFacing in BlockFacing.ALLFACES)
            {
              fromFacing.IterateThruFacingOffsets(blockPos);
              if (__instance.Api.World.BlockAccessor.GetBlock(blockPos).BlockId == 0 &&
                  __instance.Api.World.Rand.NextDouble() < (double)num)
              {
                __instance.Api.World.BlockAccessor.SetBlock(__instance.fireBlockId, blockPos);
                __instance.Api.World.BlockAccessor.GetBlockEntity(blockPos)?.GetBehavior<BEBehaviorBurning>()
                  ?.OnFirePlaced(fromFacing, ___startedByPlayerUid);
              }
            }
          }
        }

        __instance.MarkDirty();
      }
      else
      {
        ___state = EnumCharcoalPitState.Sealed;
        if (state != ___state)
          __instance.MarkDirty();
        if (___finishedAfterTotalHours > __instance.Api.World.Calendar.TotalHours)
          return;
        
        MethodInfo convertPit = __instance.GetType().GetMethod("FindHolesInPit", BindingFlags.NonPublic | BindingFlags.Instance);
        convertPit.Invoke(originalInstance, new object[] { });
      }
    }
}
