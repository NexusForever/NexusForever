using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Utility
{
    [Message(GameMessageOpcode.ClientRandomRollRequest)]
    public class ClientRandomRollRequest : IReadable
    {
        // Note that for /roll on the command line, 
        // the TargetPlayerIdentity's realmId and
        // characterId, as well as Unknown0 are 0.
        // As such, these fields are currently unused.
        public Identity UnusedIdentity { get; } = new();
        public uint MinRandom { get; private set; }
        public uint MaxRandom { get; private set; }
        public uint Unused { get; private set; }

        public void Read(GamePacketReader reader)
        {
            UnusedIdentity.Read(reader);
            MinRandom = reader.ReadUInt();
            MaxRandom = reader.ReadUInt();
            Unused = reader.ReadUInt();
        }
    }
}
