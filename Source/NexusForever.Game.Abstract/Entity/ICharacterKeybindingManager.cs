using NexusForever.Database.Character;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ICharacterKeybindingManager : IDatabaseCharacter, INetworkBuildable<BiInputKeySet>
    {
        void Update(BiInputKeySet inputKeySet);
    }
}
