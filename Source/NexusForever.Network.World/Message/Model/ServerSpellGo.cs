using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellGo)]
    public class ServerSpellGo : IWritable
    {
        public class MissileInfo : IWritable
        {
            public Position CasterPosition { get; set; } = new Position();
            public uint     MissileTravelTime { get; set; }
            public uint     TargetId { get; set; }
            public Position TargetPosition { get; set; } = new Position();
            public bool     HitPosition  { get; set; }

            public void Write(GamePacketWriter writer)
            {
                CasterPosition.Write(writer);
                writer.Write(MissileTravelTime);
                writer.Write(TargetId);
                TargetPosition.Write(writer);
                writer.Write(HitPosition);
            }
        }

        public uint ServerUniqueId { get; set; }
        public bool BIgnoreCooldown  { get; set; }
        public Position PrimaryDestination { get; set; } = new Position();

        public List<TargetInfo> TargetInfoData { get; set; } = new();
        public List<InitialPosition> InitialPositionData { get; set; } = new();
        public List<TelegraphPosition> TelegraphPositionData { get; set; } = new();
        public List<MissileInfo> MissileInfoData { get; set; } = new();

        public byte Phase { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ServerUniqueId);
            writer.Write(BIgnoreCooldown);
            PrimaryDestination.Write(writer);

            writer.Write(TargetInfoData.Count, 8u);
            TargetInfoData.ForEach(u => u.Write(writer));

            writer.Write(InitialPositionData.Count, 8u);
            InitialPositionData.ForEach(u => u.Write(writer));

            writer.Write(TelegraphPositionData.Count, 8u);
            TelegraphPositionData.ForEach(u => u.Write(writer));

            writer.Write(MissileInfoData.Count, 8u);
            MissileInfoData.ForEach(u => u.Write(writer));

            writer.Write(Phase);
        }
    }
}
