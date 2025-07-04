param([Parameter(Mandatory=$true)][string]$zipfile, [Parameter(Mandatory=$false)][string]$outpath)
Add-Type -AssemblyName System.IO.Compression.FileSystem
If($zipfile.length -eq 0){  Throw "Not valid inputPath"}
Write-Output "executing extraction ";

 If( $zipfile.length -gt 0 -and  !($outpath) )
 { Write-Output "output folder not specefied '$outpath'"

    $pos = $zipfile.LastIndexOf(".")
    $defaultFolder = $zipfile.Substring(0, $pos)
    Write-Output $defaultFolder
    Write-Output "setting default path"
     $outpath =  $defaultFolder
 }
 $inPath= Join-Path (pwd) $zipfile
 $outDir =  Join-Path (pwd) $outpath
 Write-Output $inPath $outDir

 if (Test-Path $outDir ) { 

 Get-Childitem -Recurse -Path $outDir -File | foreach  {$_.Set_IsReadOnly($False)}
 Write-Output "OutputPath exists, removing contents of '$outDir'"
 Get-ChildItem $outDir -Recurse | Remove-Item -Recurse -ErrorAction Ignore 
 Write-Output "Success, removing contents of '$outDir'"
 }
 
 & "C:\Program Files (x86)\7-Zip\7z.exe" x -y $inPath "-o$outDir"