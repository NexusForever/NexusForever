using Microsoft.EntityFrameworkCore;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Mail.Static;
using MailAttachmentModel = NexusForever.WorldServer.Database.Character.Model.CharacterMailAttachment;
using ItemModel = NexusForever.WorldServer.Database.Character.Model.Item;

namespace NexusForever.WorldServer.Game.Mail
{
    public class MailAttachment: ISaveCharacter
    {
        public ulong Id { get; }
        public ulong ItemGuid { get; }
        public uint Index { get; }

        public Entity.Item Item { get; private set; }

        private MailAttachmentSaveMask saveMask;

        public bool IsPendingDelete => ((saveMask & MailAttachmentSaveMask.Delete) != 0);

        /// <summary>
        /// Create a new <see cref="MailAttachment"/> from an existing <see cref="MailAttachmentModel"/>
        /// </summary>
        /// <param name="model"></param>
        public MailAttachment(MailAttachmentModel model)
        {
            Id = model.Id;
            ItemGuid = model.ItemGuid;
            Index = model.Index;

            Item = new Entity.Item(CharacterDatabase.GetItemById(model.ItemGuid));

            saveMask = MailAttachmentSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="MailAttachment"/>
        /// </summary>
        /// <param name="mailId"></param>
        /// <param name="itemGuid"></param>
        /// <param name="index"></param>
        /// <param name="amount"></param>
        public MailAttachment(ulong mailId, ulong itemGuid, uint index, Entity.Item item = null)
        {
            Id = mailId;
            ItemGuid = itemGuid;
            Index = index;

            if (item != null)
                Item = item;

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
                    context.Add(new MailAttachmentModel
                    {
                        Id = Id,
                        Index = Index,
                        ItemGuid = ItemGuid
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

            Item.Save(context);
        }
    }
}
