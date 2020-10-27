const aws = require('aws-sdk');
const pipeline = new aws.CodePipeline({region: 'us-west-2'});
const beanstalk = new aws.ElasticBeanstalk({region: 'us-west-2'});

const args = process.argv.slice(2);

const pipelineName = args[0];
const executionId = args[1];
const codeBuildNumber = args[2];
const applicationName = args[3];
const environmentName = args[4];

deployVersion(pipelineName, executionId, codeBuildNumber, applicationName, environmentName);

function getAction(actions, name) {
   for (i in actions) {
      if (actions[i].actionName === name)
         return actions[i];
   }

   return undefined;
}

async function deployVersion(pipelineName, executionId, codeBuildNumber, applicationName, environmentName) {
   const actions = (await pipeline.listActionExecutions({
      pipelineName: pipelineName,
         filter: {
            pipelineExecutionId: executionId
         }
   }).promise()).actionExecutionDetails;

   const sourceAction = getAction(actions, 'Source'); 
   let commitMessage = sourceAction.output.outputVariables.CommitMessage;

   console.log("Raw commit message: " + commitMessage);

   let commitMessageDashes = commitMessage.replace('Merge pull request ', '(');
   commitMessageDashes = commitMessageDashes.replace(' from Developer-Autodesk/', ')');
   commitMessageDashes = commitMessageDashes.replace(/ /g, '_');
   commitMessageDashes = commitMessageDashes.replace(/(\n)/g, '_');
   commitMessageDashes = commitMessageDashes.replace(/(\/)/g, '_');

   const commitId = sourceAction.output.outputVariables.CommitId;
   console.log("CommitId: " + commitId);

   const versionLabel = codeBuildNumber + '-' + commitId.substring(0, 7) + '-' + commitMessageDashes.substring(0, 15);
   console.log("Version label: " + versionLabel);

   const deployLocation = getAction(actions, 'Image').output.outputArtifacts[0].s3location;

   console.log("Creating application version " + versionLabel + " from artifacts " + deployLocation.key)

   await beanstalk.createApplicationVersion({
      ApplicationName: applicationName,
      Description: commitMessage.substring(0, 199),
      VersionLabel: versionLabel,
      Process: true,
      SourceBundle: {
         S3Bucket: deployLocation.bucket,
         S3Key: deployLocation.key
      }
   }).promise();

   console.log('Deploying version ' + versionLabel);
   await beanstalk.updateEnvironment({
      EnvironmentName: environmentName,
      VersionLabel: versionLabel,
	  OptionSettings: [{
		  Namespace: 'aws:autoscaling:launchconfiguration',
		  OptionName: 'SSHSourceRestriction',
		  Value: 'tcp,22,22,127.0.0.1/32'
	  }]
   }).promise();
}
