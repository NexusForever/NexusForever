namespace NexusForever.Game.Static.Combat.CrowdControl
{
    public enum CCState
    {
        Stun                 = 0x00,
        Sleep                = 0x01,
        Root                 = 0x02,
        Disarm               = 0x03,
        Silence              = 0x04,
        Polymorph            = 0x05,
        Fear                 = 0x06,
        Hold                 = 0x07,
        Knockdown            = 0x08,
        Vulnerability        = 0x09,
        VulnerabilityWithAct = 0x0A,
        Disorient            = 0x0B,
        Disable              = 0x0C,
        Taunt                = 0x0D,
        DeTaunt              = 0x0E,
        Blind                = 0x0F,
        Knockback            = 0x10,
        Pushback             = 0x11,
        Pull                 = 0x12,
        PositionSwitch       = 0x13,
        Tether               = 0x14,
        Snare                = 0x15,
        Interrupt            = 0x16,
        Daze                 = 0x17,
        Subdue               = 0x18,
        Grounded             = 0x19,
        DisableCinematic     = 0x1A,
        AbilityRestriction   = 0x1B
    }
}
