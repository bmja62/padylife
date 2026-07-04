FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY api/. ./api/
WORKDIR /src/api
RUN dotnet restore ./PadyLife.sln
RUN dotnet publish ./MyApi/PadyLife.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} dotnet PadyLife.Api.dll"]
