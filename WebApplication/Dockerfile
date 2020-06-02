FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY web/ app/
COPY AppBundles/ app/AppBundles/
COPY amazon-cloudwatch-agent.deb /app
WORKDIR /app

RUN dpkg -i -E ./amazon-cloudwatch-agent.deb
COPY amazon-cloudwatch-agent.json /opt/aws/amazon-cloudwatch-agent/etc/
RUN chmod +x commands.sh

ENTRYPOINT ["./commands.sh"]