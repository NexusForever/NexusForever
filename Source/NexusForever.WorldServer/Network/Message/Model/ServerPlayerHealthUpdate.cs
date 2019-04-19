using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    /// <summary>
    /// This is sent to the Player being updated. Forces a refresh on the health bar of that user. UnitId always matches player ID of the client.
    /// </summary>
    [Message(GameMessageOpcode.ServerPlayerHealthUpdate)]
    public class ServerPlayerHealthUpdate : IWritable
    {
        public uint UnitId { get; set; }
        public uint Health { get; set; }
        public UpdateHealthMask Mask { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Health);
            writer.Write(Mask, 18u);
        }
    }
}
