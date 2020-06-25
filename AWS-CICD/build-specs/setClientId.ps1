$userIndex=$Env:CODEBUILD_BUILD_NUMBER % $Env:UserCount
echo $userIndex
$FORGE_CLIENT_ID=(Get-ChildItem -Path Env:\FORGE_CLIENT_ID$userIndex).Value
echo $FORGE_CLIENT_ID
[System.Environment]::SetEnvironmentVariable('FORGE_CLIENT_ID',"$FORGE_CLIENT_ID", [System.EnvironmentVariableTarget]::User)
echo $Env:FORGE_CLIENT_ID
$FORGE_CLIENT_SECRET=(Get-ChildItem -Path Env:\FORGE_CLIENT_SECRET$userIndex).Value
echo $FORGE_CLIENT_SECRET
[System.Environment]::SetEnvironmentVariable('FORGE_CLIENT_SECRET',"$FORGE_CLIENT_SECRET", [System.EnvironmentVariableTarget]::User)
echo $Env:FORGE_CLIENT_SECRET
