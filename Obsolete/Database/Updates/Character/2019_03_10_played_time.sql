ALTER TABLE `character`
ADD `timePlayedTotal` int(10) unsigned NOT NULL DEFAULT '0' AFTER `activeSpec`,
ADD `timePlayedLevel` int(10) unsigned NOT NULL DEFAULT '0' AFTER `timePlayedTotal`;
