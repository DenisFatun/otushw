из корня проекта:
docker-compose up -d
docker-compose restart

свагер будет по адресу:
https://localhost:55555/swagger/index.html

rm -rf pgslave1 - удалить директорию

запуск цитуса
docker-compose -p citus up --scale worker=2 -d

docker run --name otusdb -e POSTGRES_PASSWORD=postgres123 -p 5432:5432 -d postgres
docker exec -it otusdb bash
psql -h localhost -U postgres

\l --show all db
\c otusdb

docker-compose ps
docker-compose stop homeworkotus
docker-compose build homeworkotus
docker-compose start homeworkotus

if not helped
docker-compose rm homeworkotus

install nano:
apt-get update
apt-get install nano

docker cp pgmaster:/var/lib/postgresql/data/postgresql.conf D:\temp\docker - копирую к себе
Обновляю:
	ssl = off
	wal_level = replica
	max_wal_senders = 2
docker cp D:\temp\docker\postgresql.conf pgmaster:/var/lib/postgresql/data - обратно

Если надо проверить:
docker exec -it pgmaster bash
cd /var/lib/postgresql/data
cat postgresql.conf

docker cp pgmaster:/var/lib/postgresql/data/pg_hba.conf D:\temp\docker - копирую к себе
host    replication     replicator 	172.21.0.0/16	md5
	docker network ls - показать сети
	docker network inspect otushw_somenetweork | grep Subnet - маска которая интересует в настройках
docker cp D:\temp\docker\pg_hba.conf pgmaster:/var/lib/postgresql/data - обратно

docker exec -it pgmaster su - postgres -c psql
create role replicator with login replication password 'pass'; - создаю пользователя для репликации

docker restart pgmaster -ребут

Делаю бэкап для реплик:
docker exec -it pgmaster bash
mkdir /pgslave
pg_basebackup -h postgres_db -D /pgslave -U replicator -v -P --wal-method=stream
docker cp pgmaster:/pgslave D:\temp\docker\pgslave - копирую к себе

Меняю для первой реплики postgresql.conf:
	primary_conninfo = 'host=postgres_db port=5432 user=replicator password=pass application_name=pgslave1'
docker cp D:\temp\docker\pgslave\. pgslave1:/var/lib/postgresql/data - копирую на первую реплику
docker restart pgslave1

Меняю для второй реплики postgresql.conf:
	primary_conninfo = 'host=postgres_db port=5432 user=replicator password=pass application_name=pgslave2'
docker cp D:\temp\docker\pgslave2\. pgslave2:/var/lib/postgresql/data

docker restart pgslave2


docker exec -it mytarantool console