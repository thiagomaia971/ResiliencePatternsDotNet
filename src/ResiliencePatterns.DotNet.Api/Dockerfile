#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/ResiliencePatterns.DotNet.Api/ResiliencePatterns.DotNet.Api.csproj", "src/ResiliencePatterns.DotNet.Api/"]
COPY ["src/ResiliencePatterns.DotNet.Infra/ResiliencePatterns.DotNet.Infra.csproj", "src/ResiliencePatterns.DotNet.Infra/"]
COPY ["src/ResiliencePatterns.DotNet.Domain/ResiliencePatterns.DotNet.Domain.csproj", "src/ResiliencePatterns.DotNet.Domain/"]
RUN dotnet restore "src/ResiliencePatterns.DotNet.Api/ResiliencePatterns.DotNet.Api.csproj"
COPY . .
WORKDIR "/src/src/ResiliencePatterns.DotNet.Api"
RUN dotnet build "ResiliencePatterns.DotNet.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ResiliencePatterns.DotNet.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResiliencePatterns.DotNet.Api.dll"]