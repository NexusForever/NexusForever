using NexusForever.Database.Auth;
using NexusForever.Game.Abstract.Entitlement;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Account.Entitlement
{
    public interface IAccountEntitlement : IEntitlement, INetworkBuildable<ServerAccountEntitlement>, IDatabaseAuth
    {
    }
}