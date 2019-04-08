namespace NexusForever.Shared.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerHello)]
    public class ServerHello : IWritable
    {
        public uint AuthVersion { get; set; }
        public uint RealmId { get; set; }
        public uint Unknown8 { get; set; }
        public uint UnknownC { get; set; }
        public ulong Unknown10 { get; set; }
        public ushort Unknown18 { get; set; }
        public byte Unknown1C { get; set; }
        public uint AuthMessage { get; set; }
        public uint Unknown24 { get; set; }
        public ulong Unknown28 { get; set; }
        public uint Unknown30 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AuthVersion);
            writer.Write(RealmId);
            writer.Write(Unknown8);
            writer.Write(UnknownC);
            writer.Write(Unknown10);
            writer.Write(Unknown18);
            writer.Write(Unknown1C, 5);
            writer.Write(AuthMessage);
            writer.Write(Unknown24);
            writer.Write(Unknown28);
            writer.Write(Unknown30);
        }
    }
}
