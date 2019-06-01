CREATE TABLE `residence_plot` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `index` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `plotInfoId` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
    `plugItemId` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
    `plugFacing` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `buildState` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `index`),
    CONSTRAINT `FK__residence_plot_id__residence_id` FOREIGN KEY (`id`) REFERENCES `residence` (`id`) ON DELETE CASCADE
);
