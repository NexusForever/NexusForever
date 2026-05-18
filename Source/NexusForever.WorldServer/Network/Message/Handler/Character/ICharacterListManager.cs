namespace NexusForever.WorldServer.Network.Message.Handler.Character;

public interface ICharacterListManager
{
    void SendCharacterListPackets(IWorldSession session);
}
