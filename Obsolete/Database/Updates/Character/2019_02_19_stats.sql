CREATE TABLE `character_stats` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `stat` TINYINT UNSIGNED NOT NULL DEFAULT '0',
    `value` FLOAT NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `stat`),
    CONSTRAINT `FK__character_stats_stat_id_character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);
INSERT INTO `character_stats`(id,stat,value) SELECT `id`, 0, 800 FROM `character`;
INSERT INTO `character_stats`(id,stat,value) SELECT `id`, 20, 450 FROM `character`;
INSERT INTO `character_stats`(id,stat,value) SELECT `id`, 10, `level`   FROM `character`;
INSERT INTO `character_stats`(id,stat,value) SELECT `id`, 12, 3   FROM `character`;

## FIXME ## @Rawaho the field should eventually be dropped
##ALTER TABLE `character` DROP COLUMN `level`;
