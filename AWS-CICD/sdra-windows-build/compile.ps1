$PathToSln="C:\Users\Administrator\sdra-windows-build\forge-configurator-inventor"
$DockerImage="624240287035.dkr.ecr.us-west-2.amazonaws.com/sdra-windows-build:4.8"
$CommitId=$Args[0]

cd $PSScriptRoot

# clean the solution
Remove-Item $PathToSln\ -Recurse -Force	

# get the source code
Write-Output "Cloning the repo"
git clone https://github.com/Developer-Autodesk/forge-configurator-inventor.git
cd $PathToSln
Write-Output "Checking out commit $CommitId"
git checkout $CommitId
cd ..

# copy build support files, which can't be in git
Copy-Item BuildSupport\* $PathToSln\ -Recurse -Force

# build with restore
docker run --rm -v $PathToSln\:c:/sdra/ $DockerImage msbuild -restore:true sdra\

# push artifacts to S3
aws s3 cp --recursive $PathToSln\WebApplication\AppBundles s3://sdra-build/AppBundles
