using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    [Message(GameMessageOpcode.ClientRequestActionSetChanges)]
    public class ClientRequestActionSetChanges : IReadable
    {
        public class ActionTier : IReadable
        {
            public uint Action { get; private set; }
            public byte Tier { get; private set; }

            public void Read(GamePacketReader reader)
            {
                Action = reader.ReadUInt(18u);
                Tier = reader.ReadByte();
            }
        }

        public List<uint> Actions { get; } = [];
        public List<ActionTier> ActionTiers { get; } = [];
        public byte ActionSetIndex { get; private set; }
        public List<ushort> Amps { get; } = [];

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadByte(4u);
            for (uint i = 0u; i < count; i++)
                Actions.Add(reader.ReadUInt());

            ActionSetIndex = reader.ReadByte(3u);

            count = reader.ReadByte(5u);
            for (uint i = 0u; i < count; i++)
            {
                var actionTier = new ActionTier();
                actionTier.Read(reader);
                ActionTiers.Add(actionTier);
            }

            count = reader.ReadByte(7u);
            for (uint i = 0u; i < count; i++)
                Amps.Add(reader.ReadUShort());
        }
    }
}
