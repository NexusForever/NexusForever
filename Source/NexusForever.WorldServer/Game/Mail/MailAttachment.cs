using Microsoft.EntityFrameworkCore;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Mail.Static;
using ItemEntity = NexusForever.WorldServer.Game.Entity.Item;

namespace NexusForever.WorldServer.Game.Mail
{
    public class MailAttachment: ISaveCharacter
    {
        public ulong Id { get; }
        public uint Index { get; }
        public ItemEntity Item { get; }

        private MailAttachmentSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="MailAttachment"/> from an existing <see cref="CharacterMailAttachment"/> model.
        /// </summary>
        /// <param name="model"></param>
        public MailAttachment(CharacterMailAttachment model)
        {
            Id       = model.Id;
            Index    = model.Index;
            Item     = new ItemEntity(model.ItemGu);

            saveMask = MailAttachmentSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="MailAttachment"/>.
        /// </summary>
        public MailAttachment(ulong mailId, uint index, ItemEntity item)
        {
            Id       = mailId;
            Index    = index;
            Item     = item;

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
            if (saveMask != MailAttachmentSaveMask.None)
            {
                if ((saveMask & MailAttachmentSaveMask.Create) != 0)
                {
                    context.Add(new CharacterMailAttachment
                    {
                        Id       = Id,
                        Index    = Index,
                        ItemGuid = Item.Guid
                    });
                }
                else if ((saveMask & MailAttachmentSaveMask.Delete) != 0)
                {
                    var model = new CharacterMailAttachment
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
    }
}
