CREATE TABLE `character_actions` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `specIndex` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `location` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
    `action` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `tierIndex` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `specIndex`, `location`),
    CONSTRAINT `FK__character_actions_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);



CREATE TABLE `character_amps` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `specIndex` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `ampId` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `specIndex`, `ampId`),
    CONSTRAINT `FK__character_amps_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);

ALTER TABLE `character` ADD `activeSpec` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0' AFTER `activeCostumeIndex`;
