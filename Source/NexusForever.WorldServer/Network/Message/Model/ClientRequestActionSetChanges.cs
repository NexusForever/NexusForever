using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientRequestActionSetChanges, MessageDirection.Client)]
    public class ClientRequestActionSetChanges : IReadable
    {
        public class ActionTier : IReadable
        {
            public uint Action { get; private set; }
            public byte Tier { get; private set; }

            public void Read(GamePacketReader reader)
            {
                Action = reader.ReadUInt(18u);
                Tier   = reader.ReadByte();
            }
        }

        public List<uint> Actions { get; } = new List<uint>();
        public List<ActionTier> ActionTiers { get; } = new List<ActionTier>();
        public byte ActionSetIndex { get; private set; }
        public List<ushort> Amps { get; } = new List<ushort>();
        
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
