using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;

namespace NexusForever.WorldServer.Game.Mail
{
    public class MailAttachment: ISaveCharacter
    {
        public uint ItemId { get; }
        public uint Index { get; }
        public uint Amount { get; }

        private Item2Entry ItemEntry { get; set; }

        public MailAttachment(uint itemId, uint index, uint amount)
        {
            ItemId = itemId;
            Index = index;
            Amount = amount;
        }

        public void Save(CharacterContext context)
        {

        }
    }
}
