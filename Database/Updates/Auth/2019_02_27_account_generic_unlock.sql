CREATE TABLE `account_generic_unlock` (
    `id` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `entry` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `timestamp` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`, `entry`),
    CONSTRAINT `FK__account_generic_unlock_id__account_id` FOREIGN KEY (`id`) REFERENCES `account` (`id`) ON DELETE CASCADE
);
