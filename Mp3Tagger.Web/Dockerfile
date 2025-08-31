FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
RUN apt-get update && \
    apt-get install -y ffmpeg python3 python3-pip && \
    pip3 install --no-cache-dir yt-dlp
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Mp3Tagger.Web/Mp3Tagger.Web.csproj", "Mp3Tagger.Web/"]
RUN dotnet restore "Mp3Tagger.Web/Mp3Tagger.Web.csproj"
COPY . .
WORKDIR "/src/Mp3Tagger.Web"
RUN dotnet build "./Mp3Tagger.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Mp3Tagger.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mp3Tagger.Web.dll"]
