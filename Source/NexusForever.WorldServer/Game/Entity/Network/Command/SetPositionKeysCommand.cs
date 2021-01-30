using System.Collections.Generic;
using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetPositionKeys)]
    public class SetPositionKeysCommand : IEntityCommandModel
    {
        public List<uint> Times = new();
        public List<Position> Positions = new();

        public byte Type { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUShort(10u);
            for (int i = 0; i < count; i++)
                Times.Add(reader.ReadUInt());

            var p = new Position();
            for (int i = 0; i < count; i++)
            {
                p.Read(reader);
                Positions.Add(p);
            }

            Type   = reader.ReadByte(2u);
            Offset = reader.ReadUInt();
            Blend  = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Positions.Count, 10u);
            foreach (uint time in Times)
                writer.Write(time);

            Positions.ForEach(p => p.Write(writer));

            writer.Write(Type, 2u);
            writer.Write(Offset);
            writer.Write(Blend);
        }
    }
}
