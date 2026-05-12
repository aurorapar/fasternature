using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace FasterNature.Commands;

public class CustomCommand
{
    public delegate TextCommandResult CommandDelegate(TextCommandCallingArgs args);

    protected static ICoreServerAPI Api;
    protected IChatCommand Command;

    public CustomCommand(ICoreServerAPI api, string text, string description, List<string> privileges,
        bool requiresPlayer, CommandDelegate callback)
    {
        Text = text;
        Description = description;
        Privileges = privileges;
        RequiresPlayer = requiresPlayer;
        Callback = callback;
        Api = api;

        Command = Api.ChatCommands.Create(Text);

        Command.WithDescription(Description);

        foreach (var priv in Privileges)
            Command.RequiresPrivilege(priv);

        if (RequiresPlayer)
            Command.RequiresPlayer();

        Command.HandleWith(args => Callback(args));
    }

    public string Text { get; }
    public string Description { get; }
    public List<string> Privileges { get; }
    public bool RequiresPlayer { get; }
    public CommandDelegate Callback { get; }
}