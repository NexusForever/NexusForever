CREATE TABLE IF NOT EXISTS `character_zonemap_hexgroup` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `zoneMap` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
    `hexGroup` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `zoneMap`, `hexGroup`),
    CONSTRAINT `FK__character_zonemap_hexgroup_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);
