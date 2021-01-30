using System.Collections.Generic;
using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetMoveKeys)]
    public class SetMoveKeysCommand : IEntityCommandModel
    {
        public List<uint> Times = new();
        public List<Move> Moves = new();

        public byte Type { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUShort(10u);
            for (int i = 0; i < count; i++)
                Times.Add(reader.ReadUInt());

            var move = new Move();
            for (int i = 0; i < count; i++)
            {
                move.Read(reader);
                Moves.Add(move);
            }

            Type   = reader.ReadByte(2u);
            Offset = reader.ReadUInt();
            Blend  = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Times.Count, 10u);

            foreach (uint time in Times)
                writer.Write(time);

            Moves.ForEach(m => m.Write(writer));

            writer.Write(Type, 2u);
            writer.Write(Offset);
            writer.Write(Blend);
        }
    }
}
