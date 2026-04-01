using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FasterSmelts.Config;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using static FasterSmelts.Helpers.Helpers;

namespace FasterSmelts.Commands;

using static CommandPrivileges;

public class FasterSmelts : CustomCommand
{
    public FasterSmelts(ICoreServerAPI api) : base(api, "fastersmelts", "Mod Configuration", AdminPrivileges, false, Logic)
    {
        var settingParser = api.ChatCommands.Parsers.Word(
            "setting", 
            Enum.GetValues(typeof(CustomConfig.CustomConfigSetting))
                .Cast<CustomConfig.CustomConfigSetting>()
                .Select(x => x.ToString())
                .ToArray()
        );
        var valueParser = api.ChatCommands.Parsers.OptionalDouble("rate value (Default: 1)");
        
        Command.WithArgs(settingParser, valueParser);
    }
    
    public static TextCommandResult Logic(TextCommandCallingArgs args)
    {
        var configSettings = Enum.GetValues(typeof(CustomConfig.CustomConfigSetting))
            .Cast<CustomConfig.CustomConfigSetting>()
            .Select(x => x.ToString())
            .ToList();

        string setting = (string) args.Parsers[0].GetValue();
        var customSetting = CustomConfig.GetCustomConfigSetting(setting);
        
        if (args.ArgCount == 1 || (double) args[1] == 0)
            return TextCommandResult.Success($"Current {setting} value: {CustomConfig.GetConfigSetting((CustomConfig.CustomConfigSetting) customSetting)}");
        
        double value = Math.Min(Math.Max(0.1, (double) args[1]), 100);
        
        CustomConfig.UpdateConfigSetting(FasterSmelts.Api, (CustomConfig.CustomConfigSetting) customSetting, value);
        return TextCommandResult.Success();
    }
}