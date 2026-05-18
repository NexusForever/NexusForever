using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientStatisticsWindowOpen)]
    public class ClientStatisticsWindowOpen : IReadable
    {
        public uint Always35 { get; private set; }
        public uint WindowId { get; private set; }
        public ulong CharacterId { get; private set; }
        public ulong Always0_1 { get; private set; }
        public ulong TimeSpentOpen { get; private set; }
        public ulong Always0_2 { get; private set; }
        public ulong Always0_3 { get; private set; }
        public ulong Always0_4 { get; private set; }
        public string WindowName { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Always35 = reader.ReadUInt(6);
            WindowId = reader.ReadUInt(0xA);
            CharacterId = reader.ReadULong();
            Always0_1 = reader.ReadULong();
            TimeSpentOpen = reader.ReadULong();
            Always0_2 = reader.ReadULong();
            Always0_3 = reader.ReadULong();
            Always0_4 = reader.ReadULong();
            WindowName = reader.ReadWideString();
        }
    }
}
