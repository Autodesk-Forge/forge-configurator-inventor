# Get input parameters
param (
    [string]$awsProfileName = $(throw "-awsProfileName is required.")
)

Set-AWSCredential -ProfileName $awsProfileName

$config = Get-Content -Path config.json | ConvertFrom-Json

# Get the script location
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$pipelinePath = [System.IO.Path]::Combine($scriptPath, "../codepipeline.json")

$input = [System.IO.File]::ReadAllText($pipelinePath)
$input = $input.Replace("<pipeline_name>", $config.pipeline_name).Replace("<code_pipeline_service_role_arn>", $config.code_pipeline_service_role_arn).Replace("<branch>", $config.branch).Replace("<github_token>", $config.github_token).Replace("<environment_name>", $config.environment_name)

$tempFile = New-TemporaryFile
$input | Out-File -Encoding ASCII -NoNewline $tempFile

aws codepipeline update-pipeline --cli-input-json file://$tempFile --profile $awsProfileName

Remove-Item -Path $tempFile