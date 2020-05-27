# AWS Deployment
Files used only for Autodesk AWS deployment
* TODO move out of this repo
## CLI Commands
* Get the definition of the CodePipeline pipeline
  * `aws codepipeline get-pipeline --name sdra-docker > codepipeline.json`
* Get the definition of CodeBuild projects 
  * `aws codebuild batch-get-projects --names sdra-windows-build sdra-build sdra-test sdra-deploy > codebuild-projects.json`
* Create a new CodeBuild project
  * `aws codebuild create-project --cli-input-json file://codebuild-test-project.json`
* Update a CodeBuild project property (examples)
  * `aws codebuild update-project --name "sdra-test" --source "{\"type\": \"CODEPIPELINE\", \"buildspec\": \"AWS-Deployment/BuildSpecs/buildspec-tests-docker.yml\"}"`
  * `aws codebuild update-project --name "sdra-test" --environment "{\"type\": \"LINUX_CONTAINER\", \"image\": \"aws/codebuild/amazonlinux2-x86_64-standard:3.0\", \"computeType\": \"BUILD_GENERAL1_SMALL\", \"environmentVariables\": [{\"name\":\"FORGE_CLIENT_ID\",\"value\":\"<id>\"}, {\"name\":\"FORGE_CLIENT_SECRET\",\"value\":\"<secret>\"}], \"privilegedMode\": true, \"imagePullCredentialsType\": \"CODEBUILD\"}"`
* Create a new CodePipeline pipeline
  * `aws codepipeline create-pipeline --cli-input-json file://codepipeline.json`
* Update a CodePipeline pipeline
  * `aws codepipeline update-pipeline --cli-input-json file://codepipeline.json`