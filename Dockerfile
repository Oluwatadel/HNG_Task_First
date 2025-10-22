# Use the ASP.NET Core runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Create a non-root user
RUN adduser -u 1001 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Use the .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the entire String_Analyzer project directory
COPY String_Analyzer String_Analyzer/
COPY test test/
COPY StringAnalyzer.sln .

# Restore dependencies for the solution
RUN dotnet restore "StringAnalyzer.sln"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/String_Analyzer"
RUN dotnet build "String_Analyzer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Run tests
FROM build AS test
WORKDIR "/src/test"
RUN dotnet test "test.csproj" -c $BUILD_CONFIGURATION

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/String_Analyzer"
RUN dotnet publish "String_Analyzer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "String_Analyzer.dll"]