namespace NexusForever.AuthServer.Network.Message.Static
{
    public enum NpLoginResult
    {
        ErrorUnknown                     = 0,
        Success                          = 1,
        ErrorDb                          = 2,
        ErrorUnknownDBCallback           = 3,
        ErrorDuplicateLogin              = 4,
        ErrorCannotGetNpConnection       = 5,
        ErrorHandleClientLogin           = 6,
        ErrorInitializeNcPlatformLogin   = 7,
        ErrorNcPlatform                  = 8,
        ErrorInvalidData                 = 9,
        InvalidAuthenticationMethod      = 10,
        ErrorStartTimeout                = 11,
        ErrorFinishTimeout               = 12,
        ErrorNoRoles                     = 13,
        ErrorInvalidToken                = 16,
        ErrorNoDataCenterPermissions     = 17,
        NoRealmsAvailableAtThisTime      = 18,
        ClientServerVersionMismatch      = 19,
        ErrorAccountBanned               = 20,
        AccountSuspended                 = 21,
        ErrorGettingSubscription         = 22,
        ErrorEmailAddressMismatch        = 23,
        ErrorTwoStepVerificationRequired = 24,
        ErrorPaymentConfig               = 25,
        ErrorInvalidClientIP             = 26,
        ErrorRegionMismatch              = 27
    }
}
