FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
RUN apt-get update && \
    apt-get install -y ffmpeg python3 python3-pip 

#Debian's version of yt-dlp is ancient. Download it from Github
RUN apt-get install -y --no-install-recommends ca-certificates curl && \
    curl -L https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp \
      -o /usr/local/bin/yt-dlp && \
    chmod a+rx /usr/local/bin/yt-dlp && \
    rm -rf /var/lib/apt/lists/*

USER app
WORKDIR /app

#http
EXPOSE 8080
#https
EXPOSE 8081

ENV OutputPath="/Downloads"

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy project files first for better layer caching and to ensure project references are present during restore
# Also copy NuGet.config and local ThirdParty source so restore can resolve packages
COPY ["NuGet.config", "."]
COPY ["ThirdParty/", "ThirdParty/"]
COPY ["Mp3Tagger.Web/Mp3Tagger.Web.csproj", "Mp3Tagger.Web/"]
COPY ["Mp3Tagger.Core/Mp3Tagger.Core.csproj", "Mp3Tagger.Core/"]
RUN dotnet restore "Mp3Tagger.Web/Mp3Tagger.Web.csproj"
# Copy the rest of the source
COPY . .
WORKDIR "/src/Mp3Tagger.Web"
RUN dotnet build "Mp3Tagger.Web.csproj" -c ${BUILD_CONFIGURATION} -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Mp3Tagger.Web.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

VOLUME /Downloads

ENTRYPOINT ["dotnet", "Mp3Tagger.Web.dll"]
