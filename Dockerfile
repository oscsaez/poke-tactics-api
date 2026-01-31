# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. Copy all .csproj of the solution mantaining the folders
COPY *.sln ./
COPY **/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

# 2. Restore
RUN dotnet restore

# 3. Copy rest of the code
COPY PokeTactics.Api/PokeTactics.Api.csproj PokeTactics.Api/
COPY PokeTactics.Api.Test/PokeTactics.Api.Test.csproj PokeTactics.Api.Test/
COPY PokeTactics.Contracts/PokeTactics.Contracts.csproj PokeTactics.Contracts/
COPY PokeTactics.Core/PokeTactics.Core.csproj PokeTactics.Core/
COPY PokeTactics.Infrastructure/PokeTactics.Infrastructure.csproj PokeTactics.Infrastructure/
COPY PokeTactics.Services/PokeTactics.Services.csproj PokeTactics.Services/
COPY PokeTactics.Test/PokeTactics.Test.csproj PokeTactics.Test/

# 4. Publish API project
FROM build AS publish
WORKDIR "/src/PokeTactics.Api"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Finish
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER app

ENTRYPOINT ["dotnet", "PokeTactics.Api.dll"]