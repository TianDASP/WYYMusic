#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WYYMusic.Admin.WebApi/WYYMusic.Admin.WebApi.csproj", "WYYMusic.Admin.WebApi/"]
RUN dotnet restore "WYYMusic.Admin.WebApi/WYYMusic.Admin.WebApi.csproj"
COPY . .
WORKDIR "/src/WYYMusic.Admin.WebApi"
RUN dotnet build "WYYMusic.Admin.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WYYMusic.Admin.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WYYMusic.Admin.WebApi.dll"]