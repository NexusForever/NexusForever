using NexusForever.Game.Abstract.Entity;
using NexusForever.Network;

namespace NexusForever.Game.Abstract.Quest
{
    public interface ICommunicatorMessage
    {
        uint Id { get; }
        ushort QuestId { get; }

        /// <summary>
        /// Checks if <see cref="IPlayer"/> meets the required conditions for this quest to be added to their communicator.
        /// </summary>
        bool Meets(IPlayer player);

        /// <summary>
        /// Send communicator message to <see cref="IGameSession"/>.
        /// </summary>
        void Send(IGameSession session);
    }
}