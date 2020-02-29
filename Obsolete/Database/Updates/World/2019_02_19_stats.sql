CREATE TABLE `entity_stats` (
    `id` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `stat` TINYINT UNSIGNED NOT NULL DEFAULT '0',
    `value` FLOAT NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `stat`),
    CONSTRAINT `FK__entity_stats_stat_id_entity_id` FOREIGN KEY (`id`) REFERENCES `entity` (`id`) ON DELETE CASCADE
);

INSERT IGNORE INTO `entity_stats`(id,stat,value) SELECT `id`,  0, 1000 FROM `entity`;
INSERT IGNORE INTO `entity_stats`(id,stat,value) SELECT `id`, 10, 15 FROM `entity`;
