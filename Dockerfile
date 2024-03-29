FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["JobHubBot.csproj", "."]
RUN dotnet restore "./JobHubBot.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "JobHubBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobHubBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "JobHubBot.dll"]
