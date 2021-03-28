#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY "person.csproj" .
RUN dotnet restore "person.csproj" -r linux-musl-arm64


COPY . .
RUN chmod -Rf +x /root/.nuget/packages/grpc.tools//2.36.1/tools
RUN dotnet build "person.csproj" -c Release -o /app/build -r linux-musl-arm64

FROM build AS publish
RUN dotnet publish "person.csproj" -c Release -o /app/publish -r linux-musl-arm64

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim-arm64v8
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "person.dll"]