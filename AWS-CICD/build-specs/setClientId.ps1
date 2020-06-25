$userIndex=$Env:CODEBUILD_BUILD_NUMBER % $Env:UserCount
echo $userIndex
$FORGE_CLIENT_ID=(Get-ChildItem -Path Env:\FORGE_CLIENT_ID$userIndex).Value
echo $FORGE_CLIENT_ID
$FORGE_CLIENT_SECRET=(Get-ChildItem -Path Env:\FORGE_CLIENT_SECRET$userIndex).Value
echo $FORGE_CLIENT_SECRET
(Get-Content -path .\AWS-CICD\build-specs\appsettings.Local.json -Raw) -replace '<YOUR_CLIENT_ID>',"$FORGE_CLIENT_ID" -replace '<YOUR_CLIENT_SECRET>',"$FORGE_CLIENT_SECRET" | Set-Content -Path WebApplication\appsettings.Local.json
