# run backend tests (this also builds and installs dependencies)
slnDir=$(dirname $(readlink -f "$0"))
echo $slnDir
cd $slnDir/WebApplication/ClientApp
echo "**** Getting dependencies ****"
pwd
npm install
echo "**** running backend tests ****"
cd $slnDir
pwd
dotnet test

# run frontend-lint
echo "**** running linter ****"
cd $slnDir/WebApplication/ClientApp
pwd
npm run lint

# run frontend-tests
echo "**** running frontend tests ****"
pwd
npm test

# run server in background and wait for it to start up
cd $slnDir/WebApplication
echo "**** Starting the server ****"
pwd
dotnet run clear=true initialize=true &
echo Waiting 5 minutes
sleep 5m

# run frontend-ui-tests
cd $slnDir/WebApplication/ClientApp
pwd
echo "**** running the UI tests ****"
npx codeceptjs run --verbose