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

-- Dumping structure for table nexus_forever_world.entity
CREATE TABLE IF NOT EXISTS `entity` (
  `id` int(10) unsigned NOT NULL DEFAULT '0',
  `type` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `creature` int(10) unsigned NOT NULL DEFAULT '0',
  `world` smallint(5) unsigned NOT NULL DEFAULT '0',
  `x` float NOT NULL DEFAULT '0',
  `y` float NOT NULL DEFAULT '0',
  `z` float NOT NULL DEFAULT '0',
  `rx` float NOT NULL DEFAULT '0',
  `ry` float NOT NULL DEFAULT '0',
  `rz` float NOT NULL DEFAULT '0',
  `displayInfo` int(10) unsigned NOT NULL DEFAULT '0',
  `outfitInfo` smallint(5) unsigned NOT NULL DEFAULT '0',
  `faction1` smallint(5) unsigned NOT NULL DEFAULT '0',
  `faction2` smallint(5) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Dumping data for table nexus_forever_world.entity: ~34 rows (approximately)
/*!40000 ALTER TABLE `entity` DISABLE KEYS */;
INSERT INTO `entity` (`id`, `type`, `creature`, `world`, `x`, `y`, `z`, `rx`, `ry`, `rz`, `displayInfo`, `outfitInfo`, `faction1`, `faction2`) VALUES
	(262277, 0, 73334, 870, -7700.06, -940.929, -667.332, -2.56057, 0, 0, 21339, 0, 255, 255),
	(262278, 0, 36464, 870, -7697.02, -940.711, -650.236, -1.88312, 0, 0, 35188, 9154, 170, 170),
	(262289, 0, 24187, 870, -7664.51, -942.745, -672.721, 0.839049, 0, 0, 25202, 9079, 170, 170),
	(262291, 0, 24158, 870, -7666.19, -942.697, -678.087, -3.02895, 0, 0, 25194, 9076, 170, 170),
	(262301, 0, 33538, 870, -7623.78, -950.75, -668.586, -3.14159, 0, 0, 28247, 0, 174, 174),
	(262302, 0, 33538, 870, -7568.85, -963.021, -680.979, -3.14159, 0, 0, 28247, 0, 174, 174),
	(262303, 0, 33538, 870, -7592.03, -955.337, -658.009, -3.14159, 0, 0, 28247, 0, 174, 174),
	(262325, 0, 24244, 870, -7513.56, -976.798, -804.925, -1.08664, 0, 0, 26049, 0, 242, 242),
	(262474, 0, 24054, 870, -7583.89, -954.865, -626.049, -1.56308, 0, 0, 26120, 0, 550, 550),
	(262476, 0, 24054, 870, -7563.61, -961.314, -719.553, -3.14159, 0, 0, 26120, 0, 550, 550),
	(262479, 0, 24054, 870, -7553.58, -975.297, -868.729, -2.25485, 0, 0, 26120, 0, 550, 550),
	(262812, 0, 37219, 870, -7732.75, -861.079, -555.471, -1.09296, -0.350791, 0.175443, 21782, 0, 219, 219),
	(262849, 0, 25740, 870, -7682.52, -941.223, -684.335, 2.95349, 0, 0, 28210, 9005, 170, 170),
	(263020, 0, 25685, 870, -7841.17, -949.242, -272.99, 1.41686, 0, 0, 26688, 0, 219, 219),
	(263029, 0, 24491, 870, -7663.98, -942.842, -683.979, 2.52204, 0, 0, 26070, 9777, 170, 170),
	(263034, 0, 51870, 870, -7704.48, -940.841, -650.06, 3.09754, 0, 0, 26575, 9004, 170, 170),
	(263035, 0, 51870, 870, -7652.15, -943.4, -682.637, 2.2836, 0, 0, 28208, 9004, 170, 170),
	(263040, 0, 32818, 870, -7722.6, -941.677, -733.102, 0.011311, -0.0795956, -0.0235451, 26314, 9058, 170, 170),
	(263042, 0, 32817, 870, -7721.13, -941.882, -735.195, 2.29117, 0, 0, 26072, 9004, 170, 170),
	(263084, 0, 24058, 870, -7571.77, -958.999, -645.823, -1.34628, 0, 0, 21664, 0, 413, 413),
	(263095, 0, 32851, 870, -7701.68, -933.852, -701.023, -0.762341, 0, 0, 25926, 9203, 170, 170),
	(263096, 0, 32822, 870, -7741.98, -941.703, -734.395, 2.46608, 0, 0, 25940, 9057, 170, 170),
	(263097, 0, 32825, 870, -7704.88, -942.503, -733.927, -1.3258, 0, 0, 23012, 0, 219, 219),
	(263098, 0, 32825, 870, -7700.98, -941.346, -758.05, -0.357624, 0, 0, 23012, 0, 219, 219),
	(263099, 0, 32823, 870, -7705.71, -942.119, -746.886, -1.64484, 0, 0, 31431, 9057, 170, 170),
	(263100, 0, 32822, 870, -7738.84, -941.376, -750.362, 0.129376, 0, 0, 25916, 9057, 170, 170),
	(263101, 0, 32825, 870, -7713.25, -940.666, -752.904, 0.844349, 0, 0, 23012, 0, 219, 219),
	(263102, 0, 32781, 870, -7691.29, -938.236, -614.972, -2.46045, 0, 0, 26571, 9004, 170, 170),
	(263103, 0, 32781, 870, -7655.06, -943.469, -681.412, -1.52351, 0, 0, 27533, 9003, 170, 170),
	(263104, 0, 32781, 870, -7646.81, -942.61, -638.554, -3.14159, 0, 0, 27536, 9004, 170, 170),
	(263105, 0, 32780, 870, -7700.34, -940.507, -648.308, 2.74308, 0, 0, 26072, 9004, 170, 170),
	(263108, 0, 25592, 870, -7684.32, -941.307, -681.645, -0.581855, 0.0716395, -0.0272887, 31403, 9058, 170, 170),
	(263263, 0, 25378, 870, -7687.77, -864.276, -600.977, -2.94363, -0.000000000926764, -0.0000000000920319, 26316, 9057, 170, 170),
	(264030, 0, 11254, 870, -7716.56, -935.766, -602.192, 0, 0, 0, 22968, 0, 219, 219);
/*!40000 ALTER TABLE `entity` ENABLE KEYS */;

-- Dumping structure for table nexus_forever_world.entity_vendor
CREATE TABLE IF NOT EXISTS `entity_vendor` (
  `id` int(10) unsigned NOT NULL DEFAULT '0',
  `buyPriceMultiplier` float NOT NULL DEFAULT '1',
  `sellPriceMultiplier` float NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  CONSTRAINT `FK__entity_vendor_id__entity_id` FOREIGN KEY (`id`) REFERENCES `entity` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Dumping data for table nexus_forever_world.entity_vendor: ~1 rows (approximately)
/*!40000 ALTER TABLE `entity_vendor` DISABLE KEYS */;
INSERT INTO `entity_vendor` (`id`, `buyPriceMultiplier`, `sellPriceMultiplier`) VALUES
	(263029, 1, 1);
/*!40000 ALTER TABLE `entity_vendor` ENABLE KEYS */;

-- Dumping structure for table nexus_forever_world.entity_vendor_category
CREATE TABLE IF NOT EXISTS `entity_vendor_category` (
  `id` int(10) unsigned NOT NULL DEFAULT '0',
  `index` int(10) unsigned NOT NULL DEFAULT '0',
  `localisedTextId` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`index`),
  CONSTRAINT `FK__entity_vendor_category_id__entity_id` FOREIGN KEY (`id`) REFERENCES `entity` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Dumping data for table nexus_forever_world.entity_vendor_category: ~4 rows (approximately)
/*!40000 ALTER TABLE `entity_vendor_category` DISABLE KEYS */;
INSERT INTO `entity_vendor_category` (`id`, `index`, `localisedTextId`) VALUES
	(263029, 1, 712282),
	(263029, 2, 712302),
	(263029, 3, 712312),
	(263029, 4, 712322);
/*!40000 ALTER TABLE `entity_vendor_category` ENABLE KEYS */;

-- Dumping structure for table nexus_forever_world.entity_vendor_item
CREATE TABLE IF NOT EXISTS `entity_vendor_item` (
  `id` int(10) unsigned NOT NULL DEFAULT '0',
  `index` int(10) unsigned NOT NULL DEFAULT '0',
  `categoryIndex` int(10) unsigned NOT NULL DEFAULT '0',
  `itemId` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`index`),
  CONSTRAINT `FK__entity_vendor_item_id__entity_id` FOREIGN KEY (`id`) REFERENCES `entity` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Dumping data for table nexus_forever_world.entity_vendor_item: ~24 rows (approximately)
/*!40000 ALTER TABLE `entity_vendor_item` DISABLE KEYS */;
INSERT INTO `entity_vendor_item` (`id`, `index`, `categoryIndex`, `itemId`) VALUES
	(263029, 1, 1, 13179),
	(263029, 2, 1, 13176),
	(263029, 3, 1, 13194),
	(263029, 4, 1, 13188),
	(263029, 5, 1, 13191),
	(263029, 6, 1, 13185),
	(263029, 7, 2, 13148),
	(263029, 8, 2, 13151),
	(263029, 9, 2, 13154),
	(263029, 10, 2, 83720),
	(263029, 11, 2, 13157),
	(263029, 12, 2, 83721),
	(263029, 13, 3, 13160),
	(263029, 14, 3, 13163),
	(263029, 15, 3, 13166),
	(263029, 16, 3, 83722),
	(263029, 17, 3, 13169),
	(263029, 18, 3, 83723),
	(263029, 19, 4, 27873),
	(263029, 20, 4, 27875),
	(263029, 21, 4, 27876),
	(263029, 22, 4, 83724),
	(263029, 23, 4, 27874),
	(263029, 24, 4, 83725);
/*!40000 ALTER TABLE `entity_vendor_item` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
