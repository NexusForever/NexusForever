using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Housing
{
    public interface IResidenceEntrance
    {
        WorldEntry Entry { get; }
        Vector3 Position { get; }
        Quaternion Rotation { get; }
    }
}