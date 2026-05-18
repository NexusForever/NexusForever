namespace NexusForever.Network.Message
{
    public interface IMessageManager
    {
        /// <summary>
        /// Register message of <see cref="Type"/>.
        /// </summary>
        void RegisterMessage(Type type);

        /// <summary>
        /// Register message handler of <see cref="Type"/>.
        /// </summary>
        void RegisterMessageHandler(Type type);

        /// <summary>
        /// Get message <see cref="Type"/> for supplied <see cref="GameMessageOpcode"/>.
        /// </summary>
        Type GetMessageType(GameMessageOpcode opcode);

        /// <summary>
        /// Get message <see cref="GameMessageOpcode"/> for supplied <see cref="IWritable"/>.
        /// </summary>
        GameMessageOpcode? GetOpcode(IWritable message);

        /// <summary>
        /// Get message handler <see cref="Type"/> for supplied <see cref="GameMessageOpcode"/>.
        /// </summary>
        Type GetMessageHandlerType(GameMessageOpcode opcode);

        /// <summary>
        /// Get message handler delegate for supplied <see cref="GameMessageOpcode"/>.
        /// </summary>
        MessageHandlerDelegate GetMessageHandlerDelegate(GameMessageOpcode opcode);
    }
}
