# Usar una imagen base de .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Establecer el directorio de trabajo
WORKDIR /app

# Copiar los archivos de proyecto
COPY *.csproj ./

# Restaurar las dependencias
RUN dotnet restore

# Copiar todo el código al contenedor
COPY . ./

# Publicar la aplicación en modo Release
RUN dotnet publish -c Release -o out

# Usar una imagen base de .NET Runtime para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final

# Establecer el directorio de trabajo
WORKDIR /app

# Copiar los archivos publicados desde la fase de build
COPY --from=build /app/out .

# Exponer el puerto que la aplicación usará
EXPOSE 80

# Comando para iniciar la aplicación
ENTRYPOINT ["dotnet", "RepairLink-Backend.dll"]
