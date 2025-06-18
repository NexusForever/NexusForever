using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerDialogStart)]
    public class ServerDialogStart : IWritable
    {
        public uint DialogUnitId { get; set; }
        public bool Unused { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(DialogUnitId);
            writer.Write(Unused);
        }
    }
}
