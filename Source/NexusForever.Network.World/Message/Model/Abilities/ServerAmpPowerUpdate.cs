using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    [Message(GameMessageOpcode.ServerAmpPowerUpdate)]
    public class ServerAmpPowerUpdate : IWritable
    {
        /// <summary>
        /// AMP points granted by <c>AMP Power Upgrade</c> items, 0..10.
        /// </summary>
        /// <remarks>
        /// The client sign extends this field into its bonus power slot and recomputes the AMP point
        /// total from it, so it is signed here; but the <c>&lt;10</c> cap of <c>Prerequisite</c> 28557
        /// compares the slot unsigned, so a negative value sign-extends to <c>0xFFFFFFFF</c>, which
        /// fails the unsigned <c>&lt; 10</c> gate and locks further grants out. Never send one.
        /// </remarks>
        public short BonusPower { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write((ushort)BonusPower);
        }
    }
}
