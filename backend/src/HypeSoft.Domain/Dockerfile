FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HypeSoft.Domain/HypeSoft.Domain.csproj", "HypeSoft.Domain/"]
RUN dotnet restore "HypeSoft.Domain/HypeSoft.Domain.csproj"
COPY . .
WORKDIR "/src/HypeSoft.Domain"
RUN dotnet build "./HypeSoft.Domain.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HypeSoft.Domain.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HypeSoft.Domain.dll"]
