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

-- Dumping structure for table nexus_forever_character.character_mail_attachment
CREATE TABLE IF NOT EXISTS `character_mail_attachment` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `index` int(10) unsigned NOT NULL DEFAULT '0',
  `itemGuid` bigint(20) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`index`),
  UNIQUE INDEX `itemGuid` (`itemGuid`),
  CONSTRAINT `FK__character_mail_attachment_id__character_mail_id` FOREIGN KEY (`id`) REFERENCES `character_mail` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK__character_mail_attachment_itemGuid__item_id` FOREIGN KEY (`itemGuid`) REFERENCES `item` (`id`) ON DELETE CASCADE
);

ALTER TABLE `item`
DROP FOREIGN KEY `FK__item_ownerId__character_id`;

-- Dumping structure for table nexus_forever_character.item
ALTER TABLE `item` 
  CHANGE COLUMN `ownerId` `ownerId` BIGINT(20) UNSIGNED NULL DEFAULT NULL AFTER `id`,
  ADD CONSTRAINT `FK__item_ownerId__character_id` FOREIGN KEY (`ownerId`) REFERENCES `character` (`id`) ON DELETE CASCADE ON UPDATE SET NULL;
