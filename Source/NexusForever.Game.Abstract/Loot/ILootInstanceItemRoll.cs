using NexusForever.Game.Static.Loot;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Loot;

namespace NexusForever.Game.Abstract.Loot
{
    public interface ILootInstanceItemRoll : INetworkBuildable<ServerLootWinner.LootRoll>
    {
        Identity Identity { get; }
        LootRollAction Action { get; }
        uint? Roll { get; }

        void Initialise(Identity identity, LootRollAction action, uint? roll);
    }
}
