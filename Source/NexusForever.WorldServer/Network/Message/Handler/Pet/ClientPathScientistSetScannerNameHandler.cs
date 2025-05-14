using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Pet
{
    public class ClientPathScientistSetScannerNameHandler : IMessageHandler<IWorldSession, ClientPathScientistSetScannerName>
    {
        public void HandleMessage(IWorldSession session, ClientPathScientistSetScannerName setScannerName)
        {
            session.Player.PetCustomisationManager.RenamePet(setScannerName.PetType,
                setScannerName.PathScientistScanBotProfileId,
                setScannerName.Name);
        }
    }
}
