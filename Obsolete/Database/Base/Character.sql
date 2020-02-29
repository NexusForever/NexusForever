-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               5.7.21-log - MySQL Community Server (GPL)
-- Server OS:                    Win64
-- HeidiSQL Version:             9.5.0.5278
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Dumping structure for table nexus_forever_character.character
CREATE TABLE IF NOT EXISTS `character` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `accountId` int(10) unsigned NOT NULL DEFAULT '0',
  `name` varchar(50) NOT NULL DEFAULT '',
  `sex` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `race` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `class` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `level` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `createTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `accountId` (`accountId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.
-- Dumping structure for table nexus_forever_character.character_appearance
CREATE TABLE IF NOT EXISTS `character_appearance` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `slot` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `displayId` smallint(5) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`slot`),
  CONSTRAINT `FK__character_appearance_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.
-- Dumping structure for table nexus_forever_character.character_customisation
CREATE TABLE IF NOT EXISTS `character_customisation` (
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  `label` int(10) unsigned NOT NULL DEFAULT '0',
  `value` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`label`),
  CONSTRAINT `FK__character_customisation_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
