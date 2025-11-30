# MP3Tagger

MP3Tagger downloads audio from supported sources (via yt-dlp), converts to MP3 using ffmpeg, embeds metadata (title, artist, cover), and saves the result to a configurable output path. It ships as a web app (ASP.NET Core, .NET 9) and a Docker image.

This README focuses on how to run it with Docker, especially the environment variables and volumes you can configure.

- Docker image: built from `Mp3Tagger.Web/Dockerfile`
- Default HTTP port: 8080 (inside container)
- Default Downloads folder in the container: `/Downloads`

## Quick start (Docker)

Pull and run the latest image you have built and pushed (replace `youruser/mp3tagger` and version `1.2.3` as appropriate):

- Create a host folder for downloads, e.g., `C:\mp3-output` on Windows or `/home/user/mp3-output` on Linux.

Example (Linux/macOS):

```
docker run --rm -p 8080:8080 \
  -e ConnectionStrings__Seq=http://localhost:5341 \
  -e OutputPath=/Downloads \
  -v $(pwd)/mp3-output:/Downloads \
  youruser/mp3tagger:1.2.3
```

Example (Windows PowerShell):

```
docker run --rm -p 8080:8080 \
  -e ConnectionStrings__Seq=http://host.docker.internal:5341 \
  -e OutputPath=/Downloads \
  -v C:\mp3-output:/Downloads \
  youruser/mp3tagger:1.2.3
```

Notes:
- If you are not running Seq, you can omit `ConnectionStrings__Seq`.
- `OutputPath` defaults to `/Downloads`; you may omit it if you mount your volume to `/Downloads`.

## Docker Compose

A sample `compose.yaml` is included and sets up Seq (for structured logs) and the web app:

```
services:
  seq:
    image: datalust/seq:latest
    environment:
      - ACCEPT_EULA=Y
      - SEQ_FIRSTRUN_NOAUTHENTICATION=true
    ports:
      - "8082:80"    # Seq UI
      - "5341:5341"  # Ingestion API
    volumes:
      - seq-data:/data

  mp3tagger.web:
    build:
      context: .
      dockerfile: Mp3Tagger.Web/Dockerfile
    depends_on:
      - seq
    environment:
      - ConnectionStrings__Seq=http://seq:5341
    ports:
      - "8080:8080"
    volumes:
      - mp3tagger-downloads:/Downloads

volumes:
  seq-data:
  mp3tagger-downloads:
```

Start with:

```
docker compose up --build
```

Open the app at http://localhost:8080 and Seq at http://localhost:8082.

## Environment variables

The application uses standard .NET configuration binding; keys can be provided via environment variables, appsettings, or command-line. When using environment variables, double underscores `__` indicate nesting.

- OutputPath
  - Type: string
  - Default: `/Downloads` (set in the Dockerfile and also defaults to a `Downloads` folder next to the app when running outside Docker)
  - Description: Root folder where the final MP3 files (and temp subfolders per download) are written. Must be writable by the process. If it does not exist, the app creates it at startup.
  - Example: `-e OutputPath=/Downloads` (usually not needed if you mount your volume to `/Downloads`).

- ConnectionStrings__Seq
  - Type: string (URL)
  - Default: not set
  - Description: HTTP ingestion endpoint for Seq. When set, the app writes structured logs to Seq using this connection string. In Docker Compose, this is typically `http://seq:5341`.
  - Examples:
    - With Docker Compose: `-e ConnectionStrings__Seq=http://seq:5341`
    - Standalone Docker on Linux/macOS: `-e ConnectionStrings__Seq=http://localhost:5341`
    - Standalone Docker on Windows: `-e ConnectionStrings__Seq=http://host.docker.internal:5341`

- YtdlpLocation
  - Type: string (path to yt-dlp executable)
  - Default: `yt-dlp`
  - Description: Override the path to the yt-dlp executable. Inside the official image, yt-dlp is installed into the PATH, so this is rarely needed.

- FfmpegLocation
  - Type: string (path to ffmpeg executable)
  - Default: not set (ffmpeg is in PATH in the container)
  - Description: If you run outside the provided Docker image, you can use this to point yt-dlp to a custom ffmpeg location. When set, the app passes `--ffmpeg-location` to yt-dlp.

Environment variable casing: .NET configuration keys are case-insensitive, but examples here use the exact casing shown above for clarity.

## Volumes and data persistence

To retain downloaded MP3s across container restarts, mount a volume to the app’s output directory. The image declares a writable volume at `/Downloads` and the app defaults to writing to this path.

- Container path: `/Downloads`
- Typical host mapping (Linux/macOS): `-v $(pwd)/mp3-output:/Downloads`
- Typical host mapping (Windows PowerShell): `-v C:\mp3-output:/Downloads`

In Docker Compose, this is expressed as:

```
volumes:
  - mp3tagger-downloads:/Downloads
```

You can also bind a host folder instead of a named volume:

```
volumes:
  - ./mp3-output:/Downloads
```

Make sure the mapped host directory is writable by Docker. The application creates subfolders per download inside the output folder and logs the output path on startup.

## Ports

- 8080/tcp: HTTP endpoint for the web app (exposed by default).
- 8081/tcp: HTTPS endpoint is exposed in the image but not mapped by default in compose. If you need HTTPS, map it explicitly, e.g., `-p 8081:8081` and configure certificates as needed.

## Building from source

- .NET SDK 10.0 required.
- Build: `dotnet build Mp3Tagger.sln -c Release`
- Test: `dotnet test Mp3Tagger.sln -c Release`
- Docker local build: `docker build -t youruser/mp3tagger:dev -f Mp3Tagger.Web/Dockerfile .`

## Releases and tags

The GitHub Actions pipeline builds and pushes the Docker image when a tag is pushed (via CI -> Release). Tags must follow the format `X.Y.Z` (e.g., `1.2.3`). The Docker image is tagged with both the version (e.g., `1.2.3`) and `latest`.

Configure these in your GitHub repository to enable pushing to Docker Hub:
- Variable: `DOCKERHUB_REPO` (e.g., `youruser/mp3tagger`)
- Secrets: `DOCKERHUB_USERNAME`, `DOCKERHUB_TOKEN`
