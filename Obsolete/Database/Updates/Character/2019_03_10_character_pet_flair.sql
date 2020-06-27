CREATE TABLE IF NOT EXISTS `character_pet_flair` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `petFlairId` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `petFlairId`),
    CONSTRAINT `FK__character_pet_flair_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);
