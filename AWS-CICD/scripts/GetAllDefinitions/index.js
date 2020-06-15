const aws = require('aws-sdk');
const codebuild = new aws.CodeBuild({region: 'us-west-2'});
const fs = require('fs');
const util = require('util');

exportBuild(['sdra-windows-build']);

async function exportBuild(buildNames) {
   const projects = await codebuild.batchGetProjects({
      names: buildNames
   }).promise();

   project = projects.projects[0];
   project.arn = undefined;
   project.created = undefined;
   project.lastModified = undefined;
   project.badge = undefined;

   json = JSON.stringify(projects, null, '    ');
   fs.writeFileSync(buildNames[0] + '.json', json);
}