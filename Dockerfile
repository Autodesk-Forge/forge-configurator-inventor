# start from the MS image with dotnet core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# install node
RUN apt-get update -y
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash
RUN apt-get install nodejs -yq

# copy all the code from the solution level
COPY . /sln
WORKDIR /sln

# run backend tests (this also builds and installs dependencies)
RUN dotnet test

# run frontend-lint
WORKDIR /sln/WebApplication/ClientApp
RUN npm run lint

# run frontend-tests
RUN npm test

# run server in background and wait for it to start up
WORKDIR /sln/WebApplication
RUN dotnet run clear=true initialize=true &
RUN sleep 5m

# run frontend-ui-tests
WORKDIR /sln/WebApplication/ClientApp
RUN npx codeceptjs run