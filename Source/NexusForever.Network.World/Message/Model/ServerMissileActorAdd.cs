using NexusForever.Network.Message;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMissileActorAdd)]
    public class ServerMissileActorAdd : IWritable
    {
        public ushort Spell4ClientMissileId { get; set; }
        public uint UnitId_1 { get; set; }
        public Vector3 Position_Unit1 { get; set; }
        public uint UnitId_2 { get; set; }
        public Vector3 Position_Unit2 { get; set; }
        public uint MissileActorId { get; set; }
        public uint TimeElapsed { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Spell4ClientMissileId);
            writer.Write(UnitId_1);
            writer.WriteVector3(Position_Unit1);
            writer.Write(UnitId_2);
            writer.WriteVector3(Position_Unit2);
            writer.Write(MissileActorId);
            writer.Write(TimeElapsed);            
        }
    }
}
