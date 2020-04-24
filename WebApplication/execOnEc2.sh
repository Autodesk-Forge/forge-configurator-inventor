#/usr/bin/env bash -x
instanceId="$1"
commandToRun="$2"
commandId=$(aws ssm send-command --instance-id "$instanceId" --document-name "AWS-RunPowerShellScript" --output-s3-bucket-name "sdra-build" --output-s3-key-prefix "BuildConsole" --parameters commands=["$commandToRun"] --query "Command.CommandId" --output text)
echo "CommandId - $commandId"
while [ "$(aws ssm get-command-invocation --command-id "$commandId" --instance-id "$instanceId" --query "Status" --output text)" == "InProgress" ]; do sleep 10; done
status=$(aws ssm get-command-invocation --command-id "$commandId" --instance-id "$instanceId" --query "Status" --output text)
echo "Status $status"
aws s3 cp s3://sdra-build/BuildConsole/$commandId/$instanceId/awsrunPowerShellScript/0.awsrunPowerShellScript/stdout ./stdout
cat stdout