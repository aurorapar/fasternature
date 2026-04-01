using System.IO;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace FasterSmelts.Helpers;

public class Helpers
{
    public static bool IsServerSide(EnumAppSide side)
    {
        return side == EnumAppSide.Server;
    }

    public static void SaveConfig(ICoreServerAPI api)
    {
        string configPath = Path.Join(FasterSmeltsModSystem.ModName, FasterSmeltsModSystem.ModName + ".json");
        api.StoreModConfig(FasterSmeltsModSystem.config, configPath);
    }
}