using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Static.RBAC;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Shared;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Broadcast, "A collection of commands to broadcast server wide messages.", "broadcast")]
    public class BroadcastCommandCategory : CommandCategory
    {
        [Command(Permission.BroadcastMessage, "Broadcast message to all players on the server.", "message")]
        public void HandleBroadcastMessage(ICommandContext context,
            [Parameter("Tier of the message being broadcast.", ParameterFlags.None, typeof(EnumParameterConverter<BroadcastTier>))]
            BroadcastTier tier,
            [Parameter("Message to broadcast.")]
            string message)
        {
            // TODO: move commands to dependency injection...
            var networkManager = LegacyServiceProvider.Provider.GetService<INetworkManager<IWorldSession>>();
            foreach (IWorldSession session in networkManager)
            {
                session.EnqueueMessageEncrypted(new ServerRealmBroadcast
                {
                    Tier    = tier,
                    Message = message
                });
            }
        }
    }
}
