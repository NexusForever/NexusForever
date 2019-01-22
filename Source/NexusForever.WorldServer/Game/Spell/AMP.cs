using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Spell
{
    public class AMP
    {
        public ushort Id { get; set; }
        public AMPSaveMask saveMask { get; set; }

        public AMP(ushort id, bool init = false)
        {
            Id       = id;
            if (!init)
                saveMask = AMPSaveMask.Create;
        }
    }
}
