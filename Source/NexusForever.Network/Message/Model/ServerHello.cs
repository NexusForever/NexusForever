namespace NexusForever.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerHello)]
    public class ServerHello : IWritable
    {
        public uint AuthVersion { get; set; }
        public uint RealmId { get; set; }
        public uint RealmGroupId { get; set; } // unused by client
        public uint RealmGroupEnum { get; set; } //  unused by client
        public ulong StartupTime { get; set; } //  unused by client
        public ushort ListenPort { get; set; } //  unused by client
        public byte ConnectionType { get; set; } // unused by client
        public uint AuthMessage { get; set; }
        public uint ProcessId { get; set; } //  unused by client
        public ulong ProcessCreationTime { get; set; } //  unused by client
        public uint Unused { get; set; }

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
            writer.Write(Unused);
        }
    }
}
