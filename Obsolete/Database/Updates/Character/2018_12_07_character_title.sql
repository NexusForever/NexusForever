CREATE TABLE `character_title` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `title` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
    `timeRemaining` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `revoked` TINYINT(4) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `title`),
    CONSTRAINT `FK__character_title_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);

ALTER TABLE `character` ADD `title` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0';
