using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMatchingQueue)]
    public class ClientMatchingQueue : IReadable
    {
        public MatchingMap MapData { get; private set; }
        public Role Roles { get; private set; }
        public uint Unknown1 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            MapData = new MatchingMap();
            MapData.Read(reader);

            Roles    = reader.ReadEnum<Role>(32u);
            Unknown1 = reader.ReadUInt();
        }
    }
}
