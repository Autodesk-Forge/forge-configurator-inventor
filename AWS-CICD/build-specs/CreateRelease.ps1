$version="1.0.$env:CommitId"

$targetConfiguration='Release'
$dotnet='net6.0'

$release_zip_filename="ForgeConvInv-$targetConfiguration-$targetRuntime-$version.zip"

$githubReleasesApiUrl='https://api.github.com/repos/Autodesk-Forge/forge-configurator-inventor/releases'

Write-Host 'Github OAuth token check.'
if ($githubOAuthToken -eq $null)
{
	Write-Host 'Github OAuth token not defined. Please specify variable $githubOAuthToken'
	exit 1
}

$app_publish_dir=".\WebApplication\bin\$targetConfiguration\$dotnet\publish"


##############################
#build WebApplication project
##############################
Write-Host "Building App"
dotnet publish --configuration $targetConfiguration --runtime $targetRuntime .\WebApplication\WebApplication.csproj
rm $app_publish_dir\appsettings.Local.json

Write-Host "Including app bundles"
xcopy /Y /E .\WebApplication\AppBundles\* "$app_publish_dir\AppBundles\"

Write-Host "Zipping the App"
Compress-Archive -Path "$app_publish_dir\*" -DestinationPath "$release_zip_filename" -Force

Write-Host "Release file created: $release_zip_filename"

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

##############################
#create a new github release
##############################
$param = @{
	Headers		= @{'Authorization'="token $githubOAuthToken"}
    Uri         = $githubReleasesApiUrl
    Method      = 'Post'
    Body        = @"
					{
						"tag_name" : "v$version",
						"name" : "Release $version",
						"body" : "Created out of commit hash $($env:CODEBUILD_RESOLVED_SOURCE_VERSION.Substring(0,7))"
					}
"@
    ContentType = 'application/json'
}

$create_release_ret=Invoke-RestMethod @param

Write-Host "Created github release with ID $($create_release_ret.id)"


##############################
#add new asset to the release
##############################

$upload_asset_url=$create_release_ret.upload_url.Substring(0, $create_release_ret.upload_url.Indexof('{'))

$param = @{
	Headers		= @{'Authorization'="token $githubOAuthToken"}
    Uri         = "$upload_asset_url`?name=$release_zip_filename"
    Method      = 'Post'
    ContentType = 'application/zip'
	InFile = $release_zip_filename
}
$upload_asset_ret=Invoke-RestMethod @param

Write-Host "Uploaded github release asset with ID $($upload_asset_ret.id)"
