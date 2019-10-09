param([string]$PathToFile)
& '.\cloc-1.84.exe' $pathToFile --json --by-file > 'CodeStatisticsOutput.txt'