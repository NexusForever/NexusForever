using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Game
{
    public class Disable
    {
        public DisableType Type { get; }
        public uint ObjectId { get; }
        public string Note { get; }

        public Disable(DisableType type, uint objectId, string note)
        {
            Type     = type;
            ObjectId = objectId;
            Note     = note;
        }
    }
}
