using System.Numerics;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Trigger;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.PublicEvent;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.PublicEvent;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Adventure.TheHycrestInsurrection
{
    [ScriptFilterOwnerId(HycrestPublicEvent.Main)]
    public class TheHycrestInsurrectionEventScript : IPublicEventScript, IOwnedScript<IPublicEvent>
    {
        private const uint MissionVoteId = 45u;

        // vote 45 options in track order Merciful / Tactical / Militant: The Farmer's Daughter, The Science of Revenge,
        // Leveling the Field. Each track has a spokesperson who gives the outcome line; the story communicator is the
        // mission's setup message (none is known for The Science of Revenge).
        private static readonly (uint EventId, PublicEventCreature Speaker, uint OutcomeText, uint CommunicatorText)[] MissionVoteOptions =
        [
            (420u, PublicEventCreature.AyitaSinnatus,  465550u, 444047u), // "Prema's father Tarquim is right outside..."
            (421u, PublicEventCreature.VesnaTaranoft,  465551u, 0u),      // "Be quick about finding Groo..."
            (422u, PublicEventCreature.LysionSinnatus, 465552u, 444107u)  // "Once we've compromised their shields..."
        ];

        // barn scene, en-US text ids in speaking order (retail video); gestures for Vesna and Lysion
        private const uint BarnArrivalLine = 466926u; // Ayita: "Sometimes I worry about you, Father..."
        private static readonly (PublicEventCreature Speaker, uint TextId)[] BarnBriefing =
        [
            (PublicEventCreature.VesnaTaranoft,  160643u), // "Ah, you are here at last. Let us begin the briefing."
            (PublicEventCreature.VesnaTaranoft,  160648u), // "This is Lysion Sinnatus and his daughter, Ayita..."
            (PublicEventCreature.AyitaSinnatus,  160644u), // "All we want is to leave this horrid place..."
            (PublicEventCreature.LysionSinnatus, 160649u), // "To hell with leavin'. This is our home!..."
            (PublicEventCreature.VesnaTaranoft,  160650u), // "As you can see, we've had a few disagreements..."
            (PublicEventCreature.VesnaTaranoft,  455329u)  // "Regardless, there's plenty of work to be done..."
        ];
        private static readonly (PublicEventCreature Speaker, uint TextId)[] MissionVotePitches =
        [
            (PublicEventCreature.AyitaSinnatus,  464075u), // Merciful: "We have to save Prema and Milithia!..."
            (PublicEventCreature.VesnaTaranoft,  464079u), // Tactical: "We can't let emotion cloud our vision..."
            (PublicEventCreature.LysionSinnatus, 464076u)  // Militant: "Ayita, how many times must I tell you?..."
        ];

        // speech pacing: at least MinLineSeconds per line, longer lines get more time
        private const double MinLineSeconds = 4d;
        private const double CharactersPerSecond = 14d;

        private const uint CommunicatorDurationMs = 10000u;
        private static readonly TimeSpan OutcomeCommunicatorDelay = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan OutcomeMissionDelay = TimeSpan.FromSeconds(6);

        private IPublicEvent publicEvent;
        private IMapInstance mapInstance;

        private bool voteInProgress;
        private IPublicEvent mission;

        private readonly TimedActionQueue sceneQueue = new();
        private readonly Dictionary<PublicEventCreature, uint> npcGuids = [];
        private bool barnArrivalPlayed;
        private double sceneClock;
        private double barnArrivalLineEnd;

        #region Dependency Injection

        private readonly ILogger<TheHycrestInsurrectionEventScript> log;
        private readonly IGameTableManager gameTableManager;
        private readonly IStoryBuilder storyBuilder;
        private readonly HycrestDialogue dialogue;

        public TheHycrestInsurrectionEventScript(
            ILogger<TheHycrestInsurrectionEventScript> log,
            IGameTableManager gameTableManager,
            IStoryBuilder storyBuilder)
        {
            this.log              = log;
            this.gameTableManager = gameTableManager;
            this.storyBuilder     = storyBuilder;
            dialogue = new HycrestDialogue(gameTableManager, sceneQueue);
        }

        #endregion

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(IPublicEvent owner)
        {
            publicEvent = owner;
            mapInstance = publicEvent.Map as IMapInstance;

            // spawns Vesna Taranoft and Ayita Sinnatus in the Abandoned Barn
            publicEvent.SetPhase(0u);

            // the intro is a separate root event, players are joined to it by the map script
            publicEvent.Map.PublicEventManager.CreateEvent(HycrestPublicEvent.Intro);
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        /// <remarks>
        /// Forwarded by <see cref="TheHycrestInsurrectionMapScript"/> before the public event manager updates.
        /// </remarks>
        public void Update(double lastTick)
        {
            // runs before the public event manager update, so actions here may create events (see OnVoteFinished)
            sceneClock += lastTick;
            sceneQueue.Update(lastTick);
        }

        /// <summary>
        /// Invoked when a <see cref="IGridEntity"/> is added to the map the public event is on.
        /// </summary>
        public void OnAddToMap(IGridEntity entity)
        {
            switch (entity)
            {
                case IPlayer player:
                {
                    // late joiners also need to join the current mission, the map script only joins them to the main and intro events
                    if (mission != null)
                        HycrestPublicEvent.JoinPublicTeam(mission, player);
                    break;
                }
                case IWorldEntity worldEntity:
                    OnAddToMapWorldEntity(worldEntity);
                    break;
            }
        }

        private void OnAddToMapWorldEntity(IWorldEntity worldEntity)
        {
            var creature = (PublicEventCreature)worldEntity.CreatureId;
            switch (creature)
            {
                case PublicEventCreature.VesnaTaranoft:
                case PublicEventCreature.LysionSinnatus:
                    npcGuids[creature] = worldEntity.Guid;
                    break;
                case PublicEventCreature.AyitaSinnatus:
                {
                    npcGuids[creature] = worldEntity.Guid;
                    // retail: she sits on top of the hay bales; stand state is a stat, so players arriving later see it too
                    worldEntity.StandState = StandState.Sit;
                    break;
                }
            }
        }

        private IWorldEntity GetNpc(PublicEventCreature creature)
        {
            return npcGuids.TryGetValue(creature, out uint guid) ? mapInstance.GetEntity<IWorldEntity>(guid) : null;
        }

        private static bool UsesGesture(PublicEventCreature speaker)
        {
            return speaker is PublicEventCreature.VesnaTaranoft or PublicEventCreature.LysionSinnatus;
        }

        private TimeSpan GetLineDuration(uint textId)
        {
            return TimeSpan.FromSeconds(Math.Max(MinLineSeconds, dialogue.GetText(textId).Length / CharactersPerSecond));
        }

        /// <summary>
        /// Queue <paramref name="lines"/> one after another starting after <paramref name="start"/>, returns when the last line ends.
        /// </summary>
        private TimeSpan QueueLines(TimeSpan start, IEnumerable<(PublicEventCreature Speaker, uint TextId)> lines)
        {
            TimeSpan time = start;
            foreach ((PublicEventCreature speaker, uint textId) in lines)
            {
                sceneQueue.Enqueue(time, () => dialogue.Say(GetNpc(speaker), textId, UsesGesture(speaker)));
                time += GetLineDuration(textId);
            }

            return time;
        }

        /// <summary>
        /// Invoked by <see cref="TheHycrestInsurrectionIntroEventScript"/> when the first player enters the Abandoned Barn.
        /// </summary>
        public void OnFirstBarnArrival()
        {
            if (barnArrivalPlayed)
                return;

            barnArrivalPlayed  = true;
            barnArrivalLineEnd = sceneClock + GetLineDuration(BarnArrivalLine).TotalSeconds;
            dialogue.Say(GetNpc(PublicEventCreature.AyitaSinnatus), BarnArrivalLine, gesture: false);
        }

        /// <summary>
        /// Invoked by <see cref="TheHycrestInsurrectionIntroEventScript"/> when the intro has been completed.
        /// </summary>
        public void OnIntroComplete()
        {
            // everyone is gathered: Vesna's briefing, the three pitches, then the vote
            // solo, the first arrival also completes 189, so wait for Ayita's arrival line to finish
            TimeSpan start = TimeSpan.FromSeconds(Math.Max(0d, barnArrivalLineEnd - sceneClock));
            TimeSpan time = QueueLines(start, BarnBriefing);
            time = QueueLines(time, MissionVotePitches);
            sceneQueue.Enqueue(time, StartMissionVote);
        }

        /// <summary>
        /// Start the mission vote, unless one is already in progress.
        /// </summary>
        public void StartMissionVote()
        {
            if (voteInProgress)
                return;

            if (publicEvent.HasFinished)
            {
                // a finished event no longer ticks, so the vote would never time out
                log.LogWarning($"Hycrest: public event {HycrestPublicEvent.Main} has finished, restart the world server for a new instance.");
                return;
            }

            voteInProgress = true;
            publicEvent.StartVote(PublicEventTeam.PublicTeam, MissionVoteId, 0u);
            log.LogInformation($"Hycrest: started vote {MissionVoteId} for public event {HycrestPublicEvent.Main}.");
        }

        /// <summary>
        /// Invoked when a vote on the public event has finished.
        /// </summary>
        public void OnVoteFinished(uint voteId, uint winner)
        {
            voteInProgress = false;

            string label = null;

            PublicEventVoteEntry entry = gameTableManager.PublicEventVote.GetEntry(voteId);
            if (entry != null && winner < entry.LocalizedTextIdLabel.Length)
                label = gameTableManager.GetTextTable(Language.English).GetEntry(entry.LocalizedTextIdLabel[winner]);

            log.LogInformation($"Hycrest: vote {voteId} for public event {publicEvent.Id} finished, winner {winner} ({label ?? "unknown"}).");

            if (voteId != MissionVoteId || winner >= MissionVoteOptions.Length)
                return;

            // outcome line from the track's spokesperson, the mission's story communicator, then the mission
            // a vote that times out finishes during the public event manager update, creating an event there would modify
            // the collection being enumerated, so everything runs from the scene queue on the following ticks
            (uint eventId, PublicEventCreature speaker, uint outcomeText, uint communicatorText) = MissionVoteOptions[winner];
            sceneQueue.Enqueue(TimeSpan.Zero, () => dialogue.Say(GetNpc(speaker), outcomeText, UsesGesture(speaker)));
            if (communicatorText != 0u)
                sceneQueue.Enqueue(OutcomeCommunicatorDelay, () =>
                {
                    foreach (IPlayer player in mapInstance.GetPlayers())
                        storyBuilder.SendStoryCommunicator(communicatorText, (uint)speaker, player, CommunicatorDurationMs);
                });
            sceneQueue.Enqueue(OutcomeMissionDelay, () => StartMission(eventId));
        }

        private void StartMission(uint missionId)
        {
            // sub-events are never removed after they finish, a second CreateEvent for the same id would throw
            if (publicEvent.Map.PublicEventManager.GetEvent(missionId) != null)
            {
                log.LogInformation($"Hycrest: mission {missionId} already exists in this instance, restart the world server to try it again.");
                return;
            }

            mission = publicEvent.Map.PublicEventManager.CreateEvent(missionId);
            if (mission == null)
            {
                log.LogError($"Hycrest: failed to create mission {missionId}.");
                return;
            }

            foreach (IPlayer player in mapInstance.GetPlayers())
                HycrestPublicEvent.JoinPublicTeam(mission, player);

            log.LogInformation($"Hycrest: started mission {missionId} with {mapInstance.PlayerCount} player(s).");
        }
    }
}
