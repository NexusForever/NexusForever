using System.Numerics;

namespace NexusForever.Script.Instance.Adventure.TheHycrestInsurrection
{
    /// <summary>
    /// Where the intro drop ship hovers and where things stand inside it. Keep in sync with the world database spawns
    /// (Instance/Adventure/The Hycrest Insurrection.sql).
    /// </summary>
    /// <remarks>
    /// The ship is the Dominion Imperium Transport 17722 (confirmed against the retail videos), with the door entities
    /// 18338 and 28509 over its always-open doorways, like retail. Offsets were measured in game on summoned copies
    /// and are in the ship's own frame (right ramp along +X, doorway at -17 m); a yaw r turns a local
    /// offset (x, z) into world (x cos r + z sin r, -x sin r + z cos r). The ship is turned -90 degrees so the right ramp
    /// points north and its lower end touches down in front of the Abandoned Barn.
    /// </remarks>
    public static class HycrestShipLayout
    {
        /// <summary>
        /// The ship's position: the right ramp's lower end (15.60, -9.23, -17.19) is above the spot in front of the barn
        /// (-2520.6306, -929.33386, -1229.9689, measured), the floor 60 m above that ground (retail: ~90 m), so players
        /// jump from the ramp and glide down with Rocket Fall.
        /// </summary>
        public static readonly Vector3 Origin = new(-2537.821f, -865.644f, -1245.569f);
        public const float Yaw = -1.5708f;

        /// <summary>
        /// Where the ship spawns before flying in to <see cref="Origin"/> with the players on board: above the start point
        /// measured in game (-2543.603, -921.8223, -1151.4386), north of the barn, at the same altitude. It starts facing
        /// its flight direction (<see cref="StartYaw"/>, the nose at local -Z points to world -Z), flies forward to the
        /// hover point and then turns to <see cref="Yaw"/> in <see cref="TurnDuration"/>.
        /// </summary>
        public static readonly Vector3 StartPoint = new(-2543.603f, -865.644f, -1151.4386f);
        public const float StartYaw = 0f;
        public const float FlyInSpeed = 7f;
        public static readonly TimeSpan TurnDuration = TimeSpan.FromSeconds(5);

        // the turn starts this long (ms) before the ship reaches the hover point, and it banks (rolls) this much (radians,
        // about 10 degrees) halfway through; negative leans into the right turn
        public const uint TurnLead = 1500u;
        public const float TurnBank = -0.17f;

        /// <summary>
        /// Departure ("jump away", State2): the ship moves forward and up. ASSUMPTION: the cockpit is at the ship's -Z end,
        /// which is east (+X) with the ship turned -90 degrees.
        /// </summary>
        public static readonly Vector3 DepartOffset = new(250f, 80f, 0f);
        public const float DepartSpeed = 20f;

        /// <summary>
        /// Return the offset in the ship's own frame of <paramref name="hoverPosition"/> (a spot given for the ship at
        /// <see cref="Origin"/> turned <see cref="Yaw"/>).
        /// </summary>
        public static Vector3 ToLocal(Vector3 hoverPosition)
        {
            Vector3 d = hoverPosition - Origin;
            float cos = MathF.Cos(Yaw);
            float sin = MathF.Sin(Yaw);
            return new Vector3(d.X * cos - d.Z * sin, d.Y, d.X * sin + d.Z * cos);
        }

        /// <summary>
        /// Return the world position of <paramref name="local"/> (an offset in the ship's frame) for the ship at
        /// <paramref name="shipPosition"/> turned <paramref name="shipYaw"/>.
        /// </summary>
        public static Vector3 ToWorld(Vector3 local, Vector3 shipPosition, float shipYaw)
        {
            float cos = MathF.Cos(shipYaw);
            float sin = MathF.Sin(shipYaw);
            return new Vector3(local.X * cos + local.Z * sin, local.Y, -local.X * sin + local.Z * cos) + shipPosition;
        }

        /// <summary>
        /// Return the offset in the ship's own frame of <paramref name="world"/> for the ship at <paramref name="shipPosition"/>
        /// turned <paramref name="shipYaw"/>.
        /// </summary>
        public static Vector3 ToShip(Vector3 world, Vector3 shipPosition, float shipYaw)
        {
            Vector3 d = world - shipPosition;
            float cos = MathF.Cos(shipYaw);
            float sin = MathF.Sin(shipYaw);
            return new Vector3(d.X * cos - d.Z * sin, d.Y, d.X * sin + d.Z * cos);
        }

        /// <summary>
        /// Return true if <paramref name="local"/> (an offset in the ship's frame) is aboard: on the deck or on one of the
        /// ramps (the right ramp runs from local (6.3, -3.0, -17.1) down to (15.6, -9.2, -17.2), the left one mirrored).
        /// </summary>
        /// <remarks>
        /// Used to tell if a player has left the ship: the client's platform reports can't be relied on (they stopped while
        /// the ship flew with the player standing on it).
        /// </remarks>
        public static bool IsAboard(Vector3 local)
        {
            return local.Y > -10.5f && local.Y < 1f
                && MathF.Abs(local.X) < 17f
                && MathF.Abs(local.Z) < 25f;
        }

        /// <summary>
        /// Return true if <paramref name="local"/> (an offset in the ship's frame) is on the deck, inside the hull.
        /// </summary>
        public static bool IsOnDeck(Vector3 local)
        {
            return MathF.Abs(local.Y - (FloorY - Origin.Y)) < 2.5f
                && MathF.Abs(local.X) < 12f
                && MathF.Abs(local.Z) < 25f;
        }

        /// <summary>
        /// Return <paramref name="hoverPosition"/> (a spot given for the ship at <see cref="Origin"/>) for the ship at
        /// <paramref name="shipPosition"/> turned <paramref name="shipYaw"/>.
        /// </summary>
        public static Vector3 OnShip(Vector3 hoverPosition, Vector3 shipPosition, float shipYaw)
        {
            return ToWorld(ToLocal(hoverPosition), shipPosition, shipYaw);
        }

        /// <summary>
        /// Height of the ship's interior floor (3.69 m below the ship's position).
        /// </summary>
        public const float FloorY = -869.334f;

        // arrival spots, measured standing (+0.05 m; +0.5 m dropped players onto the floor); one per party member, the sixth is spare
        public static readonly Vector3[] PlayerSpots =
        [
            new(-2525.451f, -869.334f + 0.05f, -1243.369f),
            new(-2527.751f, -868.914f + 0.05f, -1244.049f),
            new(-2531.051f, -868.334f + 0.05f, -1244.999f),
            new(-2530.411f, -868.314f + 0.05f, -1247.769f),
            new(-2526.551f, -869.384f + 0.05f, -1247.689f),
            new(-2523.891f, -869.344f + 0.05f, -1248.039f)
        ];

        // in front of the door with the red light strip, where the Caretaker hologram stands, and Dawson's spot, measured
        public static readonly Vector3 HologramSpot = new(-2520.181f, -869.334f, -1245.839f);
        public const float HologramYaw = 1.6108f;
        public static readonly Vector3 DawsonSpot = new(-2519.001f, -869.264f, -1246.529f);
        public const float DawsonYaw = 1.5913f;

        // both door entities (18338/28509) stand here (centre line, 13.28 m forward, 3.28 m down), turned like the ship;
        // 28509 covers the exit doorway (right ramp)
        public static readonly Vector3 DoorPoint = new(-2524.541f, -868.924f, -1245.439f);

        // the right ramp, the exit: its top at the doorway, its lower end above the spot in front of the barn
        public static readonly Vector3 RampTop = new(-2520.771f, -868.684f, -1239.249f);
        public static readonly Vector3 RampEnd = new(-2520.631f, -874.874f, -1229.969f);

        // anyone still on board when the ship leaves is moved just past the end of the ramp and glides down
        public static readonly Vector3 DropPoint = RampEnd + new Vector3(0f, 0.5f, 2f);
    }
}
