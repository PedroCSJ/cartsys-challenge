FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Api/Cartsys.Api.csproj", "src/Api/"]
COPY ["src/Application/Cartsys.Application.csproj", "src/Application/"]
COPY ["src/Domain/Cartsys.Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Cartsys.Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Api/Cartsys.Api.csproj"

COPY . .
RUN dotnet publish "src/Api/Cartsys.Api.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Cartsys.Api.dll"]
