using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Pet
{
    public class ClientPetRenameHandler : IMessageHandler<IWorldSession, ClientPetRename>
    {
        public void HandleMessage(IWorldSession session, ClientPetRename petRename)
        {
            session.Player.PetCustomisationManager.RenamePet(petRename.PetType,
                petRename.PetObjectId,
                petRename.Name);
        }
    }
}
