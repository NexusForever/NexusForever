namespace NexusForever.WorldServer.Game.Storefront.Static
{
    public enum StoreError
    {
        CatalogUnavailable,
        StoreDisabled,
        InvalidOffer,
        InvalidPrice,
        GenericFail,
        PurchasePending,
        PgWs_CartFraudFailure,
        PgWs_CartPaymentFailure,
        PgWs_InvalidCCExpirationDate,
        PgWs_InvalidCreditCardNumber,
        PgWs_CreditCardExpired,
        PgWs_CreditCardDeclined,
        PgWs_CreditFloorExceeded,
        PgWs_InventoryStatusFailure,
        PgWs_PaymentPostAuthFailure,
        PgWs_SubmitCartFailed,
        PurchaseVelocityLimit,
        MissingItemEntitlement,
        IneligibleGiftRecipient,
        CannotUseOffer,
        MissingEntitlement,
        CannotGiftOffer,
        Success = 22
    }
}
