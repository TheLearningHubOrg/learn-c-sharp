# Multi-stage Dockerfile for C# / .NET 8 Console Application
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS builder

WORKDIR /src

COPY LearnCSharp.sln ./
COPY src/LearnCSharp/*.csproj src/LearnCSharp/
COPY tests/LearnCSharp.Tests/*.csproj tests/LearnCSharp.Tests/
RUN dotnet restore LearnCSharp.sln

COPY . .
RUN dotnet build -c Release LearnCSharp.sln
RUN dotnet test -c Release --no-build
RUN dotnet publish -c Release -o /app/publish src/LearnCSharp/LearnCSharp.csproj

FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS runner
WORKDIR /app

COPY --from=builder /app/publish .

ENTRYPOINT ["dotnet", "LearnCSharp.dll"]
