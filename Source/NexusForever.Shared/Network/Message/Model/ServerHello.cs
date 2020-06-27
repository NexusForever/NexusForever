namespace NexusForever.Shared.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerHello)]
    public class ServerHello : IWritable
    {
        public uint AuthVersion { get; set; }
        public uint RealmId { get; set; }
        public uint RealmGroupId { get; set; }
        public uint RealmGroupEnum { get; set; }
        public ulong StartupTime { get; set; }
        public ushort ListenPort { get; set; }
        public byte ConnectionType { get; set; }
        public uint AuthMessage { get; set; }
        public uint ProcessId { get; set; }
        public ulong ProcessCreationTime { get; set; }
        public uint Unknown30 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AuthVersion);
            writer.Write(RealmId);
            writer.Write(RealmGroupId);
            writer.Write(RealmGroupEnum);
            writer.Write(StartupTime);
            writer.Write(ListenPort);
            writer.Write(ConnectionType, 5);
            writer.Write(AuthMessage);
            writer.Write(ProcessId);
            writer.Write(ProcessCreationTime);
            writer.Write(Unknown30);
        }
    }
}
