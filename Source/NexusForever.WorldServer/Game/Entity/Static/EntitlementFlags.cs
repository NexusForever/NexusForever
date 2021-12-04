using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum EntitlementFlags
    {
        None      = 0,
        Character = 1,
        Disabled  = 2
    }
}
