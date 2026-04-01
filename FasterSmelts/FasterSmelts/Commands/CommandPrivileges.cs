using System.Collections.Generic;
using Vintagestory.API.Server;

namespace FasterSmelts.Commands;

public static class CommandPrivileges
{
    public static List<string> PublicPrivileges { get; } = new List<string>()
    {
        Privilege.chat
    };

    public static List<string> AdminPrivileges { get; } = new List<string>()
    {
        Privilege.controlserver
    };
}