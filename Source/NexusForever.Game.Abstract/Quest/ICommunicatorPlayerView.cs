using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Abstract.Quest
{
    /// <summary>
    /// What communicator conditions read from a player.
    /// </summary>
    public interface ICommunicatorPlayerView
    {
        uint WorldId { get; }
        Class Class { get; }
        Race Race { get; }
        Faction Faction { get; }
        uint Level { get; }

        /// <summary>
        /// Returns if the player is in <paramref name="worldZoneId"/> or a zone inside it.
        /// </summary>
        bool IsInZone(uint worldZoneId);

        float GetReputation(Faction faction);

        /// <summary>
        /// Return the player's record of the quest, or null when there is none (or it is being deleted).
        /// </summary>
        CommunicatorQuestSnapshot? GetQuest(ushort questId);

        bool MeetsPrerequisite(uint prerequisiteId);

        /// <summary>
        /// Return why a call can't offer the quest, or <see cref="CommunicatorOfferBlock.None"/> when it can.
        /// </summary>
        CommunicatorOfferBlock CheckOffer(ushort questId);
    }
}
