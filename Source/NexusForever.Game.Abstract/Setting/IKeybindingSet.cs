using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Game.Static.Setting;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Setting
{
    public interface IKeybindingSet : IDatabaseCharacter, IDatabaseAuth, IEnumerable<IKeybinding>
    {
        uint Count { get; }
        InputSets InputSet { get; }
        ulong Owner { get; }

        void Update(BiInputKeySet biInputKeySet);
    }
}