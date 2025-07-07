# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el contenido del proyecto
COPY ./RepairLink-Backend/ ./RepairLink-Backend/

# Ir al directorio del proyecto
WORKDIR /src/RepairLink-Backend

# Restaurar dependencias
RUN dotnet restore

# Compilar y publicar el proyecto
RUN dotnet publish -c Release -o /app/publish

# Imagen final para producción
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Punto de entrada
ENTRYPOINT ["dotnet", "RepairLink-Backend.dll"]
