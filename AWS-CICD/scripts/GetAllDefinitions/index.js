const aws = require('aws-sdk');
const codebuild = new aws.CodeBuild({region: 'us-west-2'});
const pipeline = new aws.CodePipeline({region: 'us-west-2'});
const fs = require('fs');

exportBuild(['sdra-windows-build']);
exportBuild(['sdra-deploy']);
exportBuild(['sdra-image']);
exportBuild(['sdra-pr-check']);
exportPipeline('sdra-docker');

async function exportPipeline(pipelineName) {
   const pipelineDef = await pipeline.getPipeline({
      name: pipelineName
   }).promise();

   // strip-out unwanted stuff
   pipelineDef.metadata = undefined;
   pipelineDef.pipeline.name = '<pipeline_name>';
   pipelineDef.pipeline.roleArn = '<code_pipeline_service_role_arn>';
   const sourceConfiguration = pipelineDef.pipeline.stages[0].actions[0].configuration;
   sourceConfiguration.Branch = '<branch>';
   sourceConfiguration.OAuthToken = '<github_token>';

   const deployConfiguration = pipelineDef.pipeline.stages[3].actions[0].configuration;
   deployConfiguration.EnvironmentVariables = deployConfiguration.EnvironmentVariables.replace('sdra-docker', '<pipeline_name>');
   deployConfiguration.EnvironmentVariables = deployConfiguration.EnvironmentVariables.replace('sdra-dev', '<environment_name>');

   json = JSON.stringify(pipelineDef, null, '    ');
   fs.writeFileSync('../../codepipeline.json', json);
}

async function exportBuild(buildNames) {
   const projects = await codebuild.batchGetProjects({
      names: buildNames
   }).promise();

   project = projects.projects[0];

   // strip-out unwanted stuff
   project.arn = undefined;
   project.created = undefined;
   project.lastModified = undefined;
   project.badge = undefined;
   envVars = project.environment.environmentVariables;
   for (envVarId in envVars) {
      if (envVars[envVarId].name === 'FORGE_CLIENT_ID') 
         envVars[envVarId].value = '<FORGE_CLIENT_ID>';
      if (envVars[envVarId].name === 'FORGE_CLIENT_SECRET')
         envVars[envVarId].value = '<FORGE_CLIENT_SECRET>';
   }

   json = JSON.stringify(project, null, '    ');
   fs.writeFileSync('../../CodeBuild/projects/' + buildNames[0] + '.json', json);
}