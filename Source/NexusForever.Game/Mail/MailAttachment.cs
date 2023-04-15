using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Mail;
using NexusForever.Game.Entity;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Mail
{
    public class MailAttachment : IMailAttachment
    {
        /// <summary>
        /// Determines which fields need saving for <see cref="IMailAttachment"/> when being saved to the database.
        /// </summary>
        [Flags]
        public enum MailAttachmentSaveMask
        {
            None   = 0x0000,
            Create = 0x0001,
            Delete = 0x0002
        }

        public ulong Id { get; }
        public uint Index { get; }
        public IItem Item { get; }

        private MailAttachmentSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="IMailAttachment"/> from an existing <see cref="CharacterMailAttachmentModel"/> model.
        /// </summary>
        /// <param name="model"></param>
        public MailAttachment(CharacterMailAttachmentModel model)
        {
            Id       = model.Id;
            Index    = model.Index;
            Item     = new Item(model.Item);

            saveMask = MailAttachmentSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="IMailAttachment"/>.
        /// </summary>
        public MailAttachment(ulong mailId, uint index, IItem item)
        {
            Id       = mailId;
            Index    = index;
            Item     = item;

            saveMask = MailAttachmentSaveMask.Create;
        }

        /// <summary>
        /// Enqueue <see cref="IMailAttachment"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete()
        {
            saveMask = MailAttachmentSaveMask.Delete;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask != MailAttachmentSaveMask.None)
            {
                if ((saveMask & MailAttachmentSaveMask.Create) != 0)
                {
                    context.Add(new CharacterMailAttachmentModel
                    {
                        Id       = Id,
                        Index    = Index,
                        ItemGuid = Item.Guid
                    });
                }
                else if ((saveMask & MailAttachmentSaveMask.Delete) != 0)
                {
                    var model = new CharacterMailAttachmentModel
                    {
                        Id    = Id,
                        Index = Index
                    };

                    context.Entry(model).State = EntityState.Deleted;
                }

                saveMask = MailAttachmentSaveMask.None;
            }

            Item.Save(context);
        }

        public ServerMailAvailable.Attachment Build()
        {
            return new ServerMailAvailable.Attachment
            {
                ItemId = Item.Id,
                Amount = Item.StackCount
            };
        }
    }
}
