using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellCastResult)]
    public class ServerSpellCastResult : IWritable
    {
        /// <summary>
        /// Unique id the client sent with the cast request this result answers.
        /// </summary>
        /// <remarks>
        /// Evidence tier: human-verified. The client's handler for this opcode walks its pending-cast
        /// list and matches each entry's own id against this field; on a match it shows the failure
        /// text and removes the pending cast. A result that leaves this at 0 matches nothing, so the
        /// failure text still appears but the cast stays pending on the client, with no timeout path
        /// to clear it.
        /// </remarks>
        public uint ClientSpellCastUniqueId { get; set; }
        public uint Spell4Id { get; set; }
        public CastResult CastResult { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ClientSpellCastUniqueId);
            writer.Write(Spell4Id, 18u);
            writer.Write(CastResult, 9u);
        }
    }
}
