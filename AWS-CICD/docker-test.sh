#!/bin/bash

# exit when any command fails
set -e

# run backend tests (this also builds and installs dependencies)
echo "**** running backend tests ****"
dotnet test

# run frontend-lint
echo "**** running linter ****"
cd ./WebApplication/ClientApp
npm run lint

# run frontend-tests
echo "**** running frontend tests ****"
npm test -- --coverage

# run server in background and wait for it to start up
cd ..
echo "**** Starting the server ****"
dotnet run clear=true initialize=true &
echo Waiting 5 minutes
sleep 5m

# run frontend-ui-tests
cd ./ClientApp
echo "**** running the UI tests ****"
npx codeceptjs run