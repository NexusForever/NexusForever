using NexusForever.Game.Static.Entity;

namespace NexusForever.Network.World.Message.Model.Who.Parameter
{
    public class WhoParameterClass : IWhoParameterData
    {
        public Class ClassId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ClassId = reader.ReadEnum<Class>(14u);
        }
    }
}
