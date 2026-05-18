using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Command.State;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetStateKeys)]
    public class SetStateKeysCommand : IEntityCommandModel
    {
        public List<uint> Times { get; set; } = new();
        public List<StateFlags> States { get; set; } = new();
        public uint Offset { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint Count = reader.ReadByte();

            for (int i = 0; i < Count; i++)
                Times.Add(reader.ReadUInt());

            for (int i = 0; i < Count; i++)
                States.Add(reader.ReadEnum<StateFlags>(32));

            Offset = reader.ReadUInt();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Times.Count, 8u);

            foreach (uint time in Times)
                writer.Write(time);

            foreach (StateFlags state in States)
                writer.Write(state, 32);

            writer.Write(Offset);
        }
    }
}
