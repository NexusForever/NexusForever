using System;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientRandomRollRequest)]
    public class ClientRandomRollRequest : IReadable
    {
        // Note that for /roll on the command line, 
        // the TargetPlayerIdentity's realmId and
        // characterId, as well as Unknown0 are 0.
        // As such, these fields are currently unused.
        public TargetPlayerIdentity TargetPlayerIdentity { get; } = new TargetPlayerIdentity();
        public uint MinRandom { get; private set; }
        public uint MaxRandom { get; private set; }
        public uint Unknown0 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TargetPlayerIdentity.Read(reader);
            MinRandom = reader.ReadUInt();
            MaxRandom = reader.ReadUInt();
            Unknown0 = reader.ReadUInt();
        }
    }
}