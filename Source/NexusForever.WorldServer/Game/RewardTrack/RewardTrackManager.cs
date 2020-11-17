using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.RewardTrack.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexusForever.WorldServer.Game.RewardTrack
{
    public class RewardTrackManager : ISaveAuth, ISaveCharacter
    {
        private readonly WorldSession session;

        private readonly Dictionary<RewardTrackType, List<AccountRewardTrack>> accountRewardTracks 
            = new Dictionary<RewardTrackType, List<AccountRewardTrack>>();
        private readonly Dictionary<uint, CharacterRewardTrack> characterRewardTracks 
            = new Dictionary<uint, CharacterRewardTrack>();

        /// <summary>
        /// Create a new <see cref="RewardTrackManager"/> from an existing database model.
        /// </summary>
        public RewardTrackManager(WorldSession session, AccountModel model)
        {
            this.session = session;

            foreach (AccountRewardTrackModel rewardTrackModel in model.AccountRewardTrack)
            {
                RewardTrackEntry entry = GameTableManager.Instance.RewardTrack.GetEntry(rewardTrackModel.RewardTrackId);
                if (entry == null)
                    throw new InvalidOperationException(nameof(rewardTrackModel.RewardTrackId));

                if (!accountRewardTracks.ContainsKey((RewardTrackType)entry.RewardTrackTypeEnum))
                    accountRewardTracks.Add((RewardTrackType)entry.RewardTrackTypeEnum, new List<AccountRewardTrack>());

                accountRewardTracks[(RewardTrackType)entry.RewardTrackTypeEnum].Add(new AccountRewardTrack(entry, rewardTrackModel));
            }

            // Add initial Loyalty Reward Track to this account if they do not have any associated.
            if (!accountRewardTracks.ContainsKey(RewardTrackType.Loyalty))
            {
                RewardTrackEntry cosmicRewards = GameTableManager.Instance.RewardTrack.GetEntry(9);
                if (cosmicRewards == null)
                    throw new InvalidOperationException("CosmicRewards starting RewardTrack has not been found in the GameTable.");

                accountRewardTracks.Add((RewardTrackType)cosmicRewards.RewardTrackTypeEnum, new List<AccountRewardTrack>());
                accountRewardTracks[(RewardTrackType)cosmicRewards.RewardTrackTypeEnum].Add(new AccountRewardTrack(session, cosmicRewards));
            }

            CheckLoyaltyPoints();
        }

        /// <summary>
        /// Save all <see cref="AccountRewardTrack"/> data to the database.
        /// </summary>
        public void Save(AuthContext context)
        {
            foreach (AccountRewardTrack rewardTrack in accountRewardTracks.Values.SelectMany(x => x))
                rewardTrack.Save(context);
        }

        /// <summary>
        /// Save all <see cref="CharacterRewardTrack"/> data to the database.
        /// </summary>
        public void Save(CharacterContext context)
        {
            foreach (CharacterRewardTrack rewardTrack in characterRewardTracks.Values)
                rewardTrack.Save(context);
        }

        /// <summary>
        /// Returns <see cref="RewardTrack"/> with the supplied ID
        /// </summary>
        public RewardTrack GetRewardTrack(uint id)
        {
            IEnumerable<AccountRewardTrack> accountRewardTrackList = accountRewardTracks.Values.SelectMany(x => x);
            if (accountRewardTrackList.SingleOrDefault(i => i.RewardTrackId == id) != null)
                return accountRewardTrackList.Single(i => i.RewardTrackId == id);

            if (characterRewardTracks.ContainsKey(id))
                return characterRewardTracks[id];

            RewardTrackEntry entry = GameTableManager.Instance.RewardTrack.GetEntry(id);
            if (entry == null)
                throw new InvalidOperationException($"RewardTrack does not exist with ID {id}");

            CreateRewardTrack(entry);
            return GetRewardTrack(id);
        }

        /// <summary>
        /// Create a new <see cref="RewardTrack"/> in scope based on a <see cref="RewardTrackEntry"/>.
        /// </summary>
        private void CreateRewardTrack(RewardTrackEntry entry, Player player = null)
        {
            if (entry.Flags != 1 && session.Player == null && player == null)
                throw new InvalidOperationException($"Cannot create RewardTrack due to unknown state.");

            if (player == null)
                player = session.Player;

            if (entry.Flags == 1)
            {
                if (accountRewardTracks.ContainsKey((RewardTrackType)entry.RewardTrackTypeEnum))
                    accountRewardTracks[(RewardTrackType)entry.RewardTrackTypeEnum].Add(new AccountRewardTrack(session, entry));
                else
                {
                    accountRewardTracks.Add((RewardTrackType)entry.RewardTrackTypeEnum, new List<AccountRewardTrack>());
                    accountRewardTracks[(RewardTrackType)entry.RewardTrackTypeEnum].Add(new AccountRewardTrack(session, entry));
                }
            }
            else
                characterRewardTracks.Add(entry.Id, new CharacterRewardTrack(player, entry));
        }

        /// <summary>
        /// Used to add Points to a <see cref="RewardTrack"/> with a given ID and Amount.
        /// </summary>
        public void AddPoints(uint rewardTrackId, ulong amount)
        {
            if (GetRewardTrack(rewardTrackId) == null)
            {
                RewardTrackEntry entry = GameTableManager.Instance.RewardTrack.GetEntry(rewardTrackId);
                if (entry == null)
                    throw new InvalidOperationException();

                CreateRewardTrack(entry);
            }

            RewardTrack rewardTrack = GetRewardTrack(rewardTrackId);
            if (rewardTrack.IsAccountTrack())
                throw new InvalidOperationException($"RewardTrack with {rewardTrackId} is not meant to be modified by the public AddPoints() method.");

            rewardTrack.AddPoints(session, amount);
        }

        /// <summary>
        /// Handles adding Loyalty Points to this Account. Should only be called by <see cref="AccountCurrencyManager"/>.
        /// </summary>
        public void HandleAddLoyaltyPoints(ulong amount)
        {
            if (!accountRewardTracks.ContainsKey(RewardTrackType.Loyalty))
                throw new InvalidOperationException($"Loyalty Track should exist in the cache!");

            AccountRewardTrack currentLoyaltyTrack = accountRewardTracks[RewardTrackType.Loyalty].Last();
            if (amount >= currentLoyaltyTrack.MaximumPoints - currentLoyaltyTrack.Points)
            {
                ulong consumedAmount = currentLoyaltyTrack.MaximumPoints - currentLoyaltyTrack.Points;
                currentLoyaltyTrack.AddPoints(session, consumedAmount);
                amount -= consumedAmount;

                // Unlock next Tier if applicable, and add remaining points.
                RewardTrackEntry nextLoyaltyTrack = GameTableManager.Instance.RewardTrack.Entries.FirstOrDefault(i => i.RewardTrackIdParent == currentLoyaltyTrack.RewardTrackId);
                if (nextLoyaltyTrack == null)
                    return;

                CreateRewardTrack(nextLoyaltyTrack);
                HandleAddLoyaltyPoints(amount);
            }
            else
                currentLoyaltyTrack.AddPoints(session, amount);
        }

        /// <summary>
        /// Checks Loyalty Points to confirm that they are not out of sync with the Cosmic Rewards Account Currency. Only used when logging in.
        /// </summary>
        private void CheckLoyaltyPoints()
        {
            ulong totalLoyaltyPoints = 0;
            foreach (AccountRewardTrack rewardTrack in accountRewardTracks[RewardTrackType.Loyalty])
                totalLoyaltyPoints += rewardTrack.Points;
            
            ulong cosmicRewardPoints = session.AccountCurrencyManager.GetAmount(Account.Static.AccountCurrencyType.CosmicReward);
            if (cosmicRewardPoints > totalLoyaltyPoints)
                HandleAddLoyaltyPoints(cosmicRewardPoints - totalLoyaltyPoints);
        }

        /// <summary>
        /// Used to handle choosing a reward based on a provided <see cref="RewardTrackRewardsEntry"/>. Should only be called by Client handler.
        /// </summary>
        public void HandleChooseReward(RewardTrackRewardsEntry entry, uint choice)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            RewardTrack rewardTrack = GetRewardTrack(entry.RewardTrackId);
            if (rewardTrack == null)
                throw new InvalidOperationException($"RewardTrack {entry.RewardTrackId} is null!");

            rewardTrack.ChooseReward(session, entry.Id, (int)choice);
        }

        /// <summary>
        /// Used when a new <see cref="Player"/> is created for this account. Clears out existing <see cref="CharacterRewardTrack"/> cache and loads new for this Character.
        /// </summary>
        public void OnNewCharacter(Player player, CharacterModel model)
        {
            characterRewardTracks.Clear();

            foreach (CharacterRewardTrackModel rewardTrackModel in model.RewardTrack)
            {
                RewardTrackEntry entry = GameTableManager.Instance.RewardTrack.GetEntry(rewardTrackModel.RewardTrackId);
                if (entry == null)
                    throw new DatabaseDataException($"Character {model.Id} has invalid RewardTrack {rewardTrackModel.RewardTrackId} stored!");

                characterRewardTracks.Add(entry.Id, new CharacterRewardTrack(entry, rewardTrackModel));
            }

            // Create RewardTracks for all remaining entries.
            foreach (RewardTrackEntry entry in GameTableManager.Instance.RewardTrack.Entries.Where(i => i.Flags != 1 && !characterRewardTracks.Keys.Contains(i.Id) ))
                CreateRewardTrack(entry, player);
        }

        /// <summary>
        /// Sends initial packets to the <see cref="Player"/>
        /// </summary>
        public void SendInitialPackets()
        {
            SendRewardTracksLoadedMessage();
        }

        /// <summary>
        /// Sends the <see cref="ServerRewardTracksLoaded"/> to the <see cref="WorldSession"/> associated with this Manager.
        /// </summary>
        private void SendRewardTracksLoadedMessage()
        {
            var loaded = new ServerRewardTracksLoaded();

            foreach (AccountRewardTrack accountRewardTrack in accountRewardTracks.Values.SelectMany(x => x))
                loaded.RewardTracks.Add(accountRewardTrack.BuildNetworkMessage());
            
            foreach (CharacterRewardTrack characterRewardTrack in characterRewardTracks.Values)
                loaded.RewardTracks.Add(characterRewardTrack.BuildNetworkMessage());

            session.EnqueueMessageEncrypted(loaded);
        }
    }
}
