using NexusForever.Game.Static.Entity;

namespace NexusForever.Network.World.Message.Model.Who
{
    public class WhoParameterClass : IWhoParameterData
    {
        public Class ClassId { get; set; }

        public void Read(GamePacketReader reader)
        {
            ClassId = reader.ReadEnum<Class>(14u);
        }
    }
}
