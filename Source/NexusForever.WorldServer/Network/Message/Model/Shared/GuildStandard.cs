using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class GuildStandard : IWritable, IReadable
    {
        public class GuildStandardPart: IWritable, IReadable
        {
            public ushort GuildStandardPartId { get; set; }
            public ushort DyeColorRampId1 { get; set; }
            public ushort DyeColorRampId2 { get; set; }
            public ushort DyeColorRampId3 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(GuildStandardPartId, 14u);
                writer.Write(DyeColorRampId1, 14u);
                writer.Write(DyeColorRampId2, 14u);
                writer.Write(DyeColorRampId3, 14u);
            }

            public void Read(GamePacketReader reader)
            {
                GuildStandardPartId = reader.ReadUShort(14u);
                DyeColorRampId1 = reader.ReadUShort(14u);
                DyeColorRampId2 = reader.ReadUShort(14u);
                DyeColorRampId3 = reader.ReadUShort(14u);
            }
        }

        public GuildStandardPart BackgroundIcon { get; set; } = new GuildStandardPart();
        public GuildStandardPart ForegroundIcon { get; set; } = new GuildStandardPart();
        public GuildStandardPart ScanLines { get; set; } = new GuildStandardPart();

        public void Write(GamePacketWriter writer)
        {
            BackgroundIcon.Write(writer);
            ForegroundIcon.Write(writer);
            ScanLines.Write(writer);
        }

        public void Read(GamePacketReader reader)
        {
            BackgroundIcon.Read(reader);
            ForegroundIcon.Read(reader);
            ScanLines.Read(reader);
        }
    }
}
