using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Game.Static.Option;
using NexusForever.Network.World.Message.Model.Option;

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