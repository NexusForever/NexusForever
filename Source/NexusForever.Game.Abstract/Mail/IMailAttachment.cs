using NexusForever.Database.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Mail
{
    public interface IMailAttachment : IDatabaseCharacter, INetworkBuildable<ServerMailAvailable.Attachment>
    {
        ulong Id { get; }
        uint Index { get; }
        IItem Item { get; }

        /// <summary>
        /// Enqueue <see cref="IMailAttachment"/> to be deleted from the database.
        /// </summary>
        void EnqueueDelete();
    }
}