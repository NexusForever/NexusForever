using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Mail;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Mail
{
    public interface IMailItem : IDatabaseCharacter, IDatabaseState, INetworkBuildable<ServerMailAvailable.Mail>, IEnumerable<IMailAttachment>
    {
        ulong Id { get; }
        ulong RecipientId { get; }
        SenderType SenderType { get; }
        ulong SenderId { get; }
        uint CreatureId { get; }

        string Subject { get; }
        string Message { get; }
        uint TextEntrySubject { get; }
        uint TextEntryMessage { get; }

        CurrencyType CurrencyType { get; }
        ulong CurrencyAmount { get; }
        bool IsCashOnDelivery { get; }
        bool HasPaidOrCollectedCurrency { get; }

        MailFlag Flags { get; }
        DeliveryTime DeliveryTime { get; }
        DateTime CreateTime { get; }
        float ExpiryTime { get; }

        /// <summary>
        /// Returns the specific <see cref="IMailAttachment"/> based on its index.
        /// </summary>
        IMailAttachment GetAttachment(uint index);

        /// <summary>
        /// Mark this <see cref="IMailItem"/> as read by the player.
        /// </summary>
        void MarkAsRead();

        /// <summary>
        /// Mark this <see cref="IMailItem"/> as paid, or taken the currency attached.
        /// </summary>
        void PayOrTakeCash();

        /// <summary>
        /// Mark this <see cref="IMailItem"/> as not returnable.
        /// </summary>
        void MarkAsNotReturnable();

        /// <summary>
        /// Return this <see cref="IMailItem"/> to sender.
        /// </summary>
        void ReturnMail();

        /// <summary>
        /// Returns whether this item is ready to be delivered based on <see cref="DeliveryTime"/>.
        /// </summary>
        bool IsReadyToDeliver();

        /// <summary>
        /// Add a <see cref="IMailAttachment"/> to this <see cref="IMailItem"/>.
        /// </summary>
        void AttachmentAdd(IMailAttachment mailAttachment);

        /// <summary>
        /// Remove a <see cref="IMailAttachment"/> from this <see cref="IMailItem"/>.
        /// </summary>
        void AttachmentDelete(IMailAttachment mailAttachment, uint index);
    }
}