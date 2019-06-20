using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NexusForever.Shared;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Map;
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
                case DamageShape.Cone:
                {
                    float angleRadian = Position.GetAngle(position);
                    angleRadian -= Rotation.X;
                    angleRadian = angleRadian.NormaliseRadians();

                    float angleDegrees = MathF.Abs(angleRadian.ToDegrees());
                    if (angleDegrees > TelegraphDamage.Param02 / 2f)
                        return false;

                    return Vector3.Distance(Position, position) < TelegraphDamage.Param01;
                }
                case DamageShape.Rectangle:
                {
                        float Width = TelegraphDamage.Param00;
                        float Length = TelegraphDamage.Param01;
                        float Height = TelegraphDamage.Param02;

                        //Find the point offsets in location to the player
                        var bottomRight = new Vector3(-Width, 0, 0);
                        var bottomLeft = new Vector3(Width, 0, 0);
                        var topRight = new Vector3(-Width, 0, Length);
                        var topLeft = new Vector3(Width, 0, Length);

                        //Translate the points back to the global cordinate system and rotate them the same
                        //way the player if facing
                        bottomLeft = Vector3.Add(RotatePoint(bottomLeft, Rotation.X), Position);
                        bottomRight = Vector3.Add(RotatePoint(bottomRight, Rotation.X), Position);
                        topLeft = Vector3.Add(RotatePoint(topLeft, Rotation.X), Position);
                        topRight = Vector3.Add(RotatePoint(topRight, Rotation.X), Position);


                        //Create a polygon to test with the rotated points
                        List<Vector2> RotatedRectange = new List<Vector2>(){
                            new Vector2(bottomLeft.X, bottomLeft.Z),
                            new Vector2(bottomRight.X, bottomRight.Z),
                            new Vector2(topLeft.X, topLeft.Z),
                            new Vector2(topRight.X, topRight.Z)
                        };

                        return IsPointInPolygon(RotatedRectange.ToArray(), new Vector2(position.X, position.Z)) && position.Y <= Position.Y + Height && Position.Y - Height <= position.Y;
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
                case DamageShape.Cone:
                    return TelegraphDamage.Param01;
                case DamageShape.Rectangle:
                    return TelegraphDamage.Param01 / 2f;
                default:
                    return 0f;
            }
        }

        public static bool IsPointInPolygon(Vector2[] polygon, Vector2 testPoint)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        private Vector3 RotatePoint(Vector3 Point, float Rotation)
        {
            var newX = ((Point.X) * MathF.Cos(Rotation) + (Point.Z) * MathF.Sin(Rotation));
            var newY = ((-Point.X) * MathF.Sin(Rotation) + (Point.Z) * MathF.Cos(Rotation));

            newX = -newX;
            newY = -newY;

            return new Vector3(newX, Point.Y, newY);
        }
    }
}
