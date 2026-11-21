using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ServerCinematicMissileActorAdd)]
    public class ServerCinematicMissileActorAdd : IWritable
    {
        public uint Delay { get; set; }
        public uint MissileActorUnitId{ get; set; }
        public uint FromUnitId { get; set; }
        public uint ToUnitId { get; set; }
        public uint Spell4ClientMissileId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(MissileActorUnitId);
            writer.Write(FromUnitId);
            writer.Write(ToUnitId);
            writer.Write(Spell4ClientMissileId);
        }
    }
}
