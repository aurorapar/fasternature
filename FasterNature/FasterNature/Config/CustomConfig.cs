using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Server;
using static FasterNature.Helpers.Helpers;

namespace FasterNature.Config;

public class CustomConfig
{
    public class CustomConfigSettings
    {
        public double BeehiveSpeedRate = 1.0;
    }
    
    public enum CustomConfigSetting
    {
        BeehiveSpeedRate
    }

    public static void UpdateConfigSetting(ICoreServerAPI api, CustomConfigSetting setting, double value)
    {
        switch (setting)
        {
            case CustomConfigSetting.BeehiveSpeedRate:
                FasterNatureModSystem.config.BeehiveSpeedRate = value;
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
            case CustomConfigSetting.BeehiveSpeedRate:
                return FasterNatureModSystem.config.BeehiveSpeedRate;
                
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