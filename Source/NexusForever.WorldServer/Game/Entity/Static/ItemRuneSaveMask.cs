using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum ItemRuneSaveMask
    {
        None     = 0x0000,
        Create   = 0x0001,
        RuneType = 0x0002,
        RuneItem = 0x0004,
    }
}
