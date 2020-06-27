CREATE TABLE `server_message` (
    `index` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `language` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
    `message` VARCHAR(256) NOT NULL DEFAULT '',
    PRIMARY KEY (`index`, `language`)
);

INSERT INTO `server_message` (`index`, `language`, `message`) VALUES
	(0, 0, 'Welcome to this NexusForever server!\r\nVisit: https://github.com/NexusForever/NexusForever'),
	(0, 1, 'Willkommen auf diesem NexusForever server!\r\nBesuch: https://github.com/NexusForever/NexusForever');
