using NexusForever.Game.Static.Entity.Movement.Command;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;

namespace NexusForever.Network.World.Entity.Command
{
    [EntityCommand(EntityCommand.SetModeKeys)]
    public class SetModeKeysCommand : IEntityCommandModel
    {
        public List<uint> Times { get; set; } = new();
        public List<ModeType> Modes { get; set; } = new();
        public uint Offset { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint Count = reader.ReadByte();

            for (int i = 0; i < Count; i++)
                Times.Add(reader.ReadUInt());

            for (int i = 0; i < Count; i++)
                Modes.Add(reader.ReadEnum<ModeType>(32));

            Offset = reader.ReadUInt();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Times.Count, 8u);

            foreach (uint u in Times)
                writer.Write(u);

            foreach (ModeType u in Modes)
                writer.Write(u, 32);

            writer.Write(Offset);
        }
    }
}
