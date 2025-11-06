using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Network.World.Message.Model.Who
{
    public class WhoParameterPath : IWhoParameterData
    {
        public Path PathId { get; set; }

        public void Read(GamePacketReader reader)
        {
            PathId = reader.ReadEnum<Path>(3u);
        }
    }
}
