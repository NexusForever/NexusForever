-- Dumping structure for table nexus_forever_character.character_mail
CREATE TABLE IF NOT EXISTS `character_mail` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `recipientId` bigint(20) unsigned NOT NULL DEFAULT '0',
  `senderType` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `senderId` bigint(20) unsigned NOT NULL DEFAULT '0',
  `subject` varchar(200) NOT NULL DEFAULT '',
  `message` varchar(2000) NOT NULL DEFAULT '',
  `textEntrySubject` int(10) unsigned NOT NULL DEFAULT '0',
  `textEntryMessage` int(10) unsigned NOT NULL DEFAULT '0',
  `creatureId` int(10) unsigned NOT NULL DEFAULT '0',
  `currencyType` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `currencyAmount` bigint(20) unsigned NOT NULL DEFAULT '0',
  `isCashOnDelivery` tinyint(8) unsigned NOT NULL DEFAULT '0',
  `hasPaidOrCollectedCurrency` tinyint(8) unsigned NOT NULL DEFAULT '0',
  `flags` tinyint(8) unsigned NOT NULL DEFAULT '0',
  `deliveryTime` tinyint(8) unsigned NOT NULL DEFAULT '0',
  `createTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `FK__character_mail_recipientId__character_id` (`recipientId`),
  CONSTRAINT `FK__character_mail_recipientId__character_id` FOREIGN KEY (`recipientId`) REFERENCES `character` (`id`) ON DELETE CASCADE
);
