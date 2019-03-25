using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Mail.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NexusForever.WorldServer.Database.Character;
using System.Threading.Tasks;
using NexusForever.WorldServer.Game.Map;
using NexusForever.Shared.Network;

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

        /// <summary>
        /// Called every tick
        /// </summary>
        /// <param name="lastTick"></param>
        public static void Update(double lastTick)
        {
            timeToSave -= lastTick;
            if (timeToSave <= 0d)
            {
                var tasks = new List<Task>();
                foreach (MailItem mailItem in queuedMail.Values.ToList())
                {
                    tasks.Add(CharacterDatabase.SaveMail(mailItem));
                    if (mailItem.IsReadyToDeliver())
                    {
                        // Deliver mail if user is online
                        WorldSession targetSession = NetworkManager<WorldSession>.GetSession(c => c.Player?.CharacterId == mailItem.RecipientId);
                        if(targetSession != null)
                        {
                            targetSession.Player.AvailableMail.TryAdd(mailItem.Id, mailItem);
                            SendAvailableMail(targetSession, targetSession.Player.AvailableMail.Values.ToList());
                        }
                        queuedMail.Remove(mailItem.Id);
                    }
                }

                Task.WaitAll(tasks.ToArray());

                timeToSave = SaveDuration;
            }
        }

        /// <summary>
        /// Called by <see cref="Player"/> to save <see cref="MailItem"/> to database
        /// </summary>
        /// <param name="player"></param>
        /// <param name="context"></param>
        public static void Save(Player player, CharacterContext context)
        {
            var tasks = new List<Task>();
            foreach (MailItem mail in player.AvailableMail.Values.ToList())
            {
                if (mail.IsPendingDelete)
                    player.AvailableMail.Remove(mail.Id);

                tasks.Add(CharacterDatabase.SaveMail(mail));
            }
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Checks to see if the targeted <see cref="Mailbox"/> is in range
        /// </summary>
        /// <param name="session"></param>
        /// <param name="unitId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Method to process send <see cref="Player"/> to <see cref="Player"/> mails
        /// </summary>
        /// <param name="session"></param>
        /// <param name="clientMailSend"></param>
        /// <param name="targetCharacter"></param>
        /// <param name="newMail"></param>
        public static void SendMailToPlayer(WorldSession session, ClientMailSend clientMailSend, Character targetCharacter, out MailItem newMail)
        {
            newMail = null;
            bool isCod = clientMailSend.CreditsRequested > 0;

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

                        session.Player.Inventory.ItemDelete(item, 20);
                    }
                }
                foreach (MailAttachment mailAttachment in mailAttachments)
                    newMail.AttachmentAdd(mailAttachment);
            }

            // TODO: Calculate & Deduct Mail Cost

            // TODO: Handle queued mailn
            queuedMail.TryAdd(newMail.Id, newMail);
        }
        
        // TODO: Handle sending mail from creatures using mail templates

        // TODO: Handle sending mail from auctions to users upon auction end

        // TODO: Handle sending mail from GMs to replace missing items, 

        public static void PayCOD(WorldSession session, MailItem mailItem)
        {
            mailItem.PayOrTakeCash();

            MailItem newMail = new MailItem(mailItem.SenderId, session.Player, $"Cash from: {mailItem.Subject}", "", mailItem.CurrencyAmount, false, DeliveryTime.Instant);

            queuedMail.TryAdd(newMail.Id, newMail);
        }

        /// <summary>
        /// Handles returning a <see cref="MailItem"/> to sender
        /// </summary>
        /// <param name="session"></param>
        /// <param name="mailItem"></param>
        public static void ReturnMail(WorldSession session, MailItem mailItem)
        {
            mailItem.ReturnMail();

            session.Player.AvailableMail.Remove(mailItem.Id);
            queuedMail.TryAdd(mailItem.Id, mailItem);
        }

        /// <summary>
        /// Called by <see cref="Player"/> to send available mail on entering map
        /// </summary>
        /// <param name="session"></param>
        public static void SendInitialPackets(WorldSession session)
        {
            SendAvailableMail(session, session.Player.AvailableMail.Values.ToList());
        }

        /// <summary>
        /// Execute <see cref="ServerMailAvailable"/> with appropriate <see cref="MailItem"/>
        /// </summary>
        /// <param name="session"></param>
        /// <param name="mails"></param>
        public static void SendAvailableMail(WorldSession session, List<MailItem> mails)
        {
            ServerMailAvailable serverMailAvailable = new ServerMailAvailable();
            serverMailAvailable.MailList = new List<ServerMailAvailable.Mail>();

            foreach (MailItem mail in mails)
                if (mail.IsReadyToDeliver())
                {
                    serverMailAvailable.MailList.Add(ConvertMailItemToServerMail(mail));

                    if (queuedMail.ContainsKey(mail.Id))
                        queuedMail.Remove(mail.Id);
                }
                else if(!queuedMail.ContainsKey(mail.Id))
                {
                    queuedMail.TryAdd(mail.Id, mail);
                    session.Player.AvailableMail.Remove(mail.Id);
                }

            // Add queued items which have not been saved to the DB, yet
            foreach(MailItem mailItem in queuedMail.Values.ToList())
                if (mailItem.IsReadyToDeliver())
                {
                    serverMailAvailable.MailList.Add(ConvertMailItemToServerMail(mailItem));

                    queuedMail.Remove(mailItem.Id);
                    session.Player.AvailableMail.TryAdd(mailItem.Id, mailItem);
                }

            session.EnqueueMessageEncrypted(serverMailAvailable);
        }

        /// <summary>
        /// Handles converting <see cref="MailItem"/> to <see cref="ServerMailAvailable.Mail"/> for the client to process
        /// </summary>
        /// <param name="mailItem"></param>
        /// <returns></returns>
        public static ServerMailAvailable.Mail ConvertMailItemToServerMail(MailItem mailItem)
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
            foreach (MailAttachment mailAttachment in mailItem.GetAttachments().ToList())
                mailAttachments.Add(ConvertMailAttachmentToServerMailAttachment(mailAttachment));

            serverMailItem.Attachments = mailAttachments;

            return serverMailItem;
        }

        /// <summary>
        /// Handles converting <see cref="MailAttachment"/> to <see cref="ServerMailAvailable.Attachment"/> for the client to process
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
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
