# Making sure that paths are correct
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )";
cd $DIR/Source;

export TERM=vt100
for server in StsServer AuthServer; do
        cd NexusForever.${server}
        dotnet run&
        cd ..
done
cd NexusForever.WorldServer
dotnet run
