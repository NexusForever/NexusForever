using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Entity;
using NexusForever.Game.Map;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Quest
{
    /// <summary>
    /// A player as communicator conditions see it. Takes the <see cref="QuestManager"/> directly because the player's
    /// quest manager isn't assigned yet while it is being built.
    /// </summary>
    public sealed class PlayerCommunicatorView : ICommunicatorPlayerView
    {
        private readonly IPlayer player;
        private readonly QuestManager questManager;

        public PlayerCommunicatorView(IPlayer player, QuestManager questManager)
        {
            this.player       = player;
            this.questManager = questManager;
        }

        public uint WorldId => player.Map?.Entry.Id ?? 0u;
        public Class Class => player.Class;
        public Race Race => player.Race;
        public Faction Faction => player.Faction1;
        public uint Level => player.Level;

        public bool IsInZone(uint worldZoneId)
        {
            return WorldZoneHierarchy.Contains(player.Zone?.Id ?? 0u, worldZoneId);
        }

        public float GetReputation(Faction faction)
        {
            return player.ReputationManager.GetReputation(faction)?.Amount ?? 0f;
        }

        public CommunicatorQuestSnapshot? GetQuest(ushort questId)
        {
            return questManager.GetCommunicatorSnapshot(questId);
        }

        public bool MeetsPrerequisite(uint prerequisiteId)
        {
            return PrerequisiteManager.Instance.Meets(player, prerequisiteId);
        }

        public CommunicatorOfferBlock CheckOffer(ushort questId)
        {
            return questManager.GetCallOfferBlock(questId);
        }
    }
}
