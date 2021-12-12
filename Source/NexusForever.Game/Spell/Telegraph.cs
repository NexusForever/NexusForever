using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Search;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Entity;
using NexusForever.Game.Map.Search;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;
using NLog;

namespace NexusForever.Game.Spell
{
    public class Telegraph : ITelegraph
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public IUnitEntity Caster { get; }
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }
        public TelegraphDamageEntry TelegraphDamage { get; }
        public TelegraphTargetTypeFlags TelegraphTargetTypeFlags => (TelegraphTargetTypeFlags)TelegraphDamage.TargetTypeFlags;

        private float casterHitRadius => Caster.HitRadius * 0.5f;

        public Telegraph(TelegraphDamageEntry telegraphDamageEntry, IUnitEntity caster, Vector3 position, Vector3 rotation)
        {
            TelegraphDamage = telegraphDamageEntry;
            Caster          = caster;
            Position        = position;
            Rotation        = rotation;

            ApplyPositionOffsets();
            ApplyRotationOffsets();
        }

        private void ApplyPositionOffsets()
        {
            if (TelegraphDamage.ZPositionOffset == 0u
                && TelegraphDamage.XPositionOffset == 0u
                && TelegraphDamage.YPositionOffset == 0u)
                return;

            Vector3 startingPosition = Position;
            if (TelegraphDamage.ZPositionOffset != 0u)
                startingPosition = Position.GetPointForTelegraph(Rotation.X + MathF.PI / 2, TelegraphDamage.ZPositionOffset);

            startingPosition.Y += TelegraphDamage.YPositionOffset;

            Position = startingPosition.GetPointForTelegraph(Rotation.X + MathF.PI, TelegraphDamage.XPositionOffset);
        }

        private void ApplyRotationOffsets()
        {
            if (TelegraphDamage.RotationDegrees == 0u)
                return;

            float rotationRadians = Rotation.X + TelegraphDamage.RotationDegrees.ToRadians();

            if (rotationRadians > MathF.PI)
                rotationRadians -= 2 * MathF.PI;
            else if (rotationRadians <= -MathF.PI)
                rotationRadians += 2 * MathF.PI;

            Rotation = new Vector3(rotationRadians, Rotation.Y, Rotation.Z);
        }

        private void FilterTargets(IEnumerable<ISpellTargetInfo> targets, ISearchCheck check, out List<ISpellTargetInfo> filteredTargets)
        {
            filteredTargets = new();
            filteredTargets = targets.Where(x => check.CheckEntity(x.Entity)).ToList();
        }

        /// <summary>
        /// Returns a <see cref="IEnumerable{T}"/> containing all <see cref="ISpellTargetInfo"/> that can be targeted by this <see cref="ITelegraph"/>.
        /// </summary>
        public IEnumerable<ISpellTargetInfo> GetTargets(ISpell spell, List<ISpellTargetInfo> targets)
        {
            FilterTargets(targets, new SearchCheckTelegraph(this, Caster), out targets);

            foreach (var target in targets.ToList())
                if (!(EvaluateDamageFlagsForTarget(target.Entity, spell)))
                    targets.Remove(target);

            return targets;
        }

        private bool EvaluateDamageFlagsForTarget(IGridEntity target, ISpell spell)
        {
            TelegraphDamageFlag damageFlag = (TelegraphDamageFlag)TelegraphDamage.TelegraphDamageFlags;

            // This is Invalid
            //if (damageFlag.HasFlag(TelegraphDamageFlag.SpellMustBeMultiPhase))
            //    if (spell.CastMethod != CastMethod.Multiphase)
            //        return false;

            if (damageFlag.HasFlag(TelegraphDamageFlag.CasterMustBeNPC))
                if (Caster is IPlayer)
                    return false;

            if (damageFlag.HasFlag(TelegraphDamageFlag.CasterMustBePlayer))
                if (Caster is not IPlayer)
                    return false;

            if (damageFlag.HasFlag(TelegraphDamageFlag.TargetMustBeUnit))
                if (target is not IUnitEntity)
                    return false;

            return true;
        }

        /// <summary>
        /// Returns whether the supplied <see cref="Vector3"/> is inside the telegraph.
        /// </summary>
        public bool InsideTelegraph(Vector3 position, float hitRadius)
        {
            hitRadius *= 0.5f;

            switch ((DamageShape)TelegraphDamage.DamageShapeEnum)
            {
                case DamageShape.Circle:
                {
                    float telegraphRange = TelegraphDamage.Param00;
                    return Vector2.Distance(new Vector2(Position.X, Position.Z), new Vector2(position.X, position.Z)) <= telegraphRange + hitRadius;
                }
                case DamageShape.Cone:
                case DamageShape.LongCone:
                {
                    float telegraphStart = TelegraphDamage.Param00 - casterHitRadius;
                    float telegraphRadius = TelegraphDamage.Param01 + casterHitRadius;
                    float telegraphAngleInDegrees = TelegraphDamage.Param02 + (TelegraphDamage.Param00 / 2f);

                    return IsHitInsideAngle(position, hitRadius, telegraphStart, telegraphRadius, telegraphAngleInDegrees);
                }
                case DamageShape.Square:
                case DamageShape.Rectangle:
                {
                    /// <remarks>This is not perfect. It works, but there are more efficient options like https://yal.cc/rot-rect-vs-circle-intersection </remarks>
                    float telegraphWidth  = TelegraphDamage.Param00;
                    float telegraphHeight = TelegraphDamage.Param01;
                    float telegraphLength = TelegraphDamage.Param02;

                    // TODO: If target is higher or lower than telegraph height, this should not hit. Confirm functionality.
                    if (position.Y >= Position.Y + telegraphHeight || position.Y <= Position.Y - telegraphHeight)
                        return false;

                    // Find the points in the local co-ordinate space
                    var bottomRight = new Vector3(-telegraphWidth, 0, -telegraphLength);
                    var bottomLeft  = new Vector3(telegraphWidth, 0, -telegraphLength);
                    var topRight    = new Vector3(-telegraphWidth, 0, telegraphLength);
                    var topLeft     = new Vector3(telegraphWidth, 0, telegraphLength);

                    // Rectangle's origin is at it's base, so adjust the starting points appropriately.
                    if ((DamageShape)TelegraphDamage.DamageShapeEnum == DamageShape.Rectangle)
                    {
                        bottomRight.Z = 0;
                        bottomLeft.Z  = 0;
                    }

                    // Translate points to global co-ordinate space, and apply rotation based on the Telegraph's rotation
                    bottomLeft  = Vector3.Add(RotatePoint(bottomLeft, Rotation.X), Position);
                    bottomRight = Vector3.Add(RotatePoint(bottomRight, Rotation.X), Position);
                    topLeft     = Vector3.Add(RotatePoint(topLeft, Rotation.X), Position);
                    topRight    = Vector3.Add(RotatePoint(topRight, Rotation.X), Position);

                    Vector3[] points = new Vector3[] { bottomLeft, topLeft, topRight, bottomRight };
                    return IsHitInsidePolygon(points, position, hitRadius);
                }
                case DamageShape.Pie:
                {
                    float telegraphRadius         = TelegraphDamage.Param01;
                    float telegraphAngleInDegrees = TelegraphDamage.Param02;

                    float closestDistance = Position.GetDistance(position) - hitRadius;
                    if (closestDistance > telegraphRadius)
                        return false;

                    return !IsHitInsideAngle(position, hitRadius, 0f, telegraphRadius, telegraphAngleInDegrees);
                }
                default:
                    log.Warn($"Unhandled telegraph shape {(DamageShape)TelegraphDamage.DamageShapeEnum}.");
                    return false;
            }
        }

        private float GridSearchSize()
        {
            switch ((DamageShape)TelegraphDamage.DamageShapeEnum)
            {
                case DamageShape.Circle:
                    return TelegraphDamage.Param00;
                case DamageShape.Cone:
                case DamageShape.LongCone:
                    return TelegraphDamage.Param01;
                case DamageShape.Square:
                case DamageShape.Rectangle:
                    return TelegraphDamage.Param01;
                default:
                    return 0f;
            }
        }

        private static bool IsHitInsidePolygon(Vector3[] polygon, Vector3 targetPosition, float hitRadius)
        {
            if (IsPointInsidePolygon(polygon, targetPosition))
                return true;

            if (IsRadiusInsidePolygon(polygon, targetPosition, hitRadius))
                return true;

            return false;
        }

        private bool IsHitInsideAngle(Vector3 targetPosition, float hitRadius, float startRadius, float telegraphRadius, float angle)
        {
            float distance = Position.GetDistance(targetPosition);

            if (distance - hitRadius > telegraphRadius)
                return false;

            if (distance + hitRadius < startRadius)
                return false;

            float angleRadian = (Position.GetAngle(targetPosition) - Rotation.X).CondenseRadianIntoRotationRadian();

            float angleDegrees = MathF.Abs(angleRadian.NormaliseRadians().ToDegrees());
            if (angleDegrees > angle / 2f || angleDegrees < -angle / 2f)
            {
                // Checks for edge radius is skipped if the caster is not a Player. This optimises this method, but also allows for player's to dodge attacks appropriately.
                if (Caster is not IPlayer)
                    return false;

                if (angleDegrees > angle || angleDegrees < -angle)
                    return false;

                if (angleRadian.ToDegrees() < 0f)
                {
                    Vector3 rightEdge;
                    if (startRadius > 0f)
                    {
                        Vector3 rightStartPosition = Position.GetPointForTelegraph(((angle / 2f) * -1f).ToRadians() + Rotation.X + MathF.PI / 2, startRadius);
                        rightEdge = rightStartPosition.GetPointForTelegraph(((angle / 2f) * -1f).ToRadians() + Rotation.X + MathF.PI / 2, telegraphRadius);
                    }
                    else
                        rightEdge = Position.GetPointForTelegraph(((angle / 2f) * -1f).ToRadians() + Rotation.X + MathF.PI / 2, telegraphRadius);

                    if (IsRadiusIntersectingLine(Position, rightEdge, targetPosition, hitRadius))
                        return true;
                }
                else
                {
                    Vector3 leftEdge;
                    if (startRadius > 0f)
                    {
                        Vector3 leftStartPosition = Position.GetPointForTelegraph((angle / 2f).ToRadians() + Rotation.X + MathF.PI / 2, startRadius);
                        leftEdge = leftStartPosition.GetPointForTelegraph((angle / 2f).ToRadians() + Rotation.X + MathF.PI / 2, telegraphRadius);
                    }
                    else
                        leftEdge = Position.GetPointForTelegraph((angle / 2f).ToRadians() + Rotation.X + MathF.PI / 2, telegraphRadius);

                    if (IsRadiusIntersectingLine(Position, leftEdge, targetPosition, hitRadius))
                        return true;
                }

                return false;
            }

            return true;
        }

        #region Math Stuff

        /// <remarks>
        /// Based on code available in http://www.jeffreythompson.org/collision-detection/poly-circle.php
        /// </remarks>
        private static bool IsPointInsidePolygon(Vector3[] polygon, Vector3 position)
        {
            bool isInside = false;

            // Go through each of the vertices, plus the next vertex in the list
            for (int current = 0; current < polygon.Length; current++)
            {
                // Get next vertex in list. If we've hit the end, wrap around to first
                int next = current + 1;
                if (next == polygon.Length)
                    next = 0;

                Vector3 vc = polygon[current];
                Vector3 vn = polygon[next];

                if (((vc.Z > position.Z && vn.Z < position.Z) || (vc.Z < position.Z && vn.Z > position.Z)) &&
                    (position.X < (vn.X - vc.X) * (position.Z - vc.Z) / (vn.Z - vc.Z) + vc.X))
                    isInside = !isInside;
            }

            return isInside;
        }

        private static bool IsRadiusInsidePolygon(Vector3[] polygon, Vector3 position, float hitRadius)
        {
            // go through each of the vertices, plus
            // the next vertex in the list
            for (int current = 0; current < polygon.Length; current++)
            {
                // get next vertex in list
                // if we've hit the end, wrap around to 0
                int next = current + 1;
                if (next == polygon.Length)
                    next = 0;

                // get the PVectors at our current position
                // this makes our if statement a little cleaner
                Vector3 vc = polygon[current];    // c for "current"
                Vector3 vn = polygon[next];       // n for "next"

                // check for collision between the circle and
                // a line formed between the two vertices
                if (IsRadiusIntersectingLine(vc, vn, position, hitRadius)) 
                    return true;
            }

            return false;
        }

        private static bool IsRadiusIntersectingLine(Vector3 lineStart, Vector3 lineEnd, Vector3 circleOrigin, float radius)
        {
            // is either end INSIDE the circle?
            // if so, return true immediately
            bool inside1 = lineStart.GetDistance(circleOrigin) < radius;
            bool inside2 = lineEnd.GetDistance(circleOrigin) < radius;
            if (inside1 || inside2)
                return true;

            Vector2 closestPoint = GetClosestPoint(new Vector2(lineStart.X, lineStart.Z), new Vector2(lineEnd.X, lineEnd.Z), new Vector2(circleOrigin.X, circleOrigin.Z));

            // is this point actually on the line segment?
            // if so keep going, but if not, return false
            if (!IsPointIntersectingLine(lineStart.X, lineStart.Z, lineEnd.X, lineEnd.Z, closestPoint.X, closestPoint.Y))
                return false;

            // get distance to closest point
            float distX    = closestPoint.X - circleOrigin.X;
            float distZ    = closestPoint.Y - circleOrigin.Z;
            float distance = MathF.Sqrt((distX * distX) + (distZ * distZ));

            // is the circle on the line?
            if (distance <= radius)
                return true;

            return false;
        }

        private static Vector2 GetClosestPoint(Vector2 startPoint, Vector2 endPoint, Vector2 circleOrigin)
        {
            // get length of the line
            float distX = startPoint.X - endPoint.X;
            float distZ = startPoint.Y - endPoint.Y;
            float len   = MathF.Sqrt((distX * distX) + (distZ * distZ));

            // get dot product of the line and circle
            float dot = (((circleOrigin.X - startPoint.X) * (endPoint.X - startPoint.X)) + ((circleOrigin.Y - startPoint.Y) * (endPoint.Y - startPoint.Y))) / MathF.Pow(len, 2);
            // float dot = (((cx - x1) * (x2 - x1)) + ((cy - y1) * (y2 - y1))) / MathF.Pow(len, 2);

            // find the closest point on the line
            float closestX = startPoint.X + (dot * (endPoint.X - startPoint.X));
            float closestY = startPoint.Y + (dot * (endPoint.Y - startPoint.Y));

            return new Vector2(closestX, closestY);
        }

        private static bool IsPointIntersectingLine(float x1, float y1, float x2, float y2, float px, float py)
        {
            // get distance from the point to the two ends of the line
            float d1 = Vector2.Distance(new Vector2(px, py), new Vector2(x1, y1));
            float d2 = Vector2.Distance(new Vector2(px, py), new Vector2(x2, y2));

            // get the length of the line
            float lineLen = Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2));

            // since floats are so minutely accurate, add
            // a little buffer zone that will give collision
            float buffer = 0.1f;    // higher # = less accurate

            // if the two distances are equal to the line's
            // length, the point is on the line!
            // note we use the buffer here to give a range, rather
            // than one #
            if (d1 + d2 >= lineLen - buffer && d1 + d2 <= lineLen + buffer)
                return true;

            return false;
        }

        private static Vector3 RotatePoint(Vector3 point, float rotation)
        {
            var newX = ((point.X) * MathF.Cos(rotation) + (point.Z) * MathF.Sin(rotation));
            var newY = ((-point.X) * MathF.Sin(rotation) + (point.Z) * MathF.Cos(rotation));

            newX = -newX;
            newY = -newY;

            return new Vector3(newX, point.Y, newY);
        }

        #endregion
    }
}
