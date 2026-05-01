namespace NexusForever.Network.World.Message.Model.Who.Parameter
{
    public class WhoParameterPlayer : IWhoParameterData
    {
        public string PlayerName { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PlayerName = reader.ReadWideString();
        }
    }
}
