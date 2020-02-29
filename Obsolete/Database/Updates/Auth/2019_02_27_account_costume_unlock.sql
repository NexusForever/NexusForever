CREATE TABLE IF NOT EXISTS `account_costume_unlock` (
    `id` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `itemId` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `timestamp` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`, `itemId`),
    CONSTRAINT `FK__account_costume_item_id__account_id` FOREIGN KEY (`id`) REFERENCES `account` (`id`) ON DELETE CASCADE
);
