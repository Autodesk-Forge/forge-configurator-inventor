const aws = require('aws-sdk');
const beanstalk = new aws.ElasticBeanstalk({region: 'us-west-2'});

const args = process.argv.slice(2);

const environmentName = args[0];

deployVersion(environmentName);

async function deployVersion(environmentName) {
   console.log('updating environment: ' + environmentName);
   await beanstalk.updateEnvironment({
      EnvironmentName: environmentName,
	  OptionSettings: [{
		  Namespace: 'aws:autoscaling:launchconfiguration',
		  OptionName: 'SSHSourceRestriction',
		  Value: 'tcp,22,22,132.188.0.0/24'
	  }]
   }).promise();
}
