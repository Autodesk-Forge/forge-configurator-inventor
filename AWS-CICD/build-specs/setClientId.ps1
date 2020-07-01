$userIndex=$Env:CODEBUILD_BUILD_NUMBER % $Env:UserCount
echo "Using test user index $userIndex"
$FORGE_CLIENT_ID=(Get-ChildItem -Path Env:\FORGE_CLIENT_ID$userIndex).Value
echo "Client ID is $FORGE_CLIENT_ID"
$FORGE_CLIENT_SECRET=(Get-ChildItem -Path Env:\FORGE_CLIENT_SECRET$userIndex).Value
$SDRA_USERNAME=(Get-ChildItem -Path Env:\SDRA_USERNAME$userIndex).Value
echo "SDRA_USERNAME is $SDRA_USERNAME"
$SDRA_PASSWORD=(Get-ChildItem -Path Env:\SDRA_PASSWORD$userIndex).Value
(Get-Content -path .\AWS-CICD\build-specs\appsettings.Local.json -Raw) -replace '<YOUR_CLIENT_ID>',"$FORGE_CLIENT_ID" -replace '<YOUR_CLIENT_SECRET>',"$FORGE_CLIENT_SECRET" -replace '<username-for-auth>',"$SDRA_USERNAME" -replace '<password-for-auth>',"$SDRA_PASSWORD" | Set-Content -Path WebApplication\appsettings.Local.json
