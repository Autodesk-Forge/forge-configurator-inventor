FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

RUN apt-get update -y
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash --debug
RUN apt-get install nodejs -yq

COPY . /app
WORKDIR /app

RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY --from=build /app/bin/Release/netcoreapp3.1/publish/ app/
WORKDIR /app
ENTRYPOINT ["dotnet", "SalesDemoToolApp.dll"]