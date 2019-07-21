ALTER TABLE `character`
	CHANGE COLUMN `name` `name` VARCHAR(50) NULL DEFAULT '' AFTER `accountId`,
	ADD COLUMN `deleteTime` DATETIME NULL DEFAULT NULL AFTER `timePlayedLevel`,
	ADD COLUMN `originalName` VARCHAR(50) NULL DEFAULT NULL AFTER `deleteTime`;
