ALTER TABLE `character` ADD COLUMN `locationX` float NOT NULL DEFAULT '0', ADD COLUMN `locationY` float NOT NULL DEFAULT '0', ADD COLUMN `locationZ` float NOT NULL DEFAULT '0', ADD COLUMN `worldId` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0';
UPDATE `character` SET `locationX`=-7683.809, `locationY` = -942.5914, `locationZ` = -666.6343, `worldId` = 870 WHERE worldId = 0;

