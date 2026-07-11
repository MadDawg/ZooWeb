FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ZooWeb/ZooWeb.csproj .
RUN cat ZooWeb.csproj

RUN dotnet restore

COPY ZooWeb/. .
RUN dotnet publish --no-restore -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
EXPOSE 8080

WORKDIR /app

COPY --from=build /app .

USER $APP_UID

ENTRYPOINT ["./ZooWeb"]
