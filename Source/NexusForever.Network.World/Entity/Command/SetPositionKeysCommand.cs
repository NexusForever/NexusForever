using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Command;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetPositionKeys)]
    public class SetPositionKeysCommand : IEntityCommandModel
    {
        public List<uint> Times { get; set; } = new();
        public List<Vector3> Positions { get; set; } = new();
        public byte Type { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUShort(10u);
            for (int i = 0; i < count; i++)
                Times.Add(reader.ReadUInt());

            for (int i = 0; i < count; i++)
                Positions.Add(reader.ReadVector3());

            Type   = reader.ReadByte(2u);
            Offset = reader.ReadUInt();
            Blend  = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Positions.Count, 10u);
            foreach (uint time in Times)
                writer.Write(time);

            Positions.ForEach(writer.WriteVector3);

            writer.Write(Type, 2u);
            writer.Write(Offset);
            writer.Write(Blend);
        }
    }
}
