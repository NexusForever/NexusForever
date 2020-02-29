ALTER TABLE `character` 
    ADD COLUMN `innateIndex` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0' AFTER `activeSpec`;