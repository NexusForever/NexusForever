using NexusForever.API.Model;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.API.Character.Character
{
    public static class CharacterMappingExtensions
    {
        public static Model.Character.Character ToCharacter(this CharacterModel model, Server.Server server)
        {
            return new Model.Character.Character()
            {
                AccountId = model.AccountId,
                Identity  = new Identity()
                {
                    RealmId = server.Id,
                    Id      = model.Id,
                },
                IdentityName = new IdentityName()
                {
                    RealmName = server.Name,
                    Name      = model.Name,
                },
                Sex         = (Sex)model.Sex,
                Race        = (Race)model.Race,
                Class       = (Class)model.Class,
                Path        = (Game.Static.Entity.Path)model.ActivePath,
                Faction     = (Faction)model.FactionId,
                RealmId     = server.Id, // TODO
                WorldId     = model.WorldId,
                WorldZoneId = model.WorldZoneId,
                Position = new Position
                {
                    X = model.LocationX,
                    Y = model.LocationY,
                    Z = model.LocationZ,
                },
                IsOnline   = model.IsOnline,
                LastOnline = !model.IsOnline ? model.LastOnline : null,
                Stats = model.Stat.Select(stat => new Model.Character.CharacterStat
                {
                    Stat  = (Stat)stat.Stat,
                    Value = stat.Value,
                }).ToList(),
            };
        }
    }
}
