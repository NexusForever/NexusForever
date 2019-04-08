using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCharacterLogoutStart)]
    public class ServerCharacterLogoutStart : IWritable
    {
        public uint TimeTillLogout { get; set; }
        public bool Unknown0 { get; set; }
        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TimeTillLogout);
            writer.Write(Unknown0);
            writer.Write(Unknown1);
            writer.Write(Unknown2);

            // these are both probably arrays of uints
            byte[] unknown3 = new byte[120];
            writer.WriteBytes(unknown3);
            byte[] unknown4 = new byte[152];
            writer.WriteBytes(unknown4);
        }
    }
}
