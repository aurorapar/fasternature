using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace FasterNature.Messenger;

public static class Messenger
{
    public static ICoreServerAPI Api;

    public static void MessagePlayers(string message)
    {
        if (Api is null)
            return;
        
        foreach (var player in Api.World.AllOnlinePlayers)
        {
            if (player is IServerPlayer serverPlayer)
            {
                serverPlayer.SendMessage(0, message, EnumChatType.Notification);
            }
        }
    }
}