using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class StoryMessage : IWritable
    {
        public abstract class Actor : IWritable
        {
            public virtual byte Type { get; }
            public List<byte> Unknown0 { get; set; } = new()
            {
                0
            };

            public virtual void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0.Count);
                Unknown0.ForEach(i => writer.Write(i));
            }
        }

        public sealed class Creature : Actor
        {
            public override byte Type => 0;
            public uint CreatureId { get; set; } // 18

            public override void Write(GamePacketWriter writer)
            {
                writer.Write(CreatureId, 18u);
                base.Write(writer);
            }
        }

        public sealed class String : Actor
        {
            public override byte Type => 1;
            public string Text { get; set; }

            public override void Write(GamePacketWriter writer)
            {
                writer.WriteStringWide(Text);
                base.Write(writer);
            }
        }

        public sealed class TextId : Actor
        {
            public override byte Type => 2;
            public uint Id { get; set; } // 21

            public override void Write(GamePacketWriter writer)
            {
                writer.Write(Id, 21u);
                base.Write(writer);
            }
        }

        public sealed class Player : Actor
        {
            public override byte Type => 3;
            public uint PlayerGuid { get; set; }
            public string PlayerName { get; set; }
            public uint PlayerLevel { get; set; }
            public Sex PlayerGender { get; set; } // 2
            public Race PlayerRace { get; set; } // 5
            public Class PlayerClass { get; set; } // 5
            public Faction PlayerFaction { get; set; } // 14
            public Path PlayerPath { get; set; } // 3
            public ushort PlayerTitle { get; set; } // 14

            public override void Write(GamePacketWriter writer)
            {
                writer.Write(PlayerGuid);
                writer.WriteStringWide(PlayerName);
                writer.Write(PlayerLevel);
                writer.Write(PlayerGender, 2u);
                writer.Write(PlayerRace, 5u);
                writer.Write(PlayerClass, 5u);
                writer.Write(PlayerFaction, 14u);
                writer.Write(PlayerPath, 3u);
                writer.Write(PlayerTitle, 14u);
                base.Write(writer);
            }
        }

        public sealed class CreatureUnit : Actor
        {
            public override byte Type => 4;
            public uint CreatureUnitId { get; set; }
            public uint CreatureId { get; set; } // 18

            public override void Write(GamePacketWriter writer)
            {
                writer.Write(CreatureUnitId);
                writer.Write(CreatureId, 18u);
                base.Write(writer);
            }
        }

        public sealed class PlayerUnit : Actor
        {
            public override byte Type => 5;
            public uint LocalPlayerUnitId { get; set; }

            public override void Write(GamePacketWriter writer)
            {
                writer.Write(LocalPlayerUnitId);
                base.Write(writer);
            }
        }

        public uint MsgId { get; set; }
        public uint GeneralVoId { get; set; }
        public List<Actor> Actors { get; set; } = new(); // 8

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MsgId);
            writer.Write(GeneralVoId);
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

        public void AddCreature(uint creatureId)
        {
            Actors.Add(new Creature
            {
                CreatureId = creatureId
            });
        }

        public void AddUnit(uint creatureId, uint guid)
        {
            Actors.Add(new CreatureUnit
            {
                CreatureId     = creatureId,
                CreatureUnitId = guid
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
            Actors.Add(new TextId
            {
                Id = textId
            });
        }
    }
}
