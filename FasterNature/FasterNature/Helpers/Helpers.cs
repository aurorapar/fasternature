using System.IO;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace FasterNature.Helpers;

public class Helpers
{
    public static bool IsServerSide(EnumAppSide side)
    {
        return side == EnumAppSide.Server;
    }

    public static void SaveConfig(ICoreServerAPI api)
    {
        var configPath = Path.Join(FasterNatureModSystem.ModName, FasterNatureModSystem.ModName + ".json");
        api.StoreModConfig(FasterNatureModSystem.config, configPath);
    }
}