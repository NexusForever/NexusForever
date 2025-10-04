using System;

namespace NexusForever.Database.World.Model
{
    public class VersionModel
    {
        public string FileName { get; set; }
        public string FileHash { get; set; }
        public DateTime AppliedOn { get; set; }
    }
}
