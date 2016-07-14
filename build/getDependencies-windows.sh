#!/bin/bash
# server=build.palaso.org
# project=PhotoStoryToBloomTools
# build=
# root_dir=..


cd "$(dirname "$0")"

# *** Functions ***
force=0
clean=0

while getopts fc opt; do
case $opt in
f) force=1 ;;
c) clean=1 ;;
esac
done

shift $((OPTIND - 1))

copy_auto() {
if [ "$clean" == "1" ]
then
echo cleaning $2
rm -f ""$2""
else
where_curl=$(type -P curl)
where_wget=$(type -P wget)
if [ "$where_curl" != "" ]
then
copy_curl $1 $2
elif [ "$where_wget" != "" ]
then
copy_wget $1 $2
else
echo "Missing curl or wget"
exit 1
fi
fi
}

copy_curl() {
echo "curl: $2 <= $1"
if [ -e "$2" ] && [ "$force" != "1" ]
then
curl -# -L -z $2 -o $2 $1
else
curl -# -L -o $2 $1
fi
}

copy_wget() {
echo "wget: $2 <= $1"
f=$(basename $2)
d=$(dirname $2)
cd $d
wget -q -L -N $1
cd -
}




# make sure output directories exist
mkdir -p ../build/
mkdir -p ../lib/dotnet

# download artifact dependencies
copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Core.dll?branch=%3Cdefault%3E ../lib/dotnet/SIL.Core.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Core.pdb?branch=%3Cdefault%3E ../lib/dotnet/SIL.Core.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Windows.Forms.dll?branch=%3Cdefault%3E ../lib/dotnet/SIL.Windows.Forms.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/SIL.Windows.Forms.pdb?branch=%3Cdefault%3E ../lib/dotnet/SIL.Windows.Forms.Core.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt436/latest.lastSuccessful/taglib-sharp.dll?branch=%3Cdefault%3E ../lib/dotnet/taglib-sharp.dll
# End of script
