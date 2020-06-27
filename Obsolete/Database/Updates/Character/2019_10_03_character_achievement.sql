CREATE TABLE IF NOT EXISTS `character_achievement` (
	`id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
	`achievementId` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
	`data0` INT(10) UNSIGNED NOT NULL DEFAULT '0',
	`data1` INT(10) UNSIGNED NOT NULL DEFAULT '0',
	`dateCompleted` DATETIME NULL DEFAULT NULL,
	PRIMARY KEY (`id`, `achievementId`),
	CONSTRAINT `FK__character_achievement_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);
