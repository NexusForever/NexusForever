CREATE TABLE IF NOT EXISTS `character_title` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `title` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`title`),
  CONSTRAINT `FK__character_title_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

ALTER TABLE `character` ADD `title` bigint(20) unsigned NOT NULL DEFAULT '0';
