using System.Collections.Immutable;
using NexusForever.Game.Static.Quest;
using NexusForever.GameTable.Model;
using NexusForever.Network.Session;

namespace NexusForever.Game.Abstract.Quest
{
    public interface ICommunicatorMessage
    {
        uint Id { get; }
        ushort QuestId { get; }
        CommunicatorMessagesEntry Entry { get; }
        CommunicatorKind Kind { get; }

        /// <summary>
        /// Returns if the server sends this message to the client; the client fires every other row itself.
        /// </summary>
        bool ServerPushed { get; }

        /// <summary>
        /// Returns if the client can receive this message now: the server pushes it on a quest event, or the client fires it.
        /// </summary>
        bool Reachable { get; }

        /// <summary>
        /// Returns if the message offers a quest.
        /// </summary>
        bool Delivers { get; }

        /// <summary>
        /// The row's quest/state pairs, all of which must hold.
        /// </summary>
        ImmutableArray<CommunicatorCondition> Conditions { get; }

        /// <summary>
        /// The quest events that arm the row.
        /// </summary>
        ImmutableArray<CommunicatorTrigger> Triggers { get; }

        /// <summary>
        /// Evaluate the row's conditions for <paramref name="player"/> in the client's order, skipping <paramref name="skip"/>.
        /// </summary>
        CommunicatorResult Evaluate(ICommunicatorPlayerView player, CommunicatorGates skip = CommunicatorGates.None);

        /// <summary>
        /// Send communicator message to <see cref="IGameSession"/>.
        /// </summary>
        void Send(IGameSession session, bool checkConditions = true);
    }
}
