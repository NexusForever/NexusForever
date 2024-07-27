using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Housing;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingCommunityDonateHandler : IMessageHandler<IWorldSession, ClientHousingCommunityDonate>
    {
        public void HandleMessage(IWorldSession session, ClientHousingCommunityDonate housingCommunityDonate)
        {
            // can only donate to a community from a residence map
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            IResidence residence = session.Player.ResidenceManager.Residence;
            if (residence == null)
                throw new InvalidPacketValueException();

            ICommunity community = session.Player.GuildManager.GetGuild<ICommunity>(GuildType.Community);
            if (community?.Residence == null)
                throw new InvalidPacketValueException();

            foreach (DecorInfo decorInfo in housingCommunityDonate.Decor)
            {
                IDecor decor = residence.GetDecor(decorInfo.DecorId);
                if (decor == null)
                    throw new InvalidPacketValueException();

                if (decor.Type != DecorType.Crate)
                    throw new InvalidPacketValueException();

                // copy decor to recipient residence
                if (community.Residence.Map != null)
                    community.Residence.Map.DecorCopy(community.Residence, decor);
                else
                    community.Residence.DecorCopy(decor);

                // remove decor from donor residence
                if (residence.Map != null)
                    residence.Map.DecorDelete(residence, decor);
                else
                {
                    if (decor.PendingCreate)
                        residence.DecorRemove(decor);
                    else
                        decor.EnqueueDelete(true);
                }
            }
        }
    }
}
