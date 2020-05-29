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
ARG FORGE_CLIENT_ID
ENV FORGE_CLIENT_ID ${FORGE_CLIENT_ID}
ARG FORGE_CLIENT_SECRET
ENV FORGE_CLIENT_SECRET ${FORGE_CLIENT_SECRET}
RUN dotnet test

# run frontend-lint
WORKDIR /sln/WebApplication/ClientApp
RUN npm run lint

# run frontend-tests
RUN npm test

# run server in background and wait for it to start up
RUN apt-get update -y
RUN apt-get install chromium -y
RUN mkdir -p /root/.cache/ms-playwright/chromium-764964/chrome-linux
RUN ln /usr/bin/chromium /root/.cache/ms-playwright/chromium-764964/chrome-linux/chrome
WORKDIR /sln/WebApplication
RUN ./docker-test.sh