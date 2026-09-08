FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SafeLink.API/SafeLink.API.csproj", "SafeLink.API/"]
RUN dotnet restore "SafeLink.API/SafeLink.API.csproj"
COPY SafeLink.API/ SafeLink.API/
WORKDIR /src/SafeLink.API
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SafeLink.API.dll"]
