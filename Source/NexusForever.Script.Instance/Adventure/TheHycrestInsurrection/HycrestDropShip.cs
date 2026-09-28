using System.Numerics;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Entity.Movement.Spline;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.Script.Instance.Adventure.TheHycrestInsurrection
{
    /// <summary>
    /// The intro drop ship: boards arriving players, gives players who leave it the slow-burn jetpack (Rocket Fall)
    /// until they land, and sends the ship away.
    /// </summary>
    public class HycrestDropShip
    {
        private enum DropState
        {
            OnBoard,
            Falling,
            Landed
        }

        private class PlayerDrop
        {
            public DropState State;
            public Vector3 Spot;
            public bool EnteredOnDeck;
            public bool Boarded;
            public double SinceLoaded;
            public double SinceBoarded;
            public bool ReachedDeck;
            public double SinceCast;
            public bool CastConfirmed;
            public float SampleY;
            public double SinceSample;
            public double SinceFall;
        }

        // boarding waits this long after the client has finished loading the map (it can teleport again), and the
        // teleport shows a loading screen; teleporting in the same tick as ClientEnteredWorld left a client in an empty
        // void once
        public const double BoardAfterLoad = 1.5d;

        // after boarding, position and platform reports are ignored this long: reports sent before the client applied
        // the teleport (e.g. still falling from the login position) must not count as leaving the ship
        private const double BoardGracePeriod = 2d;

        // Rocket Fall: GravityMultiplier 0.1 and no fall damage for 3 s, jetpack/flame visuals
        // re-applied just before it runs out so there is no gap
        private const uint RocketFallSpell = 47734u;
        private const double RocketFallRecast = 2.9d;
        // a cast that didn't take (e.g. the player was still casting) is tried again after this
        private const double RocketFallRetry = 0.5d;

        // landed: within LandedHeight of the terrain (map file; props like roofs aren't in it), or no longer falling
        // (moved less than LandedMaxDrop vertically within LandedWindow) once the player has fallen for at least
        // MinFallTime; right after leaving the ship the fall is still too slow to tell
        private const float LandedHeight   = 2f;
        private const float LandedMaxDrop  = 0.3f;
        private const double LandedWindow  = 1d;
        private const double MinFallTime   = 5d;
        private const double MaxFallTime   = 120d;

        // the Imperium Transport (17722) always spawns with both doorways open and its ramps out; like retail, the door
        // entities (Right 18338, Left 28509, platforms) close the doorways. States are driven like DoorEntity (StandState
        // stat + emote, the models' AP_State sequences): ship State1 hovering (engines shake), State2 "jump away" (13 s);
        // doors State0 closed, State1 open. Only the right door opens, the left one stays closed.
        private static readonly TimeSpan DepartRemoveDelay = TimeSpan.FromSeconds(14);

        private readonly IMapInstance map;
        private readonly IFactory<ISpellParameters> spellParametersFactory;
        private readonly ILogger log;
        private readonly TimedActionQueue actionQueue;

        // keyed by character id: a teleport within the map removes and re-adds the player, possibly with a new guid
        private readonly Dictionary<ulong, PlayerDrop> players = [];
        private int nextSpot;

        public uint ShipGuid { get; set; }
        public uint RightDoorGuid { get; set; }
        public uint LeftDoorGuid { get; set; }
        public uint DawsonGuid { get; set; }
        public uint HologramGuid { get; set; }

        public bool FlewIn { get; private set; }

        /// <summary>
        /// The ship is at its hover point (flown in, or never had to). Players are only put on board then: carrying players
        /// on a moving platform the client doesn't carry made them fall through the floor.
        /// </summary>
        public bool Arrived { get; private set; }

        /// <summary>
        /// The ship is flying in; players are only put on board while it stands still.
        /// </summary>
        public bool Moving => FlewIn && !Arrived;

        public bool DoorsOpen { get; private set; }
        public bool Departed { get; private set; }

        /// <summary>
        /// Seconds since the first player was put on board (the client had finished loading), null until then.
        /// </summary>
        public double? SinceFirstBoard { get; private set; }

        /// <summary>
        /// Show a loading screen for the boarding teleport; not needed while the intro text's black screen hides it.
        /// </summary>
        public bool BoardWithLoadingScreen { get; set; } = true;

        /// <summary>
        /// Invoked when a player has been put on board.
        /// </summary>
        public event Action<IPlayer> PlayerBoarded;

        public HycrestDropShip(IMapInstance map, IFactory<ISpellParameters> spellParametersFactory, ILogger log, TimedActionQueue actionQueue)
        {
            this.map                    = map;
            this.spellParametersFactory = spellParametersFactory;
            this.log                    = log;
            this.actionQueue            = actionQueue;
        }

        /// <summary>
        /// Put <paramref name="player"/> on the ship's deck, unless the ship has already left or the player has boarded before.
        /// </summary>
        /// <remarks>
        /// The group finder entrance has to be a WorldLocation2 point and there is none on the moved ship, so arriving
        /// players are teleported onto the deck. The teleport waits until the client has finished loading the map,
        /// a local teleport is refused while the map transfer is still pending.
        /// </remarks>
        public void Board(IPlayer player)
        {
            if (Departed || players.ContainsKey(player.CharacterId))
                return;

            Vector3 spot = HycrestShipLayout.PlayerSpots[nextSpot++ % HycrestShipLayout.PlayerSpots.Length];
            players[player.CharacterId] = new PlayerDrop
            {
                State   = DropState.OnBoard,
                Spot    = spot,
                SampleY = spot.Y
            };
        }

        /// <summary>
        /// Reserve a spot on the deck for <paramref name="player"/> entering the map and return its position, so the player
        /// enters the map standing on the ship; null if the ship has left or is moving.
        /// </summary>
        /// <remarks>
        /// Invoked before the player (and, for the first player, the ship) is added to the map: the position comes from
        /// where the ship stands, its start point before the fly-in and the hover point after it.
        /// </remarks>
        public Vector3? ReserveEntry(IPlayer player)
        {
            if (Departed || Moving || players.ContainsKey(player.CharacterId))
                return null;

            Board(player);
            PlayerDrop drop = players[player.CharacterId];
            drop.EnteredOnDeck = true;

            return Arrived
                ? drop.Spot
                : HycrestShipLayout.OnShip(drop.Spot, HycrestShipLayout.StartPoint, HycrestShipLayout.StartYaw);
        }

        /// <summary>
        /// Set the stand state of <paramref name="entity"/>, which plays its model's AP_State transition, like <c>DoorEntity</c>.
        /// </summary>
        public static void SetState(IWorldEntity entity, StandState state)
        {
            if (entity == null)
                return;

            entity.StandState = state;
            entity.EnqueueToVisible(new ServerEmote
            {
                Guid       = entity.Guid,
                StandState = state
            });
        }

        /// <summary>
        /// Open the exit door so players can walk down the ramp and jump; the other door stays closed.
        /// </summary>
        public void OpenDoors()
        {
            if (DoorsOpen)
                return;

            DoorsOpen = true;
            // the exit is the ramp on the ship's right (towards the barn); in game the "Left" door entity (28509) is the
            // one that closes that doorway
            SetState(map.GetEntity<IWorldEntity>(LeftDoorGuid), StandState.State1);

            log.LogInformation("Hycrest: drop ship exit door opened.");
        }

        /// <summary>
        /// Fly the ship with everyone on board from its start point forward to its hover point, then turn it right into its
        /// hover rotation (see <see cref="Arrived"/>).
        /// </summary>
        /// <remarks>
        /// The doors, the hologram and the players on the deck become platform passengers of the ship: their positions are
        /// relative to it, so clients move and turn them with the ship.
        /// </remarks>
        public void FlyIn()
        {
            if (FlewIn || Departed)
                return;

            FlewIn = true;

            IWorldEntity ship = map.GetEntity<IWorldEntity>(ShipGuid);
            if (ship == null)
            {
                Arrived = true;
                return;
            }

            AttachToShip(ship, RightDoorGuid, HycrestShipLayout.DoorPoint, HycrestShipLayout.Yaw);
            AttachToShip(ship, LeftDoorGuid, HycrestShipLayout.DoorPoint, HycrestShipLayout.Yaw);
            AttachToShip(ship, HologramGuid, HycrestShipLayout.HologramSpot, HycrestShipLayout.HologramYaw);

            foreach (IPlayer player in map.GetPlayers())
                if (players.TryGetValue(player.CharacterId, out PlayerDrop drop) && drop.Boarded && drop.State == DropState.OnBoard)
                    AttachPlayer(player, ship);

            float distance = Vector3.Distance(ship.Position, HycrestShipLayout.Origin);
            uint flyMs  = (uint)(distance / HycrestShipLayout.FlyInSpeed * 1000f);
            uint turnMs = (uint)HycrestShipLayout.TurnDuration.TotalMilliseconds;

            ship.MovementManager.SetPositionKeys([0u, flyMs], [ship.Position, HycrestShipLayout.Origin]);

            // like retail the ship banks into the turn: it starts turning just before it reaches the hover point, rolls
            // into the turn (rotation is yaw, pitch, roll) and levels out as it comes to its final heading
            uint turnStart = flyMs > HycrestShipLayout.TurnLead ? flyMs - HycrestShipLayout.TurnLead : 0u;
            float startYaw = ship.Rotation.X;
            var banked = new Vector3(startYaw + (HycrestShipLayout.Yaw - startYaw) / 2f, 0f, HycrestShipLayout.TurnBank);
            ship.MovementManager.SetRotationKeys([0u, turnStart, turnStart + turnMs / 2u, turnStart + turnMs],
                [ship.Rotation, ship.Rotation, banked, new Vector3(HycrestShipLayout.Yaw, 0f, 0f)]);

            // a moment of margin for the keys to settle before late players are put on board
            actionQueue.Enqueue(TimeSpan.FromMilliseconds(flyMs + turnMs + 500u), () => Arrived = true);

            log.LogInformation($"Hycrest: drop ship flying in ({distance:0} m), then turning.");
        }

        private void AttachToShip(IWorldEntity ship, uint guid, Vector3 hoverPosition, float hoverYaw)
        {
            IWorldEntity entity = map.GetEntity<IWorldEntity>(guid);
            entity?.SetPlatform(ship, HycrestShipLayout.ToLocal(hoverPosition), new Vector3(hoverYaw - HycrestShipLayout.Yaw, 0f, 0f));
        }

        private static void AttachPlayer(IPlayer player, IWorldEntity ship)
        {
            Vector3 local = HycrestShipLayout.ToShip(player.Position, ship.Position, ship.Rotation.X);
            if (!HycrestShipLayout.IsOnDeck(local))
                return;

            // like a teleport: the server sets the position (now relative to the ship) while it has control
            player.SetControl(null);
            player.SetPlatform(ship, local, new Vector3(player.Rotation.X - ship.Rotation.X, 0f, 0f));
            player.MovementManager.BroadcastNetworkEntityCommands();
            player.SetControl(player);
        }

        /// <summary>
        /// Send the ship away ("jump away", State2). Players still on board are moved to the drop point first.
        /// </summary>
        public void Depart()
        {
            if (Departed)
                return;

            Departed = true;
            OpenDoors();

            IWorldEntity departingShip = map.GetEntity<IWorldEntity>(ShipGuid);
            foreach (IPlayer player in map.GetPlayers())
            {
                if (!players.TryGetValue(player.CharacterId, out PlayerDrop drop)
                    || drop.State != DropState.OnBoard
                    || !drop.Boarded)
                    continue;

                // only players actually still on the deck: someone who already fell or walked off stays where they are
                if (departingShip != null
                    && !HycrestShipLayout.IsAboard(HycrestShipLayout.ToShip(player.Position, departingShip.Position, departingShip.Rotation.X)))
                {
                    drop.State = DropState.Landed;
                    continue;
                }

                StartFalling(player, drop);
                player.TeleportToLocal(HycrestShipLayout.DropPoint, false);
            }

            map.GetEntity<IWorldEntity>(DawsonGuid)?.RemoveFromMap();

            // the doors don't belong to the ship model and would stay behind in the air
            map.GetEntity<IWorldEntity>(RightDoorGuid)?.RemoveFromMap();
            map.GetEntity<IWorldEntity>(LeftDoorGuid)?.RemoveFromMap();

            IWorldEntity ship = map.GetEntity<IWorldEntity>(ShipGuid);
            if (ship != null)
            {
                SetState(ship, StandState.State2);
                ship.MovementManager.SetPositionPath([ship.Position, ship.Position + HycrestShipLayout.DepartOffset],
                    SplineType.Linear, SplineMode.OneShot, HycrestShipLayout.DepartSpeed);
            }

            actionQueue.Enqueue(DepartRemoveDelay, () => map.GetEntity<IWorldEntity>(ShipGuid)?.RemoveFromMap());

            log.LogInformation("Hycrest: drop ship departed.");
        }

        /// <summary>
        /// Returns if every player in the instance has left the ship.
        /// </summary>
        public bool EveryoneOff()
        {
            return map.GetPlayers().All(p => !players.TryGetValue(p.CharacterId, out PlayerDrop drop) || drop.State != DropState.OnBoard);
        }

        /// <summary>
        /// Invoked each tick: detect players leaving the ship, keep Rocket Fall on them until they land.
        /// </summary>
        public void Update(double lastTick)
        {
            if (SinceFirstBoard.HasValue)
                SinceFirstBoard += lastTick;

            foreach (IPlayer player in map.GetPlayers())
            {
                if (!players.TryGetValue(player.CharacterId, out PlayerDrop drop))
                    continue;

                if (!drop.Boarded)
                {
                    if (Departed)
                    {
                        // never made it on board, nothing to fall from
                        drop.State = DropState.Landed;
                        continue;
                    }

                    if (!player.CanTeleport())
                    {
                        drop.SinceLoaded = 0d;
                        continue;
                    }

                    if (drop.EnteredOnDeck)
                    {
                        // entered the map standing on the deck, nothing to teleport
                        drop.Boarded = true;
                        SinceFirstBoard ??= 0d;
                        PlayerBoarded?.Invoke(player);
                        continue;
                    }

                    if (Moving)
                    {
                        // loaded, waiting for the ship to stand still
                        drop.SinceLoaded = Math.Max(drop.SinceLoaded, BoardAfterLoad);
                        continue;
                    }

                    drop.SinceLoaded += lastTick;
                    if (drop.SinceLoaded < BoardAfterLoad)
                        continue;

                    drop.Boarded = true;
                    SinceFirstBoard ??= 0d;
                    // spots are given for the ship at its hover point; it may still be at its start point or flying in
                    IWorldEntity ship = map.GetEntity<IWorldEntity>(ShipGuid);
                    Vector3 spot = ship != null ? HycrestShipLayout.OnShip(drop.Spot, ship.Position, ship.Rotation.X) : drop.Spot;
                    player.TeleportToLocal(spot, BoardWithLoadingScreen);
                    PlayerBoarded?.Invoke(player);
                    continue;
                }

                switch (drop.State)
                {
                    case DropState.OnBoard:
                        drop.SinceBoarded += lastTick;
                        if (drop.SinceBoarded < BoardGracePeriod)
                            break;
                        UpdateOnBoard(player, drop);
                        break;
                    case DropState.Falling:
                        UpdateFalling(player, drop, player.Position.Y, lastTick);
                        break;
                }
            }
        }

        private void UpdateOnBoard(IPlayer player, PlayerDrop drop)
        {
            // aboard = inside the ship's volume (deck and ramps) in the ship's own frame, wherever it is and however it is
            // turned; the client's platform reports stopped while the ship flew with the player standing on it
            IWorldEntity ship = map.GetEntity<IWorldEntity>(ShipGuid);
            if (ship == null)
                return;

            Vector3 local = HycrestShipLayout.ToShip(player.Position, ship.Position, ship.Rotation.X);
            if (HycrestShipLayout.IsAboard(local))
            {
                drop.ReachedDeck = true;
                return;
            }

            // right after boarding the teleport hasn't completed yet and the position is still on the ground
            if (!drop.ReachedDeck)
                return;

            StartFalling(player, drop);
        }

        private void StartFalling(IPlayer player, PlayerDrop drop)
        {
            drop.State       = DropState.Falling;
            drop.SampleY     = player.Position.Y;
            drop.SinceSample = 0d;
            drop.SinceFall   = 0d;
            CastRocketFall(player, drop);
        }

        private void UpdateFalling(IPlayer player, PlayerDrop drop, float y, double lastTick)
        {
            drop.SinceFall   += lastTick;
            drop.SinceSample += lastTick;

            float? ground = map.GetTerrainHeight(player.Position.X, player.Position.Z);
            if ((ground.HasValue && y - ground.Value < LandedHeight) || drop.SinceFall >= MaxFallTime)
            {
                drop.State = DropState.Landed;
                return;
            }

            if (drop.SinceSample >= LandedWindow)
            {
                if (drop.SinceFall >= MinFallTime && Math.Abs(y - drop.SampleY) < LandedMaxDrop)
                {
                    drop.State = DropState.Landed;
                    return;
                }

                drop.SampleY     = y;
                drop.SinceSample = 0d;
            }

            drop.SinceCast += lastTick;

            // a cast that didn't take (e.g. the player was still casting) is tried again; checked a moment after the cast,
            // right after it the spell isn't registered yet (checking then cast it twice)
            if (!drop.CastConfirmed && drop.SinceCast >= RocketFallRetry)
            {
                if (player.GetSpellBySpellId(RocketFallSpell) != null)
                    drop.CastConfirmed = true;
                else
                    CastRocketFall(player, drop);
                return;
            }

            if (drop.SinceCast >= RocketFallRecast)
                CastRocketFall(player, drop);
        }

        private void CastRocketFall(IPlayer player, PlayerDrop drop)
        {
            drop.SinceCast     = 0d;
            drop.CastConfirmed = false;

            // cast by the script, not the player: skips the cooldown and global cooldown checks (a player cast just before
            // the jump put Rocket Fall on the global cooldown and the cast failed)
            ISpellParameters parameters = spellParametersFactory.Resolve();
            parameters.PrimaryTargetId        = player.Guid;
            parameters.UserInitiatedSpellCast = false;
            player.CastSpell(RocketFallSpell, parameters);
        }
    }
}
