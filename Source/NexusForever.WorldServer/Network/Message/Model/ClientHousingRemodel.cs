using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingRemodel)]
    public class ClientHousingRemodel : IReadable
    {
        public TargetPlayerIdentity PlayerIdentity { get; } = new TargetPlayerIdentity();
        public uint WallpaperId { get; private set; }
        public uint RoofDecorInfoId { get; private set; }
        public uint EntrywayDecorInfoId { get; private set; }
        public uint DoorDecorInfoId { get; private set; }
        public uint SkyWallpaperId { get; private set; }
        public uint MusicId { get; private set; }
        public uint GroundWallpaperId { get; private set; }
        public byte Operation { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PlayerIdentity.Read(reader);
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
