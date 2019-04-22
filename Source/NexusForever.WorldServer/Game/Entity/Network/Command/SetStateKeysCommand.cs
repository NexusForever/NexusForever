using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetStateKeys)]
    public class SetStateKeysCommand : IEntityCommandModel
    {
        public List<uint> Times = new List<uint>();
        public List<uint> States = new List<uint>();

        public uint Offset { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint Count = reader.ReadByte();

            for (int i = 0; i < Count; i++)
                Times.Add(reader.ReadUInt());

            for (int i = 0; i < Count; i++)
                States.Add(reader.ReadUInt());

            Offset = reader.ReadUInt();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Times.Count, 8u);

            foreach (var time in Times)
                writer.Write(time);

            foreach (var state in States)
                writer.Write(state);

            writer.Write(Offset);
        }
    }
}
