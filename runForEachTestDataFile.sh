./publish.sh

rm -f ./testData/*.result.csv

files=$(ls ./testData | grep .dat)

echo $files

for file in $files
do
	echo "$file"
	./bin/Release/MO-QAP.exe ./testData/$file 30 10 ./testData/$file.result.csv
done