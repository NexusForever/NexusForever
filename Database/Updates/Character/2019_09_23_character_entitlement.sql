CREATE TABLE IF NOT EXISTS `character_entitlement` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `entitlementId` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `amount` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `entitlementId`),
    CONSTRAINT `FK__character_entitlement_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
)
