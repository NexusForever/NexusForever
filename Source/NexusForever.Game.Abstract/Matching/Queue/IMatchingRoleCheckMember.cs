using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Game.Matching.Queue
{
    public interface IMatchingRoleCheckMember
    {
        ulong CharacterId { get; }
        Role? Roles { get; }

        /// <summary>
        /// Initialise <see cref="IMatchingRoleCheckMember"/> with supplied character id.
        /// </summary>
        void Initialise(ulong characterId);

        /// <summary>
        /// Set <see cref="Role"/> for member.
        /// </summary>
        void SetRoles(Role roles);

        /// <summary>
        /// Send <see cref="IWritable"/> message to member.
        /// </summary>
        void Send(IWritable message);
    }
}
