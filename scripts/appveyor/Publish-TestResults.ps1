[CmdletBinding()]
param()
$wc = New-Object 'System.Net.WebClient';
$TestUploadUrl = "https://ci.appveyor.com/api/testresults/mstest/$($env:APPVEYOR_JOB_ID)"
Get-ChildItem -Recurse *.trx | ForEach-Object{
    Write-Host "Uploading $($_.Fullname) to $($TestUploadUrl) . . ."
    $wc.UploadFile($TestUploadUrl, $_.FullName)
}