# syntax=docker/dockerfile:1.6

############################
# Runtime base
############################
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

USER root

# OS deps
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        ffmpeg python3 python3-pip ca-certificates curl gosu && \
    rm -rf /var/lib/apt/lists/*

# Latest yt-dlp
RUN curl -L https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp \
      -o /usr/local/bin/yt-dlp && \
    chmod a+rx /usr/local/bin/yt-dlp

# Output location
ENV OutputPath=/Downloads
RUN mkdir -p "$OutputPath" && chown -R app:app "$OutputPath" && chmod 755 "$OutputPath"

# Copy entrypoint into the image
COPY entrypoint.sh /usr/local/bin/entrypoint.sh
RUN chmod +x /usr/local/bin/entrypoint.sh

# Ports
EXPOSE 8080 8081

# Declare a volume (entrypoint fixes perms when container starts)
VOLUME ["/Downloads"]

WORKDIR /app


############################
# Build stage
############################
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["NuGet.config", "."]
COPY ["ThirdParty/", "ThirdParty/"]
COPY ["Mp3Tagger.Web/Mp3Tagger.Web.csproj", "Mp3Tagger.Web/"]
COPY ["Mp3Tagger.Core/Mp3Tagger.Core.csproj", "Mp3Tagger.Core/"]

RUN dotnet restore "Mp3Tagger.Web/Mp3Tagger.Web.csproj"

COPY . .
WORKDIR "/src/Mp3Tagger.Web"
RUN dotnet build "Mp3Tagger.Web.csproj" -c ${BUILD_CONFIGURATION} -o /app/build --no-restore


############################
# Publish stage
############################
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Mp3Tagger.Web.csproj" -c ${BUILD_CONFIGURATION} \
    -o /app/publish /p:UseAppHost=false --no-restore


############################
# Final image
############################
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish ./

ENTRYPOINT ["/usr/local/bin/entrypoint.sh", "dotnet", "Mp3Tagger.Web.dll"]
