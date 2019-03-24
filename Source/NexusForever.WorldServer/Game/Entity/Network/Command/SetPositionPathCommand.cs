using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetPositionPath)]
    public class SetPositionPathCommand : IEntityCommand
    {
        public List<Position> Positions = new List<Position>();

        public ushort Speed { get; set; }
        public byte Type { get; set; }
        public byte Mode { get; set; }
        public uint Offset { get; set; }
        public bool Blend { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint Count = reader.ReadUShort(10u);
            Position position = new Position();
            
            for (int i = 0; i < Count; i++)
            {
                position.Read(reader);
                Positions.Add(position);
            }

            Speed = reader.ReadUShort();
            Type = reader.ReadByte(2u);
            Mode = reader.ReadByte(4u);
            Offset = reader.ReadUInt();
            Blend = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Positions.Count, 10u);
            Positions.ForEach(p => p.Write(writer));

            writer.Write(Speed);
            writer.Write(Type, 2u);
            writer.Write(Mode, 4u);
            writer.Write(Offset);
            writer.Write(Blend);
        }
    }
}
