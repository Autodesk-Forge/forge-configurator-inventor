FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build

RUN powershell.exe -Command Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
RUN choco install -y nodejs
ARG COMMIT_ID
ENV CODEBUILD_RESOLVED_SOURCE_VERSION=$COMMIT_ID

COPY . /app
WORKDIR /app

RUN nuget restore
RUN msbuild -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY --from=build /app/bin/Release/netcoreapp3.1/publish/ app/
WORKDIR /app

#RUN powershell.exe Invoke-WebRequest -UseBasicParsing -OutFile amazon-cloudwatch-agent.msi https://s3.us-west-2.amazonaws.com/amazoncloudwatch-agent-us-west-2/windows/amd64/latest/amazon-cloudwatch-agent.msi
#RUN msiexec /i amazon-cloudwatch-agent.msi
#COPY amazon-cloudwatch-agent.json /opt/aws/amazon-cloudwatch-agent/etc/
COPY commands.bat .

ENTRYPOINT ["./commands.bat"]