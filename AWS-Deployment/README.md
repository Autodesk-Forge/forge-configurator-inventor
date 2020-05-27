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
* Update a CodeBuild project property
  * `aws codebuild update-project --name "sdra-test" --source "{\"type\": \"CODEPIPELINE\", \"buildspec\": \"AWS-Deployment/BuildSpecs/buildspec-tests-docker.yml\"}"`
* Create a new CodePipeline pipeline
  * `aws codepipeline create-pipeline --cli-input-json file://codepipeline.json`
* Update a CodePipeline pipeline
  * `aws codepipeline update-pipeline --cli-input-json file://codepipeline.json`