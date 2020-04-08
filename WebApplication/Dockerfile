FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

RUN apt-get update -y
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash
RUN apt-get install nodejs -yq
RUN wget https://s3.us-west-2.amazonaws.com/amazoncloudwatch-agent-us-west-2/debian/amd64/latest/amazon-cloudwatch-agent.deb

COPY . /app/
WORKDIR /app

RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY --from=build /app/bin/Release/netcoreapp3.1/publish/ app/
WORKDIR /app

COPY --from=build amazon-cloudwatch-agent.deb .
RUN dpkg -i -E ./amazon-cloudwatch-agent.deb
COPY amazon-cloudwatch-agent.json /opt/aws/amazon-cloudwatch-agent/etc/
COPY commands.sh .
RUN chmod +x commands.sh

ENTRYPOINT ["./commands.sh"]