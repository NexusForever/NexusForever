ALTER TABLE `character`
	ADD COLUMN `activeCostumeIndex` TINYINT NOT NULL DEFAULT '-1' AFTER `pathActivatedTimestamp`;
