using System;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Housing
{
    public class ResidenceManager
    {
        public Residence Residence { get; private set; }

        private readonly Player owner;

        /// <summary>
        /// Create a new <see cref="ResidenceManager"/>.
        /// </summary>
        public ResidenceManager(Player player)
        {
            owner     = player;
            Residence = GlobalResidenceManager.Instance.GetResidenceByOwner(owner.CharacterId);
        }

        /// <summary>
        /// Create new <see cref="Decor"/> from supplied <see cref="HousingDecorInfoEntry"/> to residence your crate.
        /// </summary>
        /// <remarks>
        /// Decor will be created for your residence regardless of the current residence you are on.
        /// </remarks>
        public void DecorCreate(HousingDecorInfoEntry entry, uint quantity = 1u)
        {
            Residence ??= GlobalResidenceManager.Instance.CreateResidence(owner);

            if (Residence.Map != null)
                Residence.Map.DecorCreate(Residence, entry, quantity);
            else
            {
                for (uint i = 0u; i < quantity; i++)
                    Residence.DecorCreate(entry);
            }
        }

        /// <summary>
        /// Set the privacy level of your residence with the supplied <see cref="ResidencePrivacyLevel"/>.
        /// </summary>
        public void SetResidencePrivacy(ResidencePrivacyLevel privacy)
        {
            if (Residence == null)
                throw new InvalidOperationException();

            Residence.PrivacyLevel = privacy;
            SendHousingBasics();
        }

        /// <summary>
        /// Send <see cref="ServerHousingBasics"/> message to <see cref="Player"/>.
        /// </summary>
        public void SendHousingBasics()
        {
            // not really sure why the client converts the privacy level to and from flags
            // see Residence.GetResidencePrivacyLevel LUA function for more context
            var flags = (Residence?.PrivacyLevel ?? ResidencePrivacyLevel.Public) switch
            {
                ResidencePrivacyLevel.Public        => ServerHousingBasics.ResidencePrivacyLevelFlags.Public,
                ResidencePrivacyLevel.Private       => ServerHousingBasics.ResidencePrivacyLevelFlags.Private,
                ResidencePrivacyLevel.NeighborsOnly => ServerHousingBasics.ResidencePrivacyLevelFlags.NeighborsOnly,
                ResidencePrivacyLevel.RoommatesOnly => ServerHousingBasics.ResidencePrivacyLevelFlags.RoommatesOnly,
                _                                   => throw new NotImplementedException()
            };

            owner.Session.EnqueueMessageEncrypted(new ServerHousingBasics
            {
                ResidenceId     = Residence?.Id ?? 0ul,
                /*NeighbourhoodId = GuildManager.GetGuild<Community>(GuildType.Community)?.Id ?? 0ul,*/
                PrivacyLevel    = flags
            });
        }
    }
}
