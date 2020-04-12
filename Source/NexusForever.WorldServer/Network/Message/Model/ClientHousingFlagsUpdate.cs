using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingFlagsUpdate)]
    public class ClientHousingFlagsUpdate : IReadable
    {
        public TargetPlayerIdentity Identity { get; } = new TargetPlayerIdentity();
        public ResidenceFlags Flags { get; private set; }
        public uint Unknown0 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Identity.Read(reader);
            Flags = reader.ReadEnum<ResidenceFlags>(3u);
            Unknown0 = reader.ReadUInt();
        }
    }
}