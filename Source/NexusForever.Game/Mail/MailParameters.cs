using NexusForever.Game.Abstract.Mail;
using NexusForever.Game.Static.Mail;

namespace NexusForever.Game.Mail
{
    public class MailParameters : IMailParameters
    {
        public ulong RecipientCharacterId { get; set; }
        public ulong SenderCharacterId { get; set; }
        public SenderType MessageType { get; set; }
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";
        public uint SubjectStringId { get; set; }
        public uint BodyStringId { get; set; }
        public uint CreatureId { get; set; }
        public ulong MoneyToGive { get; set; }
        public ulong CodAmount { get; set; }
        public DeliveryTime DeliveryTime { get; set; }
    }
}
