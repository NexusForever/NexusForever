using System.Numerics;
using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Game.Static.Housing;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Housing
{
    public interface IDecor : IDatabaseCharacter, IDatabaseState, INetworkBuildable<ServerHousingResidenceDecor.Decor>
    {
        ulong Id { get; }
        ulong DecorId { get; }
        HousingDecorInfoEntry Entry { get; }
        DecorType Type { get; set; }
        uint PlotIndex { get; set; }
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        float Scale { get; set; }
        ulong DecorParentId { get; set; }
        ushort ColourShiftId { get; set; }
        
        IResidence Residence { get; }

        /// <summary>
        /// Move <see cref="IDecor"/> to supplied position.
        /// </summary>
        void Move(DecorType type, Vector3 position, Quaternion rotation, float scale, uint plotIndex);

        /// <summary>
        /// Move <see cref="IDecor"/> to the crate.
        /// </summary>
        void Crate();
    }
}