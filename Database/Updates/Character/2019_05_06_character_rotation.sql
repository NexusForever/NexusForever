ALTER TABLE `character`
ADD COLUMN `rotationX` float NOT NULL DEFAULT '0' AFTER `locationZ`,
ADD COLUMN `rotationY` float NOT NULL DEFAULT '0' AFTER `rotationX`,
ADD COLUMN `rotationZ` float NOT NULL DEFAULT '0' AFTER `rotationY`;


