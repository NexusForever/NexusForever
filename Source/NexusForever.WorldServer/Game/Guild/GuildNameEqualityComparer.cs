using NexusForever.WorldServer.Game.Guild.Static;
using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Game.Guild
{
    internal class GuildNameEqualityComparer : IEqualityComparer<(GuildType Type, string Name)>
    {
        public bool Equals((GuildType Type, string Name) x, (GuildType Type, string Name) y)
        {
            if (x.Type == y.Type)
            {
                if (ReferenceEquals(x.Name, y.Name))
                    return true;

                if (x.Name is null)
                    return false;

                if (y.Name is null)
                    return false;

                if (x.Name.GetType() != y.Name.GetType())
                    return false;

                return string.Equals(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        public int GetHashCode((GuildType Type, string Name) obj)
        {
            return base.GetHashCode();
        }
    }
}
