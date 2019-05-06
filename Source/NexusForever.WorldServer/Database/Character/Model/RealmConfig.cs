using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class RealmConfig
    {
        public RealmConfig()
        {
            RealmConfigCustomLocation     = new HashSet<RealmConfigCustomLocation>();
            RealmConfigStartingLocation   = new HashSet<RealmConfigStartingLocation>();
        }

        public byte Id { get; set; }
        public byte Active { get; set; }

        public virtual ICollection<RealmConfigCustomLocation> RealmConfigCustomLocation { get; set; }
        public virtual ICollection<RealmConfigStartingLocation> RealmConfigStartingLocation { get; set; }
    }
}
