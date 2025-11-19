using NexusForever.Game.Static.Entity;

namespace NexusForever.Network.World.Message.Model.Who.Parameter
{
    public class WhoParameterRace : IWhoParameterData
    {
        public Race RaceId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RaceId = reader.ReadEnum<Race>(14u);
        }
    }
}
