ALTER TABLE `residence_decor`
	ADD COLUMN `colourShiftId` smallint(5) UNSIGNED NOT NULL DEFAULT '0' AFTER `decorParentId`;
