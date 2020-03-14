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
                default:
                    return 0f;
            }
        }
    }
}
