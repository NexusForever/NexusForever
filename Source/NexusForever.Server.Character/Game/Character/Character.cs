using NexusForever.Database;
using NexusForever.Database.Query.Model;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using Path = NexusForever.Game.Static.PlayerPath.Path;

namespace NexusForever.Server.Character.Game.Character
{
    public class Character : IWrappedModel<CharacterModel>
    {
        public CharacterModel Model { get; private set; }

        public Identity Identity
        {
            get => new()
            {
                Id      = Model.CharacterId,
                RealmId = Model.RealmId
            };
        }

        public IdentityName IdentityName
        {
            get => new()
            {
                Name      = Model.Name,
                RealmName = Model.RealmName
            };
        }

        public Race Race => Model.Race;

        public Class Class => Model.Class;

        public Path Path
        {
            get => Model.Path;
            set => Model.Path = value;
        }

        public Faction Faction => Model.Faction;

        public Sex Sex => Model.Sex;

        public ushort CurrentRealmId => Model.CurrentRealmId;

        public ushort WorldZoneId
        {
            get => Model.WorldZoneId;
            set => Model.WorldZoneId = value;
        }

        public uint Level
        {
            get => Model.Level;
            set => Model.Level = value;
        }

        public string GuildName
        {
            get => Model.GuildName;
            set => Model.GuildName = value;
        }

        public DateTime? LastOnline
        {
            get => Model.LastOnline;
            set => Model.LastOnline = value;
        }

        public void Initialise(CharacterModel model)
        {
            if (Model != null)
                throw new InvalidOperationException("Character is already initialised.");

            Model = model;
        }
    }
}
