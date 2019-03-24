CREATE TABLE IF NOT EXISTS `character_action_set_shortcut` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `specIndex` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `location` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
    `shortcutType` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `objectId` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `tier` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `specIndex`, `location`),
    CONSTRAINT `FK__character_action_set_shortcut_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS `character_action_set_amp` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `specIndex` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `ampId` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `specIndex`, `ampId`),
    CONSTRAINT `FK__character_action_set_amp_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);

ALTER TABLE `character` ADD `activeSpec` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0' AFTER `activeCostumeIndex`;

CREATE TABLE IF NOT EXISTS `character_spell` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `spell4BaseId` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `tier` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `spell4BaseId`),
    CONSTRAINT `FK__character_spell_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);
