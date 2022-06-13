$userIndex=$Env:CommitId % $Env:UserCount
echo "Using test user index $userIndex"
$FORGE_CLIENT_ID=(Get-ChildItem -Path Env:\FORGE_CLIENT_ID$userIndex).Value
echo "Client ID is $FORGE_CLIENT_ID"
$FORGE_CLIENT_SECRET=(Get-ChildItem -Path Env:\FORGE_CLIENT_SECRET$userIndex).Value
(Get-Content -path .\AWS-CICD\build-specs\appsettings.Local.json -Raw) -replace '<YOUR_CLIENT_ID>',"$FORGE_CLIENT_ID" -replace '<YOUR_CLIENT_SECRET>',"$FORGE_CLIENT_SECRET" -replace '<BUCKET_KEY_SUFFIX>',"$Env:CommitId" | Set-Content -Path WebApplication\appsettings.Local.json
