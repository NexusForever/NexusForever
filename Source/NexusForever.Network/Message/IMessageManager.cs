namespace NexusForever.Network.Message
{
    public interface IMessageManager
    {
        /// <summary>
        /// Initialise <see cref="IMessageManager"/> and any related resources.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Return <see cref="IReadable"/> model for incoming packet with <see cref="GameMessageOpcode"/>.
        /// </summary>
        IReadable GetMessage(GameMessageOpcode opcode);

        /// <summary>
        /// Return <see cref="GameMessageOpcode"/> for outgoing <see cref="IWritable"/> model.
        /// </summary>
        bool GetOpcode(IWritable message, out GameMessageOpcode opcode);

        /// <summary>
        /// Return <see cref="MessageHandlerDelegate"/> delegate for incoming packet with <see cref="GameMessageOpcode"/>.
        /// </summary>
        MessageHandlerDelegate GetMessageHandler(GameMessageOpcode opcode);
    }
}