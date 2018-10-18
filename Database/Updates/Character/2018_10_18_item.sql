CREATE TABLE IF NOT EXISTS `item` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `ownerId` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `itemId` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `location` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
    `bagIndex` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `stackCount` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `charges` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `durability` FLOAT NOT NULL DEFAULT '0',
    `expirationTimeLeft` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`),
    INDEX `FK__item_ownerId__character_id` (`ownerId`),
    CONSTRAINT `FK__item_ownerId__character_id` FOREIGN KEY (`ownerId`) REFERENCES `character` (`id`) ON DELETE CASCADE
);
