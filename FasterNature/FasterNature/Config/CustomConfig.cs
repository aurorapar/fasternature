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
        public double CharcoalPitSpeedRate = 1.0;
        public double PitKilnSpeedRate = 1.0;
        public double BloomerySpeedRate = 1.0;
        public double BeehiveKilnSpeedRate = 1.0;
        public double CementationFurnaceSpeedRate = 1.0;    
    }
    
    public enum CustomConfigSetting
    {
        CharcoalPitSpeedRate,
        PitKilnSpeedRate,
        BloomerySpeedRate,
        BeehiveKilnSpeedRate,
        CementationFurnaceSpeedRate
    }

    public static void UpdateConfigSetting(ICoreServerAPI api, CustomConfigSetting setting, double value)
    {
        switch (setting)
        {
            case CustomConfigSetting.CharcoalPitSpeedRate:
                FasterNatureModSystem.config.CharcoalPitSpeedRate = value;
                break;
            
            case CustomConfigSetting.PitKilnSpeedRate:
                FasterNatureModSystem.config.PitKilnSpeedRate = value;
                break;
            
            case CustomConfigSetting.BloomerySpeedRate:
                FasterNatureModSystem.config.BloomerySpeedRate = value;
                break;
                
            case CustomConfigSetting.BeehiveKilnSpeedRate:
                FasterNatureModSystem.config.BeehiveKilnSpeedRate = value;
                break;
                
            case CustomConfigSetting.CementationFurnaceSpeedRate:
                FasterNatureModSystem.config.CementationFurnaceSpeedRate = value;
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
            case CustomConfigSetting.CharcoalPitSpeedRate:
                return FasterNatureModSystem.config.CharcoalPitSpeedRate;
            
            case CustomConfigSetting.PitKilnSpeedRate:
                return FasterNatureModSystem.config.PitKilnSpeedRate;
            
            case CustomConfigSetting.BloomerySpeedRate:
                return FasterNatureModSystem.config.BloomerySpeedRate;
                
            case CustomConfigSetting.BeehiveKilnSpeedRate:
                return FasterNatureModSystem.config.BeehiveKilnSpeedRate;
                
            case CustomConfigSetting.CementationFurnaceSpeedRate:
                return FasterNatureModSystem.config.CementationFurnaceSpeedRate;
                
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