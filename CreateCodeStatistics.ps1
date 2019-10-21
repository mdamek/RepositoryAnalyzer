param([string]$PathToFile)
$allContent =  &'.\cloc-1.84.exe' $pathToFile --json --by-file
$path = $PathToFile.split('\')
$root = $path[$path.count -1]
$allContent.replace($pathToFile, $root) > 'OutputsFiles\CodeStatisticsOutput.txt'
