# Making sure that paths are correct
DIR=$(dirname "${BASH_SOURCE[0]}")

# Making sure that DIR is not empty
if [ "${DIR}" = "" ] || [ ! -d "${DIR}" ]; then
        echo "Source directory is empty?!"
        exit
fi

DIR="${DIR}/../.."
cd ${DIR}/Source

export TERM=vt100
for server in StsServer AuthServer; do
        if [ $(ps aux | grep NexusForever.${server} | wc -l) -eq 1 ]; then
                cd NexusForever.${server}
                dotnet run&
                cd ..
        else
                echo "${server} already running!"
        fi;
done
if [ $(ps aux | grep NexusForever.WorldServer | wc -l) -eq 1 ]; then
        cd NexusForever.WorldServer
        dotnet run
else   
        echo "WorldServer already running!"
fi;
