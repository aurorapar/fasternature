using System;
using System.Linq;
using FasterNature.Config;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace FasterNature.Commands;

using static CommandPrivileges;

public class FasterNature : CustomCommand
{
    public FasterNature(ICoreServerAPI api) : base(api, "fasternature", "Mod Configuration", AdminPrivileges, false,
        Logic)
    {
        var settingParser = api.ChatCommands.Parsers.Word(
            "setting",
            Enum.GetValues(typeof(CustomConfig.CustomConfigSetting))
                .Cast<CustomConfig.CustomConfigSetting>()
                .Select(x => x.ToString())
                .ToArray()
        );
        var valueParser = api.ChatCommands.Parsers.OptionalDouble("rate value");

        Command.WithArgs(settingParser, valueParser);
    }

    public static TextCommandResult Logic(TextCommandCallingArgs args)
    {
        var configSettings = Enum.GetValues(typeof(CustomConfig.CustomConfigSetting))
            .Cast<CustomConfig.CustomConfigSetting>()
            .Select(x => x.ToString())
            .ToList();

        var setting = (string)args.Parsers[0].GetValue();
        var customSetting = CustomConfig.GetCustomConfigSetting(setting);
        if (customSetting is null)
            return TextCommandResult.Error($"Invalid config setting '{setting}'. Valid options:" +
                                           string.Join(", ", configSettings));

        if (args.ArgCount == 1 || (double)args[1] == 0)
            return TextCommandResult.Success(
                $"Current {setting} value: {CustomConfig.GetConfigSetting((CustomConfig.CustomConfigSetting)customSetting)}");

        var value = Math.Min(Math.Max(0.01, (double)args[1]), 100);

        CustomConfig.UpdateConfigSetting(Api, (CustomConfig.CustomConfigSetting)customSetting, value);
        return TextCommandResult.Success($"{setting} is now {value}");
    }
}