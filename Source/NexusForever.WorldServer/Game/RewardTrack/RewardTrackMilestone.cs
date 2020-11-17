using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.RewardTrack.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;

namespace NexusForever.WorldServer.Game.RewardTrack
{
    public class RewardTrackMilestone
    {
        public uint RewardTrackId { get; protected set; }
        public uint MilestoneId { get; protected set; }
        public uint PointsRequired { get; protected set; }
        public int Choice { get; protected set; } = -1;

        public RewardTrackRewardsEntry Entry { get; protected set; }

        protected RewardTrackSaveMask SaveMask { get; set; }
        public bool PendingCreate => (SaveMask & RewardTrackSaveMask.Create) != 0;
        public bool PendingDelete => (SaveMask & RewardTrackSaveMask.Delete) != 0;

        public RewardTrackMilestone()
        {
        }

        /// <summary>
        /// Sets the Choice for this <see cref="RewardTrackMilestone"/>.
        /// </summary>
        protected void SetChoice(int choice)
        {
            if (Choice != -1 && choice != -1)
                throw new InvalidOperationException($"Choice is already set for this RewardTrackMilestone. Cannot set again!");

            Choice = choice;
            SaveMask |= RewardTrackSaveMask.Modify;
        }

        /// <summary>
        /// Sets the Choice and grants the rewards for this <see cref="RewardTrackMilestone"/>. Should only be called from the Client's handler.
        /// </summary>
        public bool ChooseReward(WorldSession session, int choice)
        {
            SetChoice(choice);
            bool success =  GrantReward(session);
            if (!success)
                SetChoice(-1);

            return success;
        }

        /// <summary>
        /// Enqueue this <see cref="RewardTrackMilestone"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete()
        {
            if (!PendingCreate)
                SaveMask = RewardTrackSaveMask.Delete;
            else
                SaveMask = RewardTrackSaveMask.None;
        }

        /// <summary>
        /// Attempts to Grant the Reward for this <see cref="RewardTrackMilestone"/> after a Choice has been set. Returns a boolean response.
        /// </summary>
        public bool GrantReward(WorldSession session)
        {
            if (Choice < 0)
                throw new ArgumentOutOfRangeException(nameof(Choice));

            bool success = GrantItemReward(session, (RewardTrackRewardType)Entry.RewardTrackRewardTypeEnums[Choice], Entry.RewardChoiceIds[Choice], Entry.RewardChoiceCounts[Choice]);

            if (success)
                GrantCurrencyReward(session);

            return success;
        }

        /// <summary>
        /// Grants an Item Reward based on a <see cref="RewardTrackRewardType"/>, Item ID, and Count.
        /// </summary>
        protected bool GrantItemReward(WorldSession session, RewardTrackRewardType type, uint itemId, uint count)
        {
            if (itemId == 0)
                throw new ArgumentOutOfRangeException(nameof(itemId));

            switch (type)
            {
                case RewardTrackRewardType.AccountItem:
                    // Handle.
                    return false;
                case RewardTrackRewardType.Item:
                    if (session.Player == null)
                        throw new NotImplementedException();

                    // TODO: Change to check if Player has room for X Item
                    if (session.Player.Inventory.IsInventoryFull())
                    {
                        session.EnqueueMessageEncrypted(new ServerGenericError
                        {
                            Error = Game.Static.GenericError.ItemInventoryFull
                        });
                        return false;
                    }

                    session.Player.Inventory.ItemCreate(itemId, count, Entity.Static.ItemUpdateReason.RewardTrack);
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Grants Currency Reward for this <see cref="RewardTrackMilestone"/>.
        /// </summary>
        protected void GrantCurrencyReward(WorldSession session)
        {
            if (Entry.CurrencyTypeId == 0 || Entry.CurrencyAmount == 0)
                return;

            if (session.Player == null)
                throw new ArgumentNullException(nameof(session.Player));

            session.Player.CurrencyManager.CurrencyAddAmount((Entity.Static.CurrencyType)Entry.CurrencyTypeId, Entry.CurrencyAmount);
        }
    }
}
