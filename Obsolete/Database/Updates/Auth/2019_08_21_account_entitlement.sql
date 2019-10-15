CREATE TABLE IF NOT EXISTS `account_entitlement` (
    `id` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `entitlementId` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `amount` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `entitlementId`),
    CONSTRAINT `FK__account_entitlement_id__account_id` FOREIGN KEY (`id`) REFERENCES `account` (`id`) ON DELETE CASCADE
);