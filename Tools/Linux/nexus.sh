# Making sure that paths are correct
DIR=$(dirname "${BASH_SOURCE[0]}")

# Making sure that DIR is not empty
if [ "${DIR}" = "" ] || [ ! -d "${DIR}" ]; then
        echo "Source directory is empty?!"
        exit
fi
cd ${DIR}

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
        r)  bash ./run.sh
            exit
        ;;
        k)  bash ./kill.sh
            exit
        ;;
        s)  bash ./setup.sh $2 $3 $4
            exit
        ;;
        \?) printf "illegal option: -%s\n" "${OPTARG}" >&2
            echo "${usage}" >&2
            exit
        ;;
    esac
done

echo "${usage}"
