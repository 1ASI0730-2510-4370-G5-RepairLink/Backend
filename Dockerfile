# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./RepairLink-Backend/ ./RepairLink-Backend/
WORKDIR /src/RepairLink-Backend

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RepairLink-Backend.dll"]
