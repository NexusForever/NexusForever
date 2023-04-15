using NexusForever.Game.Abstract;
using NexusForever.Game.Static;

namespace NexusForever.Game
{
    public class Disable : IDisable
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
