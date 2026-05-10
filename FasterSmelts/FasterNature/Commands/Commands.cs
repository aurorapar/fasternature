using System.Collections.Generic;
using Vintagestory.API.Server;

namespace FasterSmelts.Commands;

public static class Commands
{
    public static List<CustomCommand> ModCommands { get; } = new List<CustomCommand>();

    public static void RegisterCommands(ICoreServerAPI api)
    {
        ModCommands.Add(
            new FasterSmelts(api)
        );
    }
}