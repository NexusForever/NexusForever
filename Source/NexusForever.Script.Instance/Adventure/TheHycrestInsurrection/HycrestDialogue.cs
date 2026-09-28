using System.Text.RegularExpressions;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Script.Instance.Adventure.TheHycrestInsurrection
{
    /// <summary>
    /// Plays scripted NPC lines from en-US.bin as NPC say (speech bubble and chat).
    /// </summary>
    /// <remarks>
    /// Lines are resolved to English text on the server.
    /// </remarks>
    public partial class HycrestDialogue
    {

        // talking gesture for lines with $(self.visual=5701) in en-US.bin (visual 5701 plays Default_Talk, 278). Cinematic
        // visual effects only show during a cinematic, so the gesture is an emote: 75, an NPC-only Default_Talk emote
        // with stand state Emote (the player "talk" emote 238, stand state Stand, didn't show on the hologram)
        public const uint TalkEmote = 75u;

        // the emote's stand state (Emote) keeps looping the animation until it is reset, so each talk is stopped after
        // about one Default_Talk cycle; overlapping talks keep it going until the last one ends
        private static readonly TimeSpan TalkDuration = TimeSpan.FromSeconds(4);

        [GeneratedRegex(@"\$\(self\.visual=(\d+)\)")]
        private static partial Regex SelfVisualRegex();

        [GeneratedRegex(@"\$[pm]?\(creature=(\d+)\)")]
        private static partial Regex CreatureRegex();

        private readonly IGameTableManager gameTableManager;
        private readonly TimedActionQueue actionQueue;
        private readonly Dictionary<uint, int> talking = [];

        public HycrestDialogue(IGameTableManager gameTableManager, TimedActionQueue actionQueue)
        {
            this.gameTableManager = gameTableManager;
            this.actionQueue      = actionQueue;
        }

        /// <summary>
        /// Resolve a text id to displayable English text: creature references are replaced by names, visual tags removed.
        /// </summary>
        public string GetText(uint textId)
        {
            string text = gameTableManager.TextEnglish.GetEntry(textId) ?? string.Empty;
            text = SelfVisualRegex().Replace(text, string.Empty);
            text = CreatureRegex().Replace(text, m =>
            {
                Creature2Entry entry = gameTableManager.Creature2.GetEntry(uint.Parse(m.Groups[1].Value));
                return entry != null ? gameTableManager.TextEnglish.GetEntry(entry.LocalizedTextIdName) ?? m.Value : m.Value;
            });

            return text.Trim();
        }

        /// <summary>
        /// Let <paramref name="speaker"/> say the line with <paramref name="textId"/>, optionally with the talking gesture.
        /// </summary>
        /// <remarks>
        /// When <paramref name="gesture"/> is null the gesture is played if the line has a $(self.visual=...) tag.
        /// </remarks>
        public void Say(IWorldEntity speaker, uint textId, bool? gesture = null)
        {
            if (speaker?.Map == null)
                return;

            string raw = gameTableManager.TextEnglish.GetEntry(textId) ?? string.Empty;
            speaker.NpcSay(GetText(textId));

            if (gesture ?? SelfVisualRegex().IsMatch(raw))
                PlayTalk(speaker);
        }

        /// <summary>
        /// Play the talking gesture on <paramref name="entity"/> for every player that can see it, for about one talk cycle.
        /// </summary>
        public void PlayTalk(IWorldEntity entity)
        {
            if (entity?.Map == null)
                return;

            entity.EnqueueToVisible(new ServerEmote
            {
                Guid       = entity.Guid,
                StandState = StandState.Emote,
                EmoteId    = TalkEmote
            });

            uint guid = entity.Guid;
            talking[guid] = talking.GetValueOrDefault(guid) + 1;
            actionQueue.Enqueue(TalkDuration, () => StopTalk(entity, guid));
        }

        private void StopTalk(IWorldEntity entity, uint guid)
        {
            int count = talking.GetValueOrDefault(guid) - 1;
            if (count > 0)
            {
                talking[guid] = count;
                return;
            }

            talking.Remove(guid);
            if (entity.Map == null)
                return;

            entity.EnqueueToVisible(new ServerEmote
            {
                Guid       = entity.Guid,
                StandState = StandState.Stand
            });
        }
    }
}
