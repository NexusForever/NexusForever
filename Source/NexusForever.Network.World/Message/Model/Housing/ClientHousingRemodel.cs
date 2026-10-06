using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ClientHousingRemodel)]
    public class ClientHousingRemodel : IReadable
    {
        public Identity TargetResidence { get; } = new();
        public uint WallpaperId { get; private set; }
        public uint RoofDecorInfoId { get; private set; }
        public uint EntrywayDecorInfoId { get; private set; }
        public uint DoorDecorInfoId { get; private set; }
        public uint SkyWallpaperId { get; private set; }
        public uint MusicId { get; private set; }
        public uint GroundWallpaperId { get; private set; }
        public byte Operation { get; private set; } // always set to 1, can be ignored

        public void Read(GamePacketReader reader)
        {
            TargetResidence.Read(reader);
            WallpaperId         = reader.ReadUInt();
            RoofDecorInfoId     = reader.ReadUInt();
            EntrywayDecorInfoId = reader.ReadUInt();
            DoorDecorInfoId     = reader.ReadUInt();
            SkyWallpaperId      = reader.ReadUInt();
            MusicId             = reader.ReadUInt();
            GroundWallpaperId   = reader.ReadUInt();
            Operation           = reader.ReadByte(3u);
        }
    }
}
