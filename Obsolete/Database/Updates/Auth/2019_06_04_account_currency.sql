-- Dumping structure for table nexus_forever_auth.account_currency
CREATE TABLE IF NOT EXISTS `account_currency` (
  `id` int(10) unsigned NOT NULL DEFAULT '0',
  `currencyId` tinyint(4) unsigned NOT NULL DEFAULT '0',
  `amount` bigint(20) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`currencyId`),
  CONSTRAINT `FK__account_currency_id__account_id` FOREIGN KEY (`id`) REFERENCES `account` (`id`) ON DELETE CASCADE
);