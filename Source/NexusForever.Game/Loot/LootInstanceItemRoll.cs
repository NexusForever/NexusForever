using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Loot;
using NexusForever.Game.Static.Loot;
using NexusForever.Network.World.Message.Model.Loot;

namespace NexusForever.Game.Loot
{
    public class LootInstanceItemRoll : ILootInstanceItemRoll
    {
        public Identity Identity { get; private set; }
        public LootRollAction Action { get; private set; }
        public uint? Roll { get; private set; }

        public void Initialise(Identity identity, LootRollAction action, uint? roll)
        {
            Identity = identity;
            Action   = action;
            Roll     = roll;
        }

        public ServerLootWinner.LootRoll Build()
        {
            var roll = new ServerLootWinner.LootRoll
            {
                Identity = Identity.ToNetworkIdentity()
            };

            if (Action != LootRollAction.Pass)
                roll.Value = (int)Roll.Value + (Action == LootRollAction.Greed ? 0 : 100);

            return roll;
        }
    }
}
