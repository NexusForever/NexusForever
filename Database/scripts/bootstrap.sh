#!/bin/bash

# Find dir script is running in
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"

# Clean existing DB
mysql -u root -e "DROP DATABASE nexus_forever_auth;"
mysql -u root -e "DROP DATABASE nexus_forever_character;"
mysql -u root -e "DROP DATABASE nexus_forever_world;"
mysql -u root -e "DROP USER nexusforever;"


# Create Databases
mysql -u root -e "CREATE DATABASE nexus_forever_auth;"
mysql -u root -e "CREATE DATABASE nexus_forever_character;"
mysql -u root -e "CREATE DATABASE nexus_forever_world;"

# Create user for servers to use
mysql -u root -e "CREATE USER nexusforever IDENTIFIED BY 'nexusforever';"
mysql -u root -e "GRANT ALL ON *.* to nexusforever;"

# Initial DB setup
mysql -u root -D nexus_forever_auth < $DIR/../Base/Auth.sql
mysql -u root -D nexus_forever_character < $DIR/../Base/Character.sql
mysql -u root -D nexus_forever_world < $DIR/../Base/World.sql

# Run migrations
find $DIR/../Updates/Auth/ -name '*.sql' -exec sh -c "mysql -u root -D nexus_forever_auth < {}" \;
find $DIR/../Updates/Character/ -name '*.sql' -exec sh -c "mysql -u root -D nexus_forever_character < {}" \;
find $DIR/../Updates/World/ -name '*.sql' -exec sh -c "mysql -u root -D nexus_forever_world < {}" \;

# Insert server info into Auth DB
mysql -u root -D nexus_forever_auth -e "INSERT INTO server (name, host, port, type) VALUES ('NexusForever', '127.0.0.1', 24000, 0);"
