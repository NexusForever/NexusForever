-- Matches default NexusForever database names from the server guide.
CREATE DATABASE IF NOT EXISTS nexus_forever_auth;
CREATE DATABASE IF NOT EXISTS nexus_forever_character;
CREATE DATABASE IF NOT EXISTS nexus_forever_world;
CREATE DATABASE IF NOT EXISTS nexus_forever_chat;
CREATE DATABASE IF NOT EXISTS nexus_forever_group;

CREATE USER IF NOT EXISTS 'nexusforever'@'%' IDENTIFIED BY 'nexusforever';
GRANT ALL PRIVILEGES ON *.* TO 'nexusforever'@'%';
FLUSH PRIVILEGES;
