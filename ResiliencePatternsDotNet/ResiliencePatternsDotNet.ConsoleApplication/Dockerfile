#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ResiliencePatternsDotNet.ConsoleApplication/ResiliencePatternsDotNet.ConsoleApplication.csproj", "ResiliencePatternsDotNet.ConsoleApplication/"]
RUN dotnet restore "ResiliencePatternsDotNet.ConsoleApplication/ResiliencePatternsDotNet.ConsoleApplication.csproj"
COPY . .
WORKDIR "/src/ResiliencePatternsDotNet.ConsoleApplication"
RUN dotnet build "ResiliencePatternsDotNet.ConsoleApplication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ResiliencePatternsDotNet.ConsoleApplication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResiliencePatternsDotNet.ConsoleApplication.dll"]