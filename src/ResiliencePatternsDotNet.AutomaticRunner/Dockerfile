#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/ResiliencePatternsDotNet.AutomaticRunner/ResiliencePatternsDotNet.AutomaticRunner.csproj", "src/ResiliencePatternsDotNet.AutomaticRunner/"]
RUN dotnet restore "src/ResiliencePatternsDotNet.AutomaticRunner/ResiliencePatternsDotNet.AutomaticRunner.csproj"
COPY . .
WORKDIR "/src/src/ResiliencePatternsDotNet.AutomaticRunner"
RUN dotnet build "ResiliencePatternsDotNet.AutomaticRunner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ResiliencePatternsDotNet.AutomaticRunner.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResiliencePatternsDotNet.AutomaticRunner.dll"]
