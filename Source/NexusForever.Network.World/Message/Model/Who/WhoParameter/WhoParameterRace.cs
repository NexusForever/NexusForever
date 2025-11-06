using NexusForever.Game.Static.Entity;

namespace NexusForever.Network.World.Message.Model.Who
{
    public class WhoParameterRace : IWhoParameterData
    {
        public Race RaceId { get; set; }

        public void Read(GamePacketReader reader)
        {
            RaceId = reader.ReadEnum<Race>(14u);
        }
    }
}
