using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ClientGuildRegister)]
    public class ClientGuildRegister : IReadable
    {
        public uint InteractionUnitId { get; private set; }
        public GuildType GuildType { get; private set; }
        public string GuildName { get; private set; }
        public string MasterTitle { get; private set; }
        public string CouncilTitle { get; private set; }
        public string MemberTitle { get; private set; }
        public GuildStandard GuildStandard { get; private set; } = new();
        public bool AlternateCost { get; private set; }

        public void Read(GamePacketReader reader)
        {
            InteractionUnitId = reader.ReadUInt();
            GuildType     = reader.ReadEnum<GuildType>(4u);
            GuildName     = reader.ReadWideString();
            MasterTitle   = reader.ReadWideString();
            CouncilTitle  = reader.ReadWideString();
            MemberTitle   = reader.ReadWideString();
            GuildStandard.Read(reader);
            AlternateCost = reader.ReadBit();
        }
    }
}
