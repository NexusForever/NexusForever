ALTER TABLE `residence`
	ADD COLUMN `musicId` INT(5) UNSIGNED NOT NULL DEFAULT '0' AFTER `groundWallpaperId`;
ALTER TABLE `residence_decor`
	ADD COLUMN `decorParentId` INT(20) UNSIGNED NOT NULL DEFAULT '0' AFTER `qw`;
