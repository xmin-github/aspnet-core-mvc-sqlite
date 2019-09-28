FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["dotnetcoremvcclient.csproj", "./"]
RUN dotnet restore "./dotnetcoremvcclient.csproj"
COPY . .
RUN dotnet build "dotnetcoremvcclient.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "dotnetcoremvcclient.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "dotnetcoremvcclient.dll"]
