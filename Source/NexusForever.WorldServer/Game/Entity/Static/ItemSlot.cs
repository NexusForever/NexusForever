namespace NexusForever.WorldServer.Game.Entity.Static
{
    public enum ItemSlot
    {
        [EquippedItem(EquippedItem.Chest)]
        ArmorChest                  = 1,

        [EquippedItem(EquippedItem.Legs)]
        ArmorLegs                   = 2,

        [EquippedItem(EquippedItem.Head)]
        ArmorHead                   = 3,

        [EquippedItem(EquippedItem.Shoulder)]
        ArmorShoulder               = 4,

        [EquippedItem(EquippedItem.Feet)]
        ArmorFeet                   = 5,

        [EquippedItem(EquippedItem.Hands)]
        ArmorHands                  = 6,

        [EquippedItem(EquippedItem.WeaponTool)]
        WeaponTool                  = 7,

        [EquippedItem(EquippedItem.WeaponPrimary)]
        WeaponPrimary               = 20,

        BodySkin                    = 24,
        BodyFace                    = 25,
        BodyEye                     = 26,
        BodyEar                     = 27,
        BodyHair                    = 28,
        BodyTatoo                   = 29,
        BodyTail                    = 30,
        BodyHand                    = 31,
        BodyFacialHair              = 39,

        [EquippedItem(EquippedItem.Shields)]
        ArmorShields                = 43,

        [EquippedItem(EquippedItem.Gadget)]
        ArmorGadget                 = 46,

        // TODO: bank bags
        [EquippedItem(EquippedItem.Bag0)]
        [EquippedItem(EquippedItem.Bag1)]
        [EquippedItem(EquippedItem.Bag2)]
        [EquippedItem(EquippedItem.Bag3)]
        Bag                         = 47,

        GuildStandardScanLines      = 52,
        GuildStandardTrim           = 53,
        GuildStandardBackgroundIcon = 54,
        GuildStandardForegroundIcon = 55,
        GuildStandardChest          = 56,

        [EquippedItem(EquippedItem.WeaponAttachment)]
        ArmorWeaponAttachment       = 57,

        [EquippedItem(EquippedItem.System)]
        ArmorSystem                 = 58,

        [EquippedItem(EquippedItem.Augment)]
        ArmorAugment                = 59,

        [EquippedItem(EquippedItem.Implant)]
        ArmorImplant                = 60,

        GuildStandardBack           = 61,
        GuildStandardShoulderL      = 62,
        GuildStandardShoulderR      = 63,
        EngineerMechSuit            = 64,
        Mount                       = 65,
        MountFront                  = 66,
        MountBack                   = 67,
        MountLeft                   = 68,
        MountRight                  = 69,
        BodyType                    = 70,
        WeaponArmCannon             = 71
    }
}
