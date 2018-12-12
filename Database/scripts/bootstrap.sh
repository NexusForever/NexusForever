#!/bin/bash

# Create Databases
mysql -u root -e "CREATE DATABASE nexus_forever_auth;"
mysql -u root -e "CREATE DATABASE nexus_forever_character;"
mysql -u root -e "CREATE DATABASE nexus_forever_world;"

# Create user for servers to use
mysql -u root -e "CREATE USER nexusforever;"
mysql -u root -e "GRANT ALL ON *.* to nexusforever IDENTIFIED BY nexusforever"

# Initial DB setup
mysql -u root -D nexus_forever_auth < ../Base/Auth.sql
mysql -u root -D nexus_forever_character < ../Base/Character.sql
mysql -u root -D nexus_forever_world < ../Base/World.sql

# Run Auth migrations
for filename in ../Updates/Auth/*.sql; do
    mysql -u root -D nexus_forever_auth < "$filename"
done

# Run Character migrations
for filename in ../Updates/Character/*.sql; do
    mysql -u root -D nexus_forever_character < "$filename"
done

# Run World migrations
for filename in ../Updates/World/*.sql; do
    mysql -u root -D nexus_forever_world < "$filename"
done

# Insert server info into Auth DB
mysql -u root -D nexus_forever_auth -e "INSERT INTO `server` VALUES ('NexusForever', '127.0.0.1', 24000, 0);"
