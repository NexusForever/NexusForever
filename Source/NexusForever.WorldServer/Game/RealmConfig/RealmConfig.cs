using System.Collections.Generic;
using NexusForever.WorldServer.Database.Character.Model;
using RealmConfigModel = NexusForever.WorldServer.Database.Character.Model.RealmConfig;

namespace NexusForever.WorldServer.Game.RealmConfig
{
    public class RealmConfig
    {
        private byte Id { get; }
        private byte Active { get; }

        public Dictionary<uint, CustomLocation> CustomLocations { get; } = new Dictionary<uint, CustomLocation>();
        public List<StartingLocation> StartingLocations { get; } = new List<StartingLocation>();

        public RealmConfig(RealmConfigModel model)
        {
            Id      = model.Id;
            Active  = model.Active;

            foreach (RealmConfigCustomLocation customLocationModel in model.RealmConfigCustomLocation)
            {
                var customLocation = new CustomLocation(customLocationModel);
                CustomLocations.Add(customLocation.CustomLocationId, customLocation);
            }

            foreach (RealmConfigStartingLocation startingLocationModel in model.RealmConfigStartingLocation)
            {
                var startingLocation = new StartingLocation(startingLocationModel);
                StartingLocations.Add(startingLocation);
            }
        }

        public RealmConfig()
        {
        }
    }
}
