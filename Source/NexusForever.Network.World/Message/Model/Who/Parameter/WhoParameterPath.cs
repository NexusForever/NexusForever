using Path = NexusForever.Game.Static.PlayerPath.Path;

namespace NexusForever.Network.World.Message.Model.Who.Parameter
{
    public class WhoParameterPath : IWhoParameterData
    {
        public Path PathId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PathId = reader.ReadEnum<Path>(3u);
        }
    }
}
