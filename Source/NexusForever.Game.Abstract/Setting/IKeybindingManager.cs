using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Setting
{
    public interface IKeybindingManager : IDatabaseAuth, IDatabaseCharacter
    {
        void SaveKeybinding(BiInputKeySet biInputKeySet);
        void SendInitialPackets();
        void SendInputKeySet();
        void SendInputKeySet(ulong characterId);
    }
}