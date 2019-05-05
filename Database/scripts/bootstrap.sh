#!/bin/bash

# Find dir script is running in
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"

echo "Cleaning existing DB"
mysql -v -u root -e "DROP DATABASE nexus_forever_auth;"
mysql -v -u root -e "DROP DATABASE nexus_forever_character;"
mysql -v -u root -e "DROP DATABASE nexus_forever_world;"
mysql -v -u root -e "DROP USER nexusforever;"

echo "Creating databases"
mysql -v -u root -e "CREATE DATABASE nexus_forever_auth;"
mysql -v -u root -e "CREATE DATABASE nexus_forever_character;"
mysql -v -u root -e "CREATE DATABASE nexus_forever_world;"

echo "Creating user for tests to use"
mysql -v -u root -e "CREATE USER nexusforever IDENTIFIED BY 'nexusforever';"
mysql -v -u root -e "GRANT ALL ON *.* to nexusforever;"

echo "Running base scripts"
mysql -v -u root -D nexus_forever_auth < $DIR/../Base/Auth.sql
mysql -v -u root -D nexus_forever_character < $DIR/../Base/Character.sql
mysql -v -u root -D nexus_forever_world < $DIR/../Base/World.sql

echo "Running update scripts"
for file in $(find $DIR/../Updates/Auth/ -name '*.sql' | sort); do
    mysql -u root -D nexus_forever_auth < $file;
done;
for file in $(find $DIR/../Updates/Character/ -name '*.sql' | sort); do
    mysql -u root -D nexus_forever_character < $file;
done;
for file in $(find $DIR/../Updates/World/ -name '*.sql' | sort); do
    mysql -u root -D nexus_forever_world < $file;
done;

echo "Inserting server information"
mysql -v -u root -D nexus_forever_auth -e "INSERT INTO server (name, host, port, type) VALUES ('NexusForever', '127.0.0.1', 24000, 0);"
