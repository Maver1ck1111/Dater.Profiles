FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5050
EXPOSE 5051

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Profiles.WebApi/Profiles.WebApi.csproj", "Profiles.WebApi/"]
RUN dotnet restore "./Profiles.WebApi/Profiles.WebApi.csproj"
COPY . .
WORKDIR "/src/Profiles.WebApi"
RUN dotnet build "./Profiles.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Profiles.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Development
ENV Host=localhost
ENV Port=5432
ENV DatabaseName=Profiles
ENV Username=postgres
ENV Password=1234
ENTRYPOINT ["dotnet", "Profiles.WebApi.dll"]