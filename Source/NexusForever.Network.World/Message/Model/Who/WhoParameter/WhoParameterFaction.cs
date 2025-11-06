using NexusForever.Game.Static.Reputation;

namespace NexusForever.Network.World.Message.Model.Who
{
    public class WhoParameterFaction : IWhoParameterData
    {
        public Faction Faction2Id { get; set; }

        public void Read(GamePacketReader reader)
        {
            Faction2Id = reader.ReadEnum<Faction>(14u);
        }
    }
}
