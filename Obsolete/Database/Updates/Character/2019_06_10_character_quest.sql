CREATE TABLE IF NOT EXISTS `character_quest` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `questId` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
    `state` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `flags` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `timer` INT(10) UNSIGNED NULL DEFAULT NULL,
    `reset` DATETIME NULL DEFAULT NULL,
    PRIMARY KEY (`id`, `questId`),
    CONSTRAINT `FK__character_quest_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS `character_quest_objective` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `questId` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
    `index` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `progress` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `timer` INT(10) UNSIGNED NULL DEFAULT NULL,
    PRIMARY KEY (`id`, `questId`, `index`),
    CONSTRAINT `FK__character_quest_objective_id__character_id` FOREIGN KEY (`id`, `questId`) REFERENCES `character_quest` (`id`, `questId`) ON DELETE CASCADE
);
