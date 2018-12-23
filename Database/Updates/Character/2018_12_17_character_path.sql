-- Dumping structure for table nexus_forever_character.character_path
CREATE TABLE IF NOT EXISTS `character_path` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `activePath` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `pathsUnlocked` smallint(5) unsigned NOT NULL DEFAULT '0',
  `soldierXp` int(10) unsigned NOT NULL DEFAULT '0',
  `settlerXp` int(10) unsigned NOT NULL DEFAULT '0',
  `scientistXp` int(10) unsigned NOT NULL DEFAULT '0',
  `explorerXp` int(10) unsigned NOT NULL DEFAULT '0',
  `soldierLevelRewarded` int(10) unsigned NOT NULL DEFAULT '0',
  `settlerLevelRewarded` int(10) unsigned NOT NULL DEFAULT '0',
  `scientistLevelRewarded` int(10) unsigned NOT NULL DEFAULT '0',
  `explorerLevelRewarded` int(10) unsigned NOT NULL DEFAULT '0',
  `pathActivatedTimestamp` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  CONSTRAINT `FK_character_path_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
)