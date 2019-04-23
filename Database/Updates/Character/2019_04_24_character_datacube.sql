CREATE TABLE `character_datacube` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `type` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `datacube` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
    `progress` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `type`, `datacube`),
    CONSTRAINT `FK__character_datacube_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);
