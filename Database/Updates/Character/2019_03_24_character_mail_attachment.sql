-- Dumping structure for table nexus_forever_character.character_mail_attachment
CREATE TABLE IF NOT EXISTS `character_mail_attachment` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `index` int(10) unsigned NOT NULL DEFAULT '0',
  `itemId` int(10) unsigned NOT NULL DEFAULT '0',
  `amount` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`index`),
  CONSTRAINT `FK__character_mail_attachment_id__character_mail_id` FOREIGN KEY (`id`) REFERENCES `character_mail` (`id`) ON DELETE CASCADE
)