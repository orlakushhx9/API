FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Inventario.Backend.csproj", "./"]
RUN dotnet restore "Inventario.Backend.csproj"
COPY . .
RUN dotnet build "Inventario.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Inventario.Backend.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configuración para Railway
ENV ASPNETCORE_URLS=http://+:${PORT}
EXPOSE ${PORT}

ENTRYPOINT ["dotnet", "Inventario.Backend.dll"] 