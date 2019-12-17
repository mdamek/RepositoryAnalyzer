param([string]$FolderPath)
$FileExtension = ".sln"
$solutionPath = Get-ChildItem -Path $FolderPath -Recurse -ErrorAction SilentlyContinue -Filter *$FileExtension |
Where-Object { $_.Extension -eq $FileExtension } | Select-Object Select -ExpandProperty FullName
$codeLinesCalculions =  &'.\cloc-1.84.exe' $FolderPath --json --by-file
$path = $FolderPath.split('\')
$root = $path[$path.count -1]
& 'BasicMetricsCalculator\Metrics.exe' "/solution:$solutionPath"  '/out:OutputsFiles\BasicMetrics.xml'
$codeLinesCalculions.replace($FolderPath, $root) > 'OutputsFiles\CodeStatisticsOutput.txt'
Write-Host 'Calculating lines of code...'
& 'AnalyzeManager\build\AnalyzeManager.exe' 'OutputsFiles\CodeStatisticsOutput.txt' $FolderPath
Write-Host 'All statistics of'$solutionPath ' solution are ready.'
if(!(Test-Path 'node_modules'))
{
    npm install http-server
}
Start-Job {npx http-server}
Write-Host 'Waiting for http server start'
Start-Sleep -s 3
Start-Process http://127.0.0.1:8080/AnalyzeReport/GitAnalyzeReport