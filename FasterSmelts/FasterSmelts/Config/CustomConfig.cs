using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Server;
using static FasterSmelts.Helpers.Helpers;

namespace FasterSmelts.Config;

public class CustomConfig
{
    public class CustomConfigSettings
    {
        public double BloomerySpeedRate = 1.0;
        public double BeehiveKilnSpeedRate = 1.0;
        public double CementationFurnaceSpeedRate = 1.0;    
    }
    
    public enum CustomConfigSetting
    {
        BloomerySpeedRate,
        BeehiveKilnSpeedRate,
        CementationFurnaceSpeedRate
    }

    public static void UpdateConfigSetting(ICoreServerAPI api, CustomConfigSetting setting, double value)
    {
        switch (setting)
        {
            case CustomConfigSetting.BloomerySpeedRate:
                FasterSmeltsModSystem.config.BloomerySpeedRate = value;
                break;
                
            case CustomConfigSetting.BeehiveKilnSpeedRate:
                FasterSmeltsModSystem.config.BeehiveKilnSpeedRate = value;
                break;
                
            case CustomConfigSetting.CementationFurnaceSpeedRate:
                FasterSmeltsModSystem.config.CementationFurnaceSpeedRate = value;
                break;
                
            _default:
                throw new ArgumentException($"Unknown config setting {setting}");
        }
        
        SaveConfig(api);
    }
    
    public static double GetConfigSetting(CustomConfigSetting setting)
    {
        switch (setting)
        {
            case CustomConfigSetting.BloomerySpeedRate:
                return FasterSmeltsModSystem.config.BloomerySpeedRate;
                
            case CustomConfigSetting.BeehiveKilnSpeedRate:
                return FasterSmeltsModSystem.config.BeehiveKilnSpeedRate;
                
            case CustomConfigSetting.CementationFurnaceSpeedRate:
                return FasterSmeltsModSystem.config.CementationFurnaceSpeedRate;
                
            _default:
                throw new ArgumentException($"Unknown config setting {setting}");
        }
        
        throw new ArgumentException($"Unknown config setting {setting}");
    }

    public static CustomConfigSetting? GetCustomConfigSetting(string setting)
    {
        CustomConfigSetting? match = null;
        var candidates = Enum.GetValues(typeof(CustomConfigSetting))
            .Cast<CustomConfigSetting>();
        foreach(var candidate in candidates)
            if (candidate.ToString().ToLower().Equals(setting.ToLower()))
            {
                match = candidate;
                break;
            }
        return match;
    }
}