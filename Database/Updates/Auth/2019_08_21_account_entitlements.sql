-- Dumping structure for table ws_auth.account_entitlements
CREATE TABLE IF NOT EXISTS `account_entitlements` (
  `id` int(10) unsigned NOT NULL DEFAULT '0',
  `entitlementId` int(11) unsigned NOT NULL DEFAULT '0',
  `amount` int(11) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`entitlementId`),
  CONSTRAINT `FK__account_entitlements_id__account_id` FOREIGN KEY (`id`) REFERENCES `account` (`id`) ON DELETE CASCADE
);