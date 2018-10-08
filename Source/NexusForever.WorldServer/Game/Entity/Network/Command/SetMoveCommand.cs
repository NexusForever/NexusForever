using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Entity.Network.Command
{
    [EntityCommand(EntityCommand.SetMove)]
    public class SetMoveCommand : IEntityCommand
    {
        public ushort Unknown0 { get; set; }
        public ushort Unknown1 { get; set; }
        public ushort Unknown2 { get; set; }
        public bool Unknown3 { get; set; }

        public void Read(GamePacketReader reader)
        {
            Unknown0 = reader.ReadUShort();
            Unknown1 = reader.ReadUShort();
            Unknown2 = reader.ReadUShort();
            Unknown3 = reader.ReadBit();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
        }
    }
}
