-- Dumping structure for table nexus_forever_character.character_path
CREATE TABLE IF NOT EXISTS `character_paths` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `pathName` varchar(50) NOT NULL DEFAULT '',
  `unlocked` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `totalXp` int(10) unsigned NOT NULL DEFAULT '0',
  `levelRewarded` tinyint(4) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`, `pathName`),
  CONSTRAINT `FK_character_path_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);

ALTER TABLE `character` 
ADD `activePath` int(10) unsigned NOT NULL DEFAULT '0',
ADD `pathActivatedTimestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP;