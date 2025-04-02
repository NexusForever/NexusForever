using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Pet
{
    public class ClientPlayerPathLib_SetScannerNameHandler : IMessageHandler<IWorldSession, ClientPlayerPathLib_SetScannerName>
    {
        public void HandleMessage(IWorldSession session, ClientPlayerPathLib_SetScannerName petRename)
        {
            session.Player.PetCustomisationManager.RenamePet(petRename.PetType,
                petRename.PathScientistScanBotProfileId,
                petRename.Name);
        }
    }
}
