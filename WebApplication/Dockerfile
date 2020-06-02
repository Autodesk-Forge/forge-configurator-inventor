FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY web/ app/
COPY AppBundles/ app/AppBundles/
WORKDIR /app

RUN wget https://s3.us-west-2.amazonaws.com/amazoncloudwatch-agent-us-west-2/debian/amd64/latest/amazon-cloudwatch-agent.deb
RUN dpkg -i -E ./amazon-cloudwatch-agent.deb
COPY amazon-cloudwatch-agent.json /opt/aws/amazon-cloudwatch-agent/etc/
RUN chmod +x commands.sh

ENTRYPOINT ["./commands.sh"]