using NexusForever.WorldServer.Database.Character;
using RealmConfigModel = NexusForever.WorldServer.Database.Character.Model.RealmConfig;

namespace NexusForever.WorldServer.Game.RealmConfig
{
    public static class RealmConfigManager
    {
        public static RealmConfig ActiveRealmConfig { get; private set; } = new RealmConfig();

        public static void Initialise()
        {
            RealmConfigModel model = CharacterDatabase.GetActiveRealmConfig();
            if (model == null)
                return;
            ActiveRealmConfig = new RealmConfig(model);
        }
    }
}
