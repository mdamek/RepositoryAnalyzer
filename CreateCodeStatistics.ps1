param([string]$FolderPath)

if(!(Test-Path 'OutputsFiles'))
{
    new-item -Name 'OutputsFiles' -ItemType directory
}

$FileExtension = ".sln"
$solutionPath = Get-ChildItem -Path $FolderPath -Recurse -ErrorAction SilentlyContinue -Filter *$FileExtension |
Where-Object { $_.Extension -eq $FileExtension } | Select-Object Select -ExpandProperty FullName
$codeLinesCalculions =  &'.\cloc-1.84.exe' $FolderPath --json --by-file
$path = $FolderPath.split('\')
$root = $path[$path.count -1]
& 'BasicMetricsCalculator\Metrics.exe' "/solution:$solutionPath"  '/out:OutputsFiles\BasicMetrics.xml'
$codeLinesCalculions.replace($FolderPath, $root) > 'OutputsFiles\CodeStatisticsOutput.txt'
Write-Host 'Calculating lines of code...'
& 'AnalyzeManager\build\AnalyzeManager.exe' 'OutputsFiles' $FolderPath
Write-Host 'All statistics of'$solutionPath ' solution are ready.'
if(!(Test-Path 'node_modules'))
{
    npm install http-server 
}
Start-Process npx -ArgumentList "http-server -o /AnalyzeReport/GitAnalyzeReport"
