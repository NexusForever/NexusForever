namespace NexusForever.Game.Static.Combat.CrowdControl
{
    public enum CCStateApplyRulesResult
    {
        Ok                           = 0,
        InvalidCCState               = 1,
        NoTargetSpecified            = 2,
        TargetImmune                 = 3,
        TargetInfiniteInterruptArmor = 4,
        TargetInterruptArmorReduced  = 5,
        TargetInterruptArmorBlocked  = 6,
        StackingDoesNotStack         = 7,
        StackingShorterDuration      = 8,
        DiminishingReturnsTriggerCap = 9
    };
}
