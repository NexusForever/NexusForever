CREATE TABLE `entity_properties` (
    `id` INT(10) UNSIGNED NOT NULL DEFAULT '0',
    `property` TINYINT UNSIGNED NOT NULL DEFAULT '0',
    `base` FLOAT NOT NULL DEFAULT '0',
    `value` FLOAT NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `property`),
    CONSTRAINT `FK__entity_properties_property_id_entity_id` FOREIGN KEY (`id`) REFERENCES `entity` (`id`) ON DELETE CASCADE
);

INSERT IGNORE INTO `entity_properties`(id,property,base,value) SELECT `id`, 7, 1000, 1000 FROM `entity`;   #basehealth
