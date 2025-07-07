# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copiar todo el contenido del proyecto
COPY ./RepairLink-Backend/ ./RepairLink-Backend/

# Ir al directorio del proyecto
WORKDIR /src/RepairLink-Backend

# Restaurar dependencias
RUN dotnet restore

# Compilar el proyecto
RUN dotnet publish -c Release -o /app/publish

# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RepairLink-Backend.dll"]
