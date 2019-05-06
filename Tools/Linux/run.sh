# Making sure that paths are correct
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

# Making sure that DIR is not empty
if [ "${DIR}" = "" ] || [ ! -d "${DIR}" ]; then
        echo "Source directory is empty?!"
        exit
fi
cd ${DIR} || exit "Something went wrong"

DIR="${DIR}/../../"
cd ${DIR}/Source

export TERM=vt100
for server in StsServer AuthServer; do
        cd NexusForever.${server}
        dotnet run&
        cd ..
done
cd NexusForever.WorldServer
dotnet run
