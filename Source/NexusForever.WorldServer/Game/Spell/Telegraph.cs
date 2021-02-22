using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NexusForever.Shared;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Map.Search;
using NexusForever.WorldServer.Game.Spell.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Spell
{
    public class Telegraph
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public UnitEntity Caster { get; }
        public Vector3 Position { get; }
        public Vector3 Rotation { get; }
        public TelegraphDamageEntry TelegraphDamage { get; }

        public Telegraph(TelegraphDamageEntry telegraphDamageEntry, UnitEntity caster, Vector3 position, Vector3 rotation)
        {
            TelegraphDamage = telegraphDamageEntry;
            Caster          = caster;
            Position        = position;
            Rotation        = rotation;
        }

        /// <summary>
        /// Returns any <see cref="UnitEntity"/> inside the <see cref="Telegraph"/>.
        /// </summary>
        public IEnumerable<UnitEntity> GetTargets()
        {
            Caster.Map.Search(Position, GridSearchSize(), new SearchCheckTelegraph(this, Caster), out List<GridEntity> targets);
            return targets.Select(t => t as UnitEntity);
        }

        /// <summary>
        /// Returns whether the supplied <see cref="Vector3"/> is inside the telegraph.
        /// </summary>
        public bool InsideTelegraph(Vector3 position)
        {
            switch ((DamageShape)TelegraphDamage.DamageShapeEnum)
            {
                case DamageShape.Circle:
                    return Vector3.Distance(Position, position) < TelegraphDamage.Param00;
                case DamageShape.Cone:
                case DamageShape.LongCone:
                {
                    float angleRadian = Position.GetAngle(position);
                    angleRadian -= Rotation.X;
                    angleRadian = angleRadian.NormaliseRadians();

                    float angleDegrees = MathF.Abs(angleRadian.ToDegrees());
                    if (angleDegrees > TelegraphDamage.Param02 / 2f)
                        return false;

                    return Vector3.Distance(Position, position) < TelegraphDamage.Param01;
                }
                case DamageShape.Quadrilateral:
                {
                    float telegraphLength = TelegraphDamage.Param01;
                    float telegraphHeight = TelegraphDamage.Param02;

                    // TODO: If target is higher or lower than telegraph height, this should not hit. Confirm functionality.
                    if (position.Y >= Position.Y + telegraphHeight || position.Y <= Position.Y - telegraphHeight)
                        return false;

                    // TODO: Confirm whether it's necessary to add the model's hit box to the Z Offset (it probably is needed).
                    Vector3 startingPosition = Position.GetPoint2D(-Rotation.X - MathF.PI / 2, TelegraphDamage.ZPositionOffset);
                    startingPosition.Y += TelegraphDamage.YPositionOffset;

                    float leftAngle     = -Rotation.X + MathF.PI / 2;
                    Vector3 bottomLeft  = startingPosition.GetPoint2D(leftAngle, (telegraphLength / 2f) + TelegraphDamage.XPositionOffset);
                    Vector3 bottomRight = startingPosition.GetPoint2D(leftAngle + MathF.PI / 2, (telegraphLength / 2f) + TelegraphDamage.XPositionOffset);
                    Vector3 topRight    = bottomRight.GetPoint2D(-Rotation.X - MathF.PI / 4, telegraphLength);
                    Vector3 topLeft     = bottomLeft.GetPoint2D(-Rotation.X - MathF.PI / 4, telegraphLength);

                    // TODO: Add appropriate check that takes Hit Box from Creature2Model TBL into account with check
                    return IsPointInsidePolygon(new Vector2(position.X, position.Z), bottomLeft, bottomRight, topRight, topLeft);
                }
                case DamageShape.Rectangle:
                {
                    float telegraphWidth  = TelegraphDamage.Param00;
                    float telegraphHeight = TelegraphDamage.Param01;
                    float telegraphLength = TelegraphDamage.Param02;

                    // TODO: If target is higher or lower than telegraph height, this should not hit. Confirm functionality.
                    if (position.Y >= Position.Y + telegraphHeight || position.Y <= Position.Y - telegraphHeight)
                        return false;

                    // TODO: Confirm whether it's necessary to add the model's hit box to the Z Offset (it probably is needed).
                    Vector3 startingPosition = Position.GetPoint2D(-Rotation.X - MathF.PI / 2, TelegraphDamage.ZPositionOffset);
                    startingPosition.Y += TelegraphDamage.YPositionOffset;

                    float leftAngle     = -Rotation.X + MathF.PI;
                    Vector3 bottomLeft  = startingPosition.GetPoint2D(leftAngle, (telegraphWidth / 2f) + TelegraphDamage.XPositionOffset);
                    Vector3 bottomRight = startingPosition.GetPoint2D(leftAngle + MathF.PI, (telegraphWidth / 2f) + TelegraphDamage.XPositionOffset);
                    Vector3 topRight    = bottomRight.GetPoint2D(-Rotation.X - MathF.PI / 2, telegraphLength);
                    Vector3 topLeft     = bottomLeft.GetPoint2D(-Rotation.X - MathF.PI / 2, telegraphLength);

                    // TODO: Add appropriate check that takes Hit Box from Creature2Model TBL into account with check
                    return IsPointInsidePolygon(new Vector2(position.X, position.Z), bottomLeft, bottomRight, topRight, topLeft);
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
                case DamageShape.Quadrilateral:
                case DamageShape.Rectangle:
                    return TelegraphDamage.Param01;
                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Returns a boolean whether a point sits in a horizontal plane of points.
        /// </summary>
        /// <remarks>
        /// Based on code available in https://github.com/substack/point-in-polygon/ and the algorithm from https://wrf.ecse.rpi.edu//Research/Short_Notes/pnpoly.html/
        /// </remarks>
        private static bool IsPointInsidePolygon(Vector2 point, params Vector3[] polygon)
        {
            bool isInside = false;
            for (int i = 0, j = polygon.Length- 1; i < polygon.Length; j = i++)
            {
                float xi = polygon[i].X;
                float yi = polygon[i].Z;
                float xj = polygon[j].X;
                float yj = polygon[j].Z;

                bool intersect = ((yi > point.Y) != (yj > point.Y))
                    && (point.X < (xj - xi) * (point.Y - yi) / (yj - yi) + xi);

                if (intersect)
                    isInside = !isInside;
            }

            return isInside;
        }
    }
}
