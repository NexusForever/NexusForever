ALTER TABLE `character`
        ADD COLUMN `inputKeySet` TINYINT NOT NULL DEFAULT '0' AFTER `activeCostumeIndex`;
