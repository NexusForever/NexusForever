using NexusForever.Shared;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Mail.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NLog;
using NexusForever.WorldServer.Database.Character;
using System.Threading.Tasks;
using NexusForever.WorldServer.Game.Map;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.GameTable;

namespace NexusForever.WorldServer.Game.Mail
{
    public static class MailManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private const double SaveDuration = 60d;

        /// <summary>
        /// Id to be assigned to the next created mail.
        /// </summary>
        public static ulong NextMailId => nextMailId++;
        private static ulong nextMailId;

        private static double timeToSave = SaveDuration;

        private static Dictionary<ulong, MailItem> queuedMail = new Dictionary<ulong, MailItem>();

        public static void Initialise()
        {
            nextMailId = CharacterDatabase.GetNextMailId() + 1ul;
        }

        public static void Update(double lastTick)
        {
            timeToSave -= lastTick;
            if (timeToSave <= 0d)
            {
                var tasks = new List<Task>();
                foreach (MailItem mailItem in queuedMail.Values)
                    tasks.Add(CharacterDatabase.SaveMail(mailItem));

                Task.WaitAll(tasks.ToArray());

                timeToSave = SaveDuration;
            }
        }

        public static void Save(Player player, CharacterContext context)
        {
            var tasks = new List<Task>();
            foreach (MailItem mail in player.AvailableMail.Values)
            {
                if (mail.IsPendingDelete)
                    player.AvailableMail.Remove(mail.Id);

                tasks.Add(CharacterDatabase.SaveMail(mail));
            }
            Task.WaitAll(tasks.ToArray());
        }

        public static bool IsTargetMailBoxInRange(WorldSession session, uint unitId)
        {
            float searchDistance = 20f;

            session.Player.Map.Search(
                session.Player.Position,
                searchDistance,
                new SearchCheckRangeMailboxOnly(session.Player.Position, searchDistance, session.Player),
                out List<GridEntity> intersectedEntities
            );

            if (intersectedEntities.FirstOrDefault(c => c.Guid == unitId) != null)
                return true;

            return false;
        }

        public static void SendMailToPlayer(WorldSession session, ClientMailSend clientMailSend, Character targetCharacter, out MailItem newMail)
        {
            newMail = null;
            bool isCod = clientMailSend.CreditsRequested > 0;

            // TODO: Deduct Credit Cost (if creditsSend > 0)
            if (!isCod && clientMailSend.CreditsSent > 0)
                session.Player.CurrencyManager.CurrencySubtractAmount(1, clientMailSend.CreditsSent);

            newMail = new MailItem(targetCharacter.Id, session.Player, clientMailSend.Subject, clientMailSend.Message, isCod ? clientMailSend.CreditsRequested : clientMailSend.CreditsSent, isCod, DeliveryTime.Instant);

            if(newMail != null)
            {
                List<MailAttachment> mailAttachments = new List<MailAttachment>();
                for (int i = 0; i < clientMailSend.Items.Count; i++)
                {
                    ulong itemGuid = clientMailSend.Items[i];
                    if (itemGuid > 0)
                    {
                        Entity.Item item = session.Player.Inventory.GetItem(itemGuid);

                        // TODO: Check the Item can be traded.
                        MailAttachment mailAttachment = new MailAttachment(newMail.Id, item.Entry.Id, (uint)i, item.StackCount);
                        if (mailAttachment != null)
                            mailAttachments.Add(mailAttachment);

                        session.Player.Inventory.ItemDelete(item);
                    }
                }
                foreach (MailAttachment mailAttachment in mailAttachments)
                    newMail.AttachmentAdd(mailAttachment);
            }

            // TODO: Calculate & Deduct Mail Cost

            // TODO: Handle queued mail
            queuedMail.Add(newMail.Id, newMail);
        }

        public static void SendInitialPackets(WorldSession session)
        {
            SendAvailableMail(session, session.Player.AvailableMail.Values.ToList());
        }

        public static void SendAvailableMail(WorldSession session, List<MailItem> mails)
        {
            ServerMailAvailable message = new ServerMailAvailable();

            message.MailList = new List<ServerMailAvailable.Mail>();
            mails.ForEach(c => message.MailList.Add(ConverMailItemToServerMail(c)));

            // Include mail that should have been delivered to them but wasn't in their model
            queuedMail.Values.Where(c => c.RecipientId == session.Player.CharacterId).ToList().ForEach(c => message.MailList.Add(ConverMailItemToServerMail(c)));

            session.EnqueueMessageEncrypted(message);
        }

        public static ServerMailAvailable.Mail ConverMailItemToServerMail(MailItem mailItem)
        {
            var serverMailItem = new ServerMailAvailable.Mail
            {
                MailId = mailItem.Id,
                SenderType = mailItem.SenderType,
                SenderRealm = mailItem.SenderType == SenderType.Player || mailItem.SenderType == SenderType.GM ? WorldServer.RealmId : (ushort)0,
                SenderCharacterId = mailItem.SenderType == SenderType.Player || mailItem.SenderType == SenderType.GM ? mailItem.SenderId : 0,
                Subject = mailItem.Subject,
                Message = mailItem.Message,
                TextEntrySubject = mailItem.TextEntrySubject,
                TextEntryMessage = mailItem.TextEntryMessage,
                CreatureId = mailItem.SenderType == SenderType.Creature || mailItem.SenderType == SenderType.ItemAuction || mailItem.SenderType == SenderType.CommodityAuction ? (uint)mailItem.SenderId : 0,
                CurrencyGiftType = 0,
                CurrencyGiftAmount = mailItem.IsCashOnDelivery || mailItem.HasPaidOrCollectedCurrency ? 0 : mailItem.CurrencyAmount,
                CostOnDeliveryAmount = mailItem.IsCashOnDelivery && !mailItem.HasPaidOrCollectedCurrency ? mailItem.CurrencyAmount : 0,
                Flags = mailItem.Flags,
                ExpiryTimeInDays = mailItem.ExpiryTime
            };

            List<ServerMailAvailable.Attachment> mailAttachments = new List<ServerMailAvailable.Attachment>();
            mailItem.GetAttachments().ToList().ForEach(c => mailAttachments.Add(ConvertMailAttachmentToServerMailAttachment(c)));
            serverMailItem.Attachments = mailAttachments;

            return serverMailItem;
        }

        public static ServerMailAvailable.Attachment ConvertMailAttachmentToServerMailAttachment(MailAttachment attachment)
        {
            var serverMailAttachment = new ServerMailAvailable.Attachment
            {
                ItemId = attachment.ItemId,
                Amount = attachment.Amount
            };

            return serverMailAttachment;
        }
    }
}
