CREATE TABLE IF NOT EXISTS `character_pet_customisation` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `type` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `objectId` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `name` VARCHAR(128) NOT NULL DEFAULT '',
    `flairIdMask` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `type`, `objectId`),
    CONSTRAINT `FK__character_pet_customisation_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);

