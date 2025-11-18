namespace NexusForever.Game.Static.Combat
{
    public enum CCStateApplyRulesResult
    {
        Ok                            = 0,
        InvalidCCState                = 1,
        NoTargetSpecified             = 2,
        Target_Immune                 = 3,
        Target_InfiniteInterruptArmor = 4,
        Target_InterruptArmorReduced  = 5,
        Target_InterruptArmorBlocked  = 6,
        Stacking_DoesNotStack         = 7,
        Stacking_ShorterDuration      = 8,
        DiminishingReturns_TriggerCap = 9,
    };
}
