CREATE TABLE `residence_decor` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `decorId` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `decorInfoId` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `decorType` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `scale` FLOAT NOT NULL DEFAULT '0',
    `x` FLOAT NOT NULL DEFAULT '0',
    `y` FLOAT NOT NULL DEFAULT '0',
    `z` FLOAT NOT NULL DEFAULT '0',
    `qx` FLOAT NOT NULL DEFAULT '0',
    `qy` FLOAT NOT NULL DEFAULT '0',
    `qz` FLOAT NOT NULL DEFAULT '0',
    `qw` FLOAT NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `decorId`),
    CONSTRAINT `FK__residence_decor_id__residence_id` FOREIGN KEY (`id`) REFERENCES `residence` (`id`) ON DELETE CASCADE
);
