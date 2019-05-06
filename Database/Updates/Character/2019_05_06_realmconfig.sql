CREATE TABLE IF NOT EXISTS `realmconfig` (
    `id` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `active` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`)
);

CREATE TABLE IF NOT EXISTS `realmconfig_starting_location` (
    `id` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `race` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `factionId` SMALLINT(5) UNSIGNED NOT NULL DEFAULT '0',
    `locationId` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `characterCreationStart` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `race`, `factionId`, `locationId`, `characterCreationStart`),
    CONSTRAINT `FK__realmconfig_starting_location_id__realmconfig_id` FOREIGN KEY (`id`) REFERENCES `realmconfig` (`id`) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS `realmconfig_custom_location` (
    `id` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `customLocationId` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `position0` FLOAT NOT NULL DEFAULT '0',
    `position1` FLOAT NOT NULL DEFAULT '0',
    `position2` FLOAT NOT NULL DEFAULT '0',
    `facing0` FLOAT NOT NULL DEFAULT '0',
    `facing1` FLOAT NOT NULL DEFAULT '0',
    `facing2` FLOAT NOT NULL DEFAULT '0',
    `facing3` FLOAT NOT NULL DEFAULT '0',
    `worldId` SMALLINT(5) unsigned NOT NULL DEFAULT '0',
    `worldZoneId` SMALLINT(5) unsigned NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `customLocationId`),
    CONSTRAINT `FK__realmconfig_custom_location_id__realmconfig_id` FOREIGN KEY (`id`) REFERENCES `realmconfig` (`id`) ON DELETE CASCADE,
    CHECK (customLocationId >= 100000)
);

INSERT INTO `realmconfig`(`id`, `active`) VALUES(1, 1);

#human veteran
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 1, 167, 1594, 3);
#human novice
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 1, 167, 51596, 4);

#granok veteran
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 3, 167, 1594, 3);
#granok novice
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 3, 167, 51596, 4);

#chua veteran
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 13, 166, 8223, 3);
#chua novice
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 13, 166, 51596, 4);

#draken veteran
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 5, 166, 8223, 3);
#draken novice
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 5, 166, 51596, 4);

#cassian veteran
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 1, 166, 18987, 3);
#cassian novice
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 1, 166, 51596, 4);

#mechari veteran
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 12, 166, 18987, 3);
#mechari novice
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 12, 166, 51596, 4);

#aurin veteran
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 4, 167, 37015, 3);
#aurin novice
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 4, 167, 51596, 4);

#mordesh veteran
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 16, 167, 37015, 3);
#mordesh novice
INSERT INTO `realmconfig_starting_location`(`id`, `race`, `factionId`, `locationId`, `characterCreationStart`) VALUES(1, 16, 167, 51596, 4);
