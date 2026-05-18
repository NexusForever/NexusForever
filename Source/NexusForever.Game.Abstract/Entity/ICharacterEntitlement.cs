using NexusForever.Database.Character;
using NexusForever.Game.Abstract.Entitlement;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ICharacterEntitlement : IEntitlement, INetworkBuildable<ServerEntitlement>, IDatabaseCharacter
    {
    }
}