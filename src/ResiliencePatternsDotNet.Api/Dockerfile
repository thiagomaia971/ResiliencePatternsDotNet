#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/ResiliencePatternsDotNet.Api/ResiliencePatternsDotNet.Api.csproj", "src/ResiliencePatternsDotNet.Api/"]
COPY ["src/ResiliencePatternsDotNet.Infra/ResiliencePatternsDotNet.Infra.csproj", "src/ResiliencePatternsDotNet.Infra/"]
COPY ["src/ResiliencePatternsDotNet.Domain/ResiliencePatternsDotNet.Domain.csproj", "src/ResiliencePatternsDotNet.Domain/"]
RUN dotnet restore "src/ResiliencePatternsDotNet.Api/ResiliencePatternsDotNet.Api.csproj"
COPY . .
WORKDIR "/src/src/ResiliencePatternsDotNet.Api"
RUN dotnet build "ResiliencePatternsDotNet.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ResiliencePatternsDotNet.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResiliencePatternsDotNet.Api.dll"]