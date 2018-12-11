CREATE TABLE IF NOT EXISTS `character_reputation` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `factionId` MEDIUMINT(8) UNSIGNED NOT NULL DEFAULT '0',
    `value` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `factionId`),
    CONSTRAINT `FK_character_faction_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);
