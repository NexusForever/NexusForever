using NexusForever.Game.Static.Mail;

namespace NexusForever.Game.Abstract.Mail
{
    public interface IMailParameters
    {
        ulong RecipientCharacterId { get; set; }
        ulong SenderCharacterId { get; set; }
        SenderType MessageType { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        uint SubjectStringId { get; set; }
        uint BodyStringId { get; set; }
        uint CreatureId { get; set; }
        ulong MoneyToGive { get; set; }
        ulong CodAmount { get; set; }
        DeliveryTime DeliveryTime { get; set; }
    }
}