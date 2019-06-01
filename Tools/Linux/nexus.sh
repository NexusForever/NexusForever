# Making sure that paths are correct
DIR=$(dirname "${BASH_SOURCE[0]}")

# Making sure that DIR is not empty
if [ "${DIR}" = "" ] || [ ! -d "${DIR}" ]; then
        echo "Source directory is empty?!"
        exit
fi
cd ${DIR}

function _run {
    cd ../../Source
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
}

function _kill {
    echo "Killing NexusForever.StsServer"
    pkill -f NexusForever.StsServer

    echo "Killing NexusForever.AuthServer"
    pkill -f NexusForever.AuthServer

    echo "Killing NexusForever.WorldServer"
    pkill -f NexusForever.WorldServer
}

function _setup {
    GAME_PATH=$1
    TABLE_PATH=$2
    DB_USER=$3
    DB_PW=$4

    # Some validation beforehand
    if [ "${GAME_PATH}" == "" ] || [ "${TABLE_PATH}" == "" ] || [ "${DB_USER}" == "" ]; then
            echo "setup.sh game_path table_path db_user [db_pw]"
            exit
    fi

    DNS="user=${DB_USER}";

    if [ "$DB_PW" != "" ]; then
            DNS="user=${DB_USER};password=${DB_PW}"
    fi

    cd ../../

    # Cleaning
    echo "Cleaning the directory from previous setups"
    rm -rf ./Source/NexusForever.AuthServer/bin;
    rm -rf ./Source/NexusForever.StsServer/bin;
    rm -rf ./Source/NexusForever.WorldServer/bin;

    # Shamelessly stolen from the wiki @Ahoiahoi
    # And slightly modified
    cd ./Source
    framework_version=$(sed -n 's/.*>\(.*[0-9].[0-9]\).*/\1/p' NexusForever.Shared/NexusForever.Shared.csproj)
    for server in WorldServer StsServer AuthServer; do 
            cd NexusForever.${server}
            mkdir -pv ./bin/Debug/${framework_version}/
            cat ${server}.example.json > ./bin/Debug/${framework_version}/${server}.json
            sed -i -e "s/user\=nexusforever\;password\=nexusforever/${DNS}/g" ./bin/Debug/${framework_version}/${server}.json
            cd ..
    done

    cd ./NexusForever.WorldServer/
    sed -i -e "s|\(.*MapPath\": \"\)\(.*\)|\1${PWD}\/bin\/\2|" ./bin/Debug/${framework_version}/WorldServer.json
    mkdir ./bin/Debug/${framework_version}/tbl
    cp -r ${TABLE_PATH}/* ./bin/Debug/${framework_version}/tbl
    cd ..

    # Setting up maps
    cd ./NexusForever.MapGenerator/
    dotnet run $GAME_PATH/Patch/
    cp -a ./map ./../NexusForever.WorldServer/bin/map
    rm -rf ./map
    rm -rf ./tbl

    # Setting up db
    cd ../../
    chmod 755 ./Database/scripts/bootstrap.sh
    ./Database/scripts/bootstrap.sh
}


# Getopts real start
usage="$(basename "${0}") [-r|k|s|h]
where:
   -r run
   -k kill
   -s setup
   -h show this help message"


while getopts 'hrks' option; do
    case "${option}" in
        h)  echo "${usage}"
            exit
        ;;
        r)  _run
            exit
        ;;
        k)  _kill
            exit
        ;;
        s)  _setup $2 $3 $4
            exit
        ;;
        \?) printf "illegal option: -%s\n" "${OPTARG}" >&2
            echo "${usage}" >&2
            exit
        ;;
    esac
done

echo "${usage}"
