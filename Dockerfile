# ASP.NET Core minimal API on .NET 10 (LTS).
# The aspnet image ships a non-root "app" user. Program.cs reads PORT (default
# 8080) and binds 0.0.0.0, so EXPOSE 8080 matches APP_PORT in ci.yml.

# ---- build ------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Restore from the project file and lock file alone so this layer caches between code edits.
COPY global.json Directory.Build.props ./
COPY src/LearnCSharp/LearnCSharp.csproj src/LearnCSharp/packages.lock.json src/LearnCSharp/
RUN dotnet restore src/LearnCSharp/LearnCSharp.csproj --locked-mode
COPY src/ src/
RUN dotnet publish src/LearnCSharp/LearnCSharp.csproj -c Release -o /out --no-restore

# ---- runtime ----------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /out ./
ENV PORT=8080
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "LearnCSharp.dll"]
