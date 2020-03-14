ALTER TABLE `entity`
    ADD COLUMN `activePropId` BIGINT UNSIGNED NOT NULL DEFAULT '0' AFTER `questChecklistIdx`;
