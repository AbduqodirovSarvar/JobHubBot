FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

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

# Copy SSL certificates to the app directory
COPY ./certificates/fullchain.pem .
COPY ./certificates/privkey.pem .

ENTRYPOINT ["dotnet", "JobHubBot.dll"]
