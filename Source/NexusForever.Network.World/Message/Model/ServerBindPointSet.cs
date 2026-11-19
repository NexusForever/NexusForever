using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    /// <summary>
    /// Set the bind point (BindPoint id) of the player.
    /// </summary>
    /// <remarks>
    /// The client only offers Recall - Transmat once it knows a bind point, ServerPlayerCreate sends the same value at login.
    /// </remarks>
    [Message(GameMessageOpcode.ServerBindPointSet)]
    public class ServerBindPointSet : IWritable
    {
        public ushort BindPointId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BindPointId);
        }
    }
}
