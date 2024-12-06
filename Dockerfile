FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-noble

WORKDIR /usr/src/app

# Bundle App Source
COPY Credfeto.Notification.Bot.Server .
COPY appsettings.json .
COPY healthcheck .

RUN apt-get update && apt-get upgrade -y && apt-get install curl -y --no-install-recommends && apt-get autoremove -y && apt-get clean

EXPOSE 8080
ENTRYPOINT [ "/usr/src/app/Credfeto.Notification.Bot.Server" ]
 
# Perform a healthcheck.  note that ECS ignores this, so this is for local development
HEALTHCHECK --interval=5s --timeout=2s --retries=3 --start-period=5s CMD [ "/usr/src/app/healthcheck" ]
