echo Starting the server
dotnet run clear=true initialize=true &
echo Waiting 5 minutes
sleep 5m

# run frontend-ui-tests
cd ClientApp
echo running the UI tests
npx codeceptjs run --verbose