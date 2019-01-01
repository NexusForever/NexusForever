ALTER TABLE `account`
	ADD COLUMN `status` TINYINT NOT NULL DEFAULT '1' AFTER `createTime`;
UPDATE `account` SET `status` = 1 WHERE `status` = 0;