using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientGuildRegister)]
    public class ClientGuildRegister : IReadable
    {
        public uint UnitId { get; set; }
        public GuildType GuildType { get; set; }
        public string GuildName { get; set; }
        public string MasterTitle { get; set; }
        public string CouncilTitle { get; set; }
        public string MemberTitle { get; set; }
        public GuildStandard GuildStandard { get; set; } = new GuildStandard();
        public bool Unknown0 { get; set; }

        public void Read(GamePacketReader reader)
        {
            UnitId = reader.ReadUInt();
            GuildType = (GuildType)reader.ReadByte(4u);
            GuildName = reader.ReadWideString();
            MasterTitle = reader.ReadWideString();
            CouncilTitle = reader.ReadWideString();
            MemberTitle = reader.ReadWideString();
            GuildStandard.Read(reader);
            Unknown0 = reader.ReadBit();
        }
    }
}
