namespace NexusForever.GameTable.Model
{
    public class UnitVehicleEntry
    {
        public uint Id { get; set; }
        public uint VehicleTypeEnum { get; set; }
        public uint NumberPilots { get; set; }
        public uint PilotPosture00 { get; set; }
        public uint PilotPosture01 { get; set; }
        public uint NumberPassengers { get; set; }
        public uint PassengerPosture00 { get; set; }
        public uint PassengerPosture01 { get; set; }
        public uint PassengerPosture02 { get; set; }
        public uint PassengerPosture03 { get; set; }
        public uint PassengerPosture04 { get; set; }
        public uint PassengerPosture05 { get; set; }
        public uint NumberGunners { get; set; }
        public uint GunnerPosture00 { get; set; }
        public uint GunnerPosture01 { get; set; }
        public uint GunnerPosture02 { get; set; }
        public uint GunnerPosture03 { get; set; }
        public uint GunnerPosture04 { get; set; }
        public uint GunnerPosture05 { get; set; }
        public uint VendorItemPrice { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdTooltip { get; set; }
        public string ButtonIcon { get; set; }
        public uint Flags { get; set; }
        public uint SoundEventIdTakeoff { get; set; }
        public uint SoundEventIdLanding { get; set; }
        public uint WaterSurfaceEffectIdMoving { get; set; }
        public uint WaterSurfaceEffectIdStanding { get; set; }
        public uint WaterSurfaceEffectIdJumpIn { get; set; }
        public uint WaterSurfaceEffectIdJumpOut { get; set; }
    }
}
