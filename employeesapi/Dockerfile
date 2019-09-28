FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["employeesapi.csproj", "./"]
RUN dotnet restore "./employeesapi.csproj"
COPY . .

RUN dotnet build "employeesapi.csproj" -o /app

FROM build AS publish
RUN dotnet publish "employeesapi.csproj"  -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY ["employees.db","/app"]
ENTRYPOINT ["dotnet", "employeesapi.dll"]