CREATE TABLE IF NOT EXISTS `character_currency` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `currencyId` TINYINT(4) UNSIGNED NOT NULL DEFAULT '0',
    `amount` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `currencyId`),
    CONSTRAINT `FK_character_currency_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);
