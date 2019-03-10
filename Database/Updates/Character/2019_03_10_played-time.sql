ALTER TABLE `character`
ADD `timePlayedTotal` int(10) unsigned NOT NULL DEFAULT '0' AFTER `activeCostumeIndex`,
ADD `timePlayedLevel` int(10) unsigned NOT NULL DEFAULT '0' AFTER `timePlayedTotal`;
