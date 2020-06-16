FROM mcr.microsoft.com/dotnet/framework/sdk:4.8

RUN powershell.exe -Command Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

RUN choco install -y nodejs
RUN choco install -y awscli
ENV NUGET_PACKAGES=c:\\sdra\\nuget