using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetRotationKeys)]
    public class SetRotationKeysCommand : IEntityCommand
    {
        public List<uint> Times = new List<uint>();
        public List<Position> Rotations = new List<Position>();

        public byte Type { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint Count = reader.ReadUShort(10u);
            Position r = new Position();

            for (int i = 0; i < Count; i++)
                Times.Add(reader.ReadUInt());

            for (int i = 0; i < Count; i++)
            {
                r.Read(reader);
                Rotations.Add(r);
            }
            Type = reader.ReadByte(2u);
            Offset = reader.ReadUInt();
            Blend = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Times.Count, 10u);
            foreach (var time in Times)
                writer.Write(time);

            Rotations.ForEach(r => r.Write(writer));

            writer.Write(Type, 2u);
            writer.Write(Offset);
            writer.Write(Blend);
        }
    }
}
