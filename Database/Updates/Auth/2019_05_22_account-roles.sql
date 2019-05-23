-- Dumping structure for table nexus_forever_auth.account_permission
CREATE TABLE IF NOT EXISTS `account_permission` (
  `id` int(10) unsigned NOT NULL DEFAULT '0',
  `permissionId` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`permissionId`),
  CONSTRAINT `FK__account_permission_id__account_id` FOREIGN KEY (`id`) REFERENCES `account` (`id`) ON DELETE CASCADE
);

-- Dumping structure for table nexus_forever_auth.account_role
CREATE TABLE IF NOT EXISTS `account_role` (
  `id` int(10) unsigned NOT NULL,
  `roleId` bigint(20) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`roleId`),
  KEY `FK__account_role_roleId__role_id` (`roleId`),
  CONSTRAINT `FK__account_role_id__account_id` FOREIGN KEY (`id`) REFERENCES `account` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK__account_role_roleId__role_id` FOREIGN KEY (`roleId`) REFERENCES `role` (`id`) ON DELETE CASCADE
);

-- Dumping structure for table nexus_forever_auth.role
CREATE TABLE IF NOT EXISTS `role` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `name` varchar(50) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
);
-- Dumping data for table nexus_forever_auth.role: ~0 rows (approximately)
INSERT INTO `role` (`id`, `name`) VALUES
	(1, 'Player'),
	(2, 'Game Master'),
	(3, 'Administrator'),
	(4, 'Sandbox Player');

-- Dumping structure for table nexus_forever_auth.role_permission
CREATE TABLE IF NOT EXISTS `role_permission` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `permissionId` bigint(20) NOT NULL,
  PRIMARY KEY (`id`,`permissionId`),
  CONSTRAINT `FK__role_permission_id__role_id` FOREIGN KEY (`id`) REFERENCES `role` (`id`) ON DELETE CASCADE
);
-- Dumping data for table nexus_forever_auth.role_permission: ~2 rows (approximately)
INSERT INTO `role_permission` (`id`, `permissionId`) VALUES
	(1, 29),
	(2, 3),
	(2, 100),
	(3, -1),
	(4, 4),
	(4, 5),
	(4, 6),
	(4, 7),
	(4, 8),
	(4, 9),
	(4, 10),
	(4, 11),
	(4, 12),
	(4, 13),
	(4, 14),
	(4, 15),
	(4, 16),
	(4, 17),
	(4, 18),
	(4, 19),
	(4, 20),
	(4, 21),
	(4, 22),
	(4, 23),
	(4, 24),
	(4, 25),
	(4, 26),
	(4, 27),
	(4, 28),
	(4, 29);