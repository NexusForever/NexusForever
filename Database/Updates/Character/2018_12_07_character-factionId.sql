ALTER TABLE `character`
	ADD COLUMN `factionId` SMALLINT UNSIGNED NOT NULL DEFAULT '0' AFTER `level`;
UPDATE `character` SET `factionId` = 166 WHERE `factionId` = 0;
