# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source
EXPOSE 5000
EXPOSE 4999

# copy csproj and restore as distinct layers
COPY *.sln .
COPY *.csproj .
RUN dotnet restore -r linux-arm64

# copy everything else and build app
COPY . .
RUN dotnet publish -c release -o /app -r linux-arm64 --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim-arm64v8
WORKDIR /app
COPY --from=build /app ./


ENV ASPNETCORE_URLS="http://+:5000;http://+:4999"

ENTRYPOINT ["./person"]