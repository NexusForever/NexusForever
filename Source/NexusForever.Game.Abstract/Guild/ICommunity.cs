using NexusForever.Game.Abstract.Housing;

namespace NexusForever.Game.Abstract.Guild
{
    public interface ICommunity : IGuildChat
    {
        IResidence Residence { get; set; }

        /// <summary>
        /// Set <see cref="ICommunity"/> privacy level.
        /// </summary>
        void SetCommunityPrivate(bool enabled);
    }
}