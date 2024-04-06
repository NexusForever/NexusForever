using System.Numerics;
using NexusForever.Game.Static.Entity.Movement.Command;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetMoveKeys)]
    public class SetMoveKeysCommand : IEntityCommandModel
    {
        public List<uint> Times { get; set; } = new();
        public List<Vector3> Moves { get; set; } = new();
        public byte Type { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUShort(10u);
            for (int i = 0; i < count; i++)
                Times.Add(reader.ReadUInt());

            for (int i = 0; i < count; i++)
                Moves.Add(reader.ReadPackedVector3());

            Type   = reader.ReadByte(2u);
            Offset = reader.ReadUInt();
            Blend  = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Times.Count, 10u);

            foreach (uint time in Times)
                writer.Write(time);

            Moves.ForEach(writer.WritePackedVector3);

            writer.Write(Type, 2u);
            writer.Write(Offset);
            writer.Write(Blend);
        }
    }
}
