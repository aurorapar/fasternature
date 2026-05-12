using System.Collections.Generic;
using Vintagestory.API.Server;

namespace FasterNature.Commands;

public static class Commands
{
    public static List<CustomCommand> ModCommands { get; } = new();

    public static void RegisterCommands(ICoreServerAPI api)
    {
        ModCommands.Add(
            new FasterNature(api)
        );
    }
}