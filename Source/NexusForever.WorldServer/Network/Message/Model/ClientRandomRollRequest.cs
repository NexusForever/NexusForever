using System;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientRandomRollRequest)]
    public class ClientRandomRollRequest : IReadable
    {
        // Note that for /roll on the command line, the first two fields below,
        // realmId, characterId, and Unknown0-3 are 0.
        // As such, these fields are currently unused.
        public ushort realmId { get; set; }
        public ulong characterId { get; set; }
        public int MinRandom { get; set; }
        public int MaxRandom { get; set; }
        public int Unknown0 { get; set; }
        public Random rnd = new Random();

        public void Read(GamePacketReader reader)
        {
            realmId = reader.ReadUShort(14u);
            characterId = reader.ReadULong();
            MinRandom = reader.ReadInt();
            MaxRandom = reader.ReadInt();
            Unknown0 = reader.ReadInt();
        }

    }
}