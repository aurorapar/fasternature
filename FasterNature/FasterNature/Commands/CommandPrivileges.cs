using System.Collections.Generic;
using Vintagestory.API.Server;

namespace FasterNature.Commands;

public static class CommandPrivileges
{
    public static List<string> PublicPrivileges { get; } = new()
    {
        Privilege.chat
    };

    public static List<string> AdminPrivileges { get; } = new()
    {
        Privilege.controlserver
    };
}