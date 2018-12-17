CREATE TABLE `character_properties` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `property` TINYINT UNSIGNED NOT NULL DEFAULT '0',
    `base` FLOAT NOT NULL DEFAULT '0',
    `value` FLOAT NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `property`),
    CONSTRAINT `FK__character_properties_property_id_character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);

INSERT INTO `character_properties`(id,property,base,value) SELECT `id`, 7, 200, 800 FROM `character`;   #basehealth
INSERT INTO `character_properties`(id,property,base,value) SELECT `id`, 8, 0.04, 0.04 FROM `character`; #HealthRegenMultiplier
INSERT INTO `character_properties`(id,property,base,value) SELECT `id`, 100, 1, 1 FROM `character`;     #MoveSpeedMultiplier
INSERT INTO `character_properties`(id,property,base,value) SELECT `id`, 129, 2.5, 2.5 FROM `character`; #JumpHeight
INSERT INTO `character_properties`(id,property,base,value) SELECT `id`, 130, 0.8, 0.8 FROM `character`; #GravityMultiplier
