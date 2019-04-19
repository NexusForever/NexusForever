using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    /// <summary>
    /// This is sent to clients when other entitiy's health is updated. This forces a refresh on other's health bars, unlike ServerPlayerHealthUpdate (0x092F).
    /// </summary>
    [Message(GameMessageOpcode.ServerEntityHealthUpdate)]
    public class ServerEntityHealthUpdate : IWritable
    {
        public uint UnitId { get; set; }
        public uint Health { get; set; }
        public bool Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Health);
            writer.Write(Unknown0);
        }
    }
}
