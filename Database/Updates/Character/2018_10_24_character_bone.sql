CREATE TABLE IF NOT EXISTS `character_bone` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `boneIndex` TINYINT(4) UNSIGNED NOT NULL DEFAULT '0',
    `bone` FLOAT NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `boneIndex`),
    CONSTRAINT `FK_character_bone_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);
