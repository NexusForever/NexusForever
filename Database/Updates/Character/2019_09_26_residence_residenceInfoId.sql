ALTER TABLE `residence`
	ADD COLUMN `residenceInfoId` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0' AFTER `privacyLevel`;