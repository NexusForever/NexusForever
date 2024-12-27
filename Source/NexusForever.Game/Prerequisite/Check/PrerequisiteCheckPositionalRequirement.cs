using System.Numerics;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.PositionalRequirement)]
    public class PrerequisiteCheckPositionalRequirement : IPrerequisiteCheck
    {
        private readonly ILogger<PrerequisiteCheckPositionalRequirement> log;
        private readonly IGameTableManager gameTableManager;

        public PrerequisiteCheckPositionalRequirement(
            ILogger<PrerequisiteCheckPositionalRequirement> log,
            IGameTableManager gameTableManager)
        {
            this.log              = log;
            this.gameTableManager = gameTableManager;
        }

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            if (parameters.Target == null || objectId == 0)
                return false;

            PositionalRequirementEntry entry = gameTableManager.PositionalRequirement.GetEntry(objectId);
            if (entry == null)
                return false;

            float x = MathF.Cos(-parameters.Target.Rotation.X);
            float z = MathF.Sin(-parameters.Target.Rotation.X);
            Vector3 forward = new Vector3(x, 0, z);

            Vector3 direction = Vector3.Normalize(player.Position - parameters.Target.Position);
            float dot = Vector3.Dot(direction, forward);
            float cross = Vector3.Cross(direction, forward).Y;

            float angle = MathF.Atan2(cross, dot);
            angle += MathF.PI / 2;
            if (angle < 0)
                angle += MathF.PI * 2;

            angle = angle.ToDegrees();

            float minBounds = entry.AngleCenter - entry.AngleRange / 2f;
            float maxBounds = entry.AngleCenter + entry.AngleRange / 2f;
            bool isAllowed = angle >= minBounds && angle <= maxBounds;

            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return isAllowed;
                case PrerequisiteComparison.NotEqual:
                    return !isAllowed;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.PositionalRequirement}!");
                    return false;
            }
        }
    }
}
