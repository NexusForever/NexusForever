using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientUpdateActionSet, MessageDirection.Client)]
    public class ClientUpdateActionSet : IReadable
    {
        public List<uint> Actions { get; private set; } = new List<uint>();
        public List<(uint Action, byte Tier)> ActionTiers { get; private set; } = new List<(uint, byte)>();
        public byte ActionSetIndex { get; private set; }
        public List<ushort> AMPs { get; private set; } = new List<ushort>();
        
        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadByte(4u);
            for (uint i = 0u; i < count; i++)
                Actions.Add(reader.ReadUInt());

            ActionSetIndex = reader.ReadByte(3u);

            count = reader.ReadByte(5u);            
            for (uint i = 0u; i < count; i++)
                ActionTiers.Add((reader.ReadUInt(18u), reader.ReadByte()));

            count = reader.ReadByte(7u);
            for (uint i = 0u; i < count; i++)
                AMPs.Add(reader.ReadUShort());
        }
    }
}
