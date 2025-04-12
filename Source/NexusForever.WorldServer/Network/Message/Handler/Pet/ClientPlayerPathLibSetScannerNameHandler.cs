using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Pet
{
    public class ClientPlayerPathLibSetScannerNameHandler : IMessageHandler<IWorldSession, ClientPlayerPathLibSetScannerName>
    {
        public void HandleMessage(IWorldSession session, ClientPlayerPathLibSetScannerName petRename)
        {
            session.Player.PetCustomisationManager.RenamePet(petRename.PetType,
                petRename.PathScientistScanBotProfileId,
                petRename.Name);
        }
    }
}
