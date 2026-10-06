using NexusForever.Game.Static.Group;
using InternalGroup = NexusForever.Network.Internal.Message.Group.Shared.Group;

namespace NexusForever.Game.Abstract.Group
{
    public interface IGroup : IEnumerable<IGroupMember>
    {
        ulong Id { get; }
        LootRule NormalRule { get; }
        LootRule ThresholdRule { get; }
        LootThreshold ThresholdQuality { get; }
        HarvestLootRule HarvestRule { get; }
        Identity Leader { get; }

        void Initialise(InternalGroup internalGroup);

        IGroupMember GetMember(Identity identity);
    }
}
