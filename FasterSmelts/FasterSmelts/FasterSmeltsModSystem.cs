using System;
using System.IO;
using FasterSmelts.Config;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using static FasterSmelts.Helpers.Helpers;
using static FasterSmelts.Commands.Commands;

namespace FasterSmelts;

public class FasterSmeltsModSystem : ModSystem
{
    public static CustomConfig.CustomConfigSettings config;
    public static string ModName = "FasterSmelts";
    
    public override void Start(ICoreAPI api)
    {
        LoadConfig(api);
    }
    
    public override bool ShouldLoad(EnumAppSide side)
    {
        return IsServerSide(side);
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
        RegisterCommands(api);
    }

    public void LoadConfig(ICoreAPI api)
    {
        string configPath = Path.Join(ModName, ModName + ".json");
        try
        {
            config = api.LoadModConfig<CustomConfig.CustomConfigSettings>(configPath);
            if (config == null) 
                config = new CustomConfig.CustomConfigSettings();

            SaveConfig((ICoreServerAPI) api);
        }
        catch (Exception e)
        {
            Mod.Logger.Error("Could not load config! Loading default settings instead.");
            Mod.Logger.Error(e);
            config = new CustomConfig.CustomConfigSettings();
        }
    }
}