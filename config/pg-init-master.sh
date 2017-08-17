#!/bin/bash
set -e

# postgresql server config
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
    ALTER SYSTEM SET wal_level = hot_standby;
    ALTER SYSTEM SET max_wal_senders = 3;
    ALTER SYSTEM SET min_wal_size = 80;
    ALTER SYSTEM SET max_wal_size = 240;
EOSQL

# create database first to prevent connections being torn down
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
    CREATE DATABASE "Toucan";
EOSQL

# PGMD5=`echo -n $POSTGRES_PASSWORD$POSTGRES_USER | md5sum | cut -d ' ' -f1`;

sed -i '$c\host all postgres 172.18.0.0/24 md5\r' $PGDATA/pg_hba.conf;