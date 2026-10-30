using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    /// <summary>
    /// Confirms to the client that a cast it requested was accepted (opcode 0x07FD).
    /// </summary>
    /// <remarks>
    /// The client's handler for this opcode looks its pending cast up
    /// by (<see cref="ClientUniqueId"/>, <see cref="CastingId"/>), stores the server's casting id into
    /// it, moves it to the confirmed table and only then applies the position, yaw, telegraphs and
    /// primary target carried here. It is reached by every cast that goes through the client's cast
    /// initiate routine, not only by a client-side interaction
    ///
    /// An id the client does not hold raises a client-side exception, so this message must only ever
    /// be sent back with an id the client itself supplied.
    /// </remarks>
    [Message(GameMessageOpcode.ServerSpellStartClientInteraction)]
    public class ServerSpellStartClientInteraction : IWritable
    {
        public uint ClientUniqueId { get; set; }
        public uint CastingId { get; set; }
        public uint CasterId { get; set; }
        public Position Position { get; set; } = new Position();
        /// <summary>
        /// Facing of the caster, in radians.
        /// </summary>
        /// <remarks>
        /// The client reads this field with the same 32-bit float primitive
        /// that its position reader calls three times, so the width was already right and only the
        /// type was wrong: an integer yaw decodes as a denormal angle. No sender set the field before
        /// this change.
        /// </remarks>
        public float Yaw { get; set; }

        public List<InitialPosition> InitialPositionData { get; set; } = new List<InitialPosition>();
        public List<TelegraphPosition> TelegraphPositionData { get; set; } = new List<TelegraphPosition>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ClientUniqueId);
            writer.Write(CastingId);
            writer.Write(CasterId);
            Position.Write(writer);
            writer.Write(Yaw);

            writer.Write(InitialPositionData.Count, 8u);
            InitialPositionData.ForEach(u => u.Write(writer));

            writer.Write(TelegraphPositionData.Count, 8u);
            TelegraphPositionData.ForEach(u => u.Write(writer));
        }
    }
}