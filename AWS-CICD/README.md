# AWS CI/CD
Files used only for AWS deployment
* These are not required to run application. It can be used as an example to setup CI/CD on AWS.
## CLI Commands
* Get the definition of the CodePipeline pipeline
  * see GetAllDefinitions bellow
* Get the definition of CodeBuild projects 
  * see GetAllDefinitions bellow
* Create a new CodeBuild project
  * `aws codebuild create-project --cli-input-json file://codebuild-test-project.json`
* Update a CodeBuild project property (examples)
  * `aws codebuild update-project --name "sdra-test" --source "{\"type\": \"CODEPIPELINE\", \"buildspec\": \"AWS-CICD/CodeBuild/build-specs/buildspec-tests.yml\"}"`
  * `aws codebuild update-project --name "sdra-test" --environment "{\"type\": \"LINUX_CONTAINER\", \"image\": \"aws/codebuild/amazonlinux2-x86_64-standard:3.0\", \"computeType\": \"BUILD_GENERAL1_SMALL\", \"environmentVariables\": [{\"name\":\"FORGE_CLIENT_ID\",\"value\":\"<id>\"}, {\"name\":\"FORGE_CLIENT_SECRET\",\"value\":\"<secret>\"}], \"privilegedMode\": true, \"imagePullCredentialsType\": \"CODEBUILD\"}"`
* Create a new CodePipeline pipeline
  * `aws codepipeline create-pipeline --cli-input-json file://codepipeline.json`
* Update a CodePipeline pipeline
  * `aws codepipeline update-pipeline --cli-input-json file://codepipeline.json`
## Powershell Scripts
* For convenience there are some scripts that automate replacing values in the `.json ` files.
* To use them: 
  * Copy `/scripts/config.empty.json` to `/scripts/config.json` and fill in values
  * Then run in a console with commands such as `./update-pipeline.ps1 <aws_profile_name>`
## GetAllDefinitions nodejs module
* This project will help you retrieve CodeBuild and CodePipeline project definitions out of AWS to be stored in git
* Usage:
  * npm install
  * node index.js
  * outputs are stored in the AWS-CICD/CodeBuild/projects folder for codeBuild and AWS-CICD/codepipeline.json file for the pipeline
