using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Story;
using NexusForever.Network.Message;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Network.World.Message.Model.Story
{
    public class StoryMessage : IWritable
    {
        public abstract class Actor : IWritable
        {
            public virtual StoryTextSourceType Type { get; }
            public uint TokenReplacementValue { get; set; }
            public string TokenName { get; set; } // used to specify a custom token name in a localized string i.e. $(direction) $(enemy_fac) $(resource_type)

            public virtual void Write(GamePacketWriter writer)
            {
                writer.Write(TokenReplacementValue);
                writer.WriteStringChar(TokenName);
            }
        }

        public sealed class Creature : Actor
        {
            public override StoryTextSourceType Type => StoryTextSourceType.Creature;
            public uint Creature2Id { get; set; }

            public override void Write(GamePacketWriter writer)
            {
                writer.Write(Creature2Id, 18u);
                base.Write(writer);
            }
        }

        public sealed class String : Actor
        {
            public override StoryTextSourceType Type => StoryTextSourceType.CustomText;
            public string Text { get; set; }

            public override void Write(GamePacketWriter writer)
            {
                writer.WriteStringWide(Text);
                base.Write(writer);
            }
        }

        public sealed class LocalizedText : Actor
        {
            public override StoryTextSourceType Type => StoryTextSourceType.LocalizedText;
            public uint LocalizedTextId { get; set; }

            public override void Write(GamePacketWriter writer)
            {
                writer.Write(LocalizedTextId, 21u);
                base.Write(writer);
            }
        }

        public sealed class Player : Actor
        {
            public override StoryTextSourceType Type => StoryTextSourceType.Player;
            public uint UnitId { get; set; }
            public string Name { get; set; }
            public uint Level { get; set; }
            public Sex Gender { get; set; }
            public Race Race { get; set; }
            public Class Class { get; set; }
            public Faction Faction { get; set; }
            public Path Path { get; set; }
            public ushort TitleId { get; set; }

            public override void Write(GamePacketWriter writer)
            {
                writer.Write(UnitId);
                writer.WriteStringWide(Name);
                writer.Write(Level);
                writer.Write(Gender, 2u);
                writer.Write(Race, 5u);
                writer.Write(Class, 5u);
                writer.Write(Faction, 14u);
                writer.Write(Path, 3u);
                writer.Write(TitleId, 14u);
                base.Write(writer);
            }
        }

        public sealed class CreatureUnit : Actor
        {
            public override StoryTextSourceType Type => StoryTextSourceType.CreatureUnit;
            public uint UnitId { get; set; }
            public uint Creature2Id { get; set; } // used if unit details have not been loaded on client

            public override void Write(GamePacketWriter writer)
            {
                writer.Write(UnitId);
                writer.Write(Creature2Id, 18u);
                base.Write(writer);
            }
        }

        public sealed class PlayerSelf : Actor
        {
            public override StoryTextSourceType Type => StoryTextSourceType.PlayerSelf;
            public uint PlayerUnitId { get; set; } // not used by the client

            public override void Write(GamePacketWriter writer)
            {
                writer.Write(PlayerUnitId);
                base.Write(writer);
            }
        }

        public uint MsgId { get; set; } // StoryPanelId for message 0x75A otherwise LocalizedTextId
        public uint RandomTextLineId { get; set; }
        public List<Actor> Actors { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MsgId);
            writer.Write(RandomTextLineId);
            writer.Write(Actors.Count, 8u);

            foreach (Actor actor in Actors)
            {
                writer.Write(actor.Type, 3u);
                actor.Write(writer);
            }
        }

        public void AddActor(Actor actor)
        {
            Actors.Add(actor);
        }

        public void AddCreature(uint creature2Id)
        {
            Actors.Add(new Creature
            {
                Creature2Id = creature2Id
            });
        }

        public void AddUnit(uint creature2Id, uint unitId)
        {
            Actors.Add(new CreatureUnit
            {
                Creature2Id = creature2Id,
                UnitId      = unitId
            });
        }

        public void AddPlayer(Player player)
        {
            Actors.Add(player);
        }

        public void AddString(string text)
        {
            Actors.Add(new String
            {
                Text = text
            });
        }

        public void AddTextId(uint textId)
        {
            Actors.Add(new LocalizedText
            {
                LocalizedTextId = textId
            });
        }
    }
}
