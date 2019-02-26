CREATE TABLE IF NOT EXISTS `character_costume_item` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `index` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `slot` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `itemId` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `dyeData` INT(10) NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `index`, `slot`),
    CONSTRAINT `FK__character_costume_item_id-index__character_costume_id-index` FOREIGN KEY (`id`, `index`) REFERENCES `character_costume` (`id`, `index`) ON DELETE CASCADE
);
