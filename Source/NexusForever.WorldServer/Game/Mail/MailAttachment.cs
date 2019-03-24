using Microsoft.EntityFrameworkCore;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Mail.Static;
using MailAttachmentModel = NexusForever.WorldServer.Database.Character.Model.CharacterMailAttachment;

namespace NexusForever.WorldServer.Game.Mail
{
    public class MailAttachment: ISaveCharacter
    {
        public ulong Id { get; }
        public uint ItemId { get; }
        public uint Index { get; }
        public uint Amount { get; }

        private Item2Entry ItemEntry { get; set; }

        private MailAttachmentSaveMask saveMask;

        public MailAttachment(ulong mailId, uint itemId, uint index, uint amount)
        {
            Id = mailId;
            ItemId = itemId;
            Index = index;
            Amount = amount;

            saveMask = MailAttachmentSaveMask.Create;
        }

        /// <summary>
        /// Enqueue <see cref="MailAttachment"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete()
        {
            saveMask = MailAttachmentSaveMask.Delete;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == MailAttachmentSaveMask.None)
                return;

            if ((saveMask & MailAttachmentSaveMask.Create) != 0)
            {
                context.Add(new MailAttachmentModel
                {
                    Id = Id,
                    Index = Index,
                    ItemId = ItemId,
                    Amount = Amount
                });
            }
            else if ((saveMask & MailAttachmentSaveMask.Delete) != 0)
            {
                var model = new MailAttachmentModel
                {
                    Id = Id,
                    Index = Index
                };

                context.Entry(model).State = EntityState.Deleted;
            }

            saveMask = MailAttachmentSaveMask.None;
        }
    }
}
