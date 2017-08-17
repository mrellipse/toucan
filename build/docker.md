
# DEBUG NOTES:
# To logon to an instance of docker container

# $ docker run -it -P --net=bridge --entrypoint "" toucan:latest bash
# $ docker run -it -P --dns 8.8.8.8 --add-host toucan:10.0.75.1 --net=bridge --entrypoint "" toucan:latest bash
# $ cat < /dev/tcp/172.17.0.2/80
# $ exit 1

# Delete all container instances
# $ docker rm $(docker ps -a -q)

# Delete all 'dangling' unused images
# $ docker rmi $(docker images -q -f dangling=true)

$ docker volume prune

# $ docker network create -d bridge --subnet 172.34.0.0/16 toucan
# $ docker run --network=toucan --name web01 --ip=172.34.0.16 --add-host=toucan:10.0.75.1  -d -e TOUCAN_URLS=http://0.0.0.0:80 -e TOUCAN_DBHOST=toucan toucan:latest
# $ docker run --network=toucan --name web02 --ip=172.34.0.17 --add-host=toucan:10.0.75.1 -d -e TOUCAN_URLS=http://0.0.0.0:80 -e TOUCAN_DBHOST=toucan toucan:latest
# $ docker create --network=toucan --name ha01 --ip=172.34.0.2 --add-host=toucan:10.0.75.1 -d -p 80:80  toucan-proxy:latest
# $ docker cp ./haproxy.cfg /usr/local/etc/haproxy/haproxy.cfg
# $ docker start ha01 