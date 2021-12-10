using NexusForever.WorldServer.Game.Guild.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Game.Guild
{
    internal class GuildNameEqualityComparer : IEqualityComparer<(GuildType, string)>
    {
        public bool Equals((GuildType, string) x, (GuildType, string) y)
        {
            if (x.Item1 == y.Item1)
            {
                if (ReferenceEquals(x.Item2, y.Item2))
                    return true;

                if (x.Item2 is null)
                    return false;

                if (y.Item2 is null)
                    return false;

                if (x.Item2.GetType() != y.Item2.GetType())
                    return false;

                return string.Equals(x.Item2, y.Item2, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        public int GetHashCode((GuildType, string) obj)
        {
            return base.GetHashCode();
        }
    }
}
