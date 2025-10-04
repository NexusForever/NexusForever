using NexusForever.Network.Internal.Message.Group.Shared;

namespace NexusForever.Server.GroupServer.Character
{
    public static class CharacterMappingExtensions
    {
        public static GroupCharacter ToGroupCharacter(this Character character)
        {
            return new GroupCharacter
            {
                Name               = character.IdentityName.Name,
                RealmName          = character.IdentityName.RealmName,
                Faction            = character.Faction,
                Race               = character.Race,
                Class              = character.Class,
                Sex                = character.Sex,
                Level              = character.Level,
                EffectiveLevel     = character.EffectiveLevel,
                Path               = character.Path,
                Health             = character.Health,
                HealthMax          = character.MaxHealth,
                Shield             = character.Shield,
                ShieldMax          = character.MaxShield,
                InterruptArmour    = character.InterruptArmour,
                InterruptArmourMax = character.MaxInterruptArmour,
                Absorption         = character.Absorption,
                AbsorptionMax      = character.MaxAbsorption,
                Focus              = character.Focus,
                FocusMax           = character.MaxFocus,
                HealingAbsorb      = character.HealingAbsorb,
                HealingAbsorbMax   = character.MaxHealingAbsorb,
                RealmId            = character.CurrentRealmId,
                WorldZoneId        = character.WorldZoneId,
                WorldId            = character.WorldId,
                Position           = new NexusForever.Network.Internal.Message.Shared.Position
                {
                    X = character.Position.X,
                    Y = character.Position.Y,
                    Z = character.Position.Z
                },
                PhaseFlags1 = character.PhaseFlags1,
                PhaseFlags2 = character.PhaseFlags2,
            };
        }
    }
}
