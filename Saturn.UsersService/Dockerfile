#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Saturn.UsersService/Saturn.UsersService.csproj", "Saturn.UsersService/"]
COPY ["Saturn.CommonLibrary/Saturn.CommonLibrary.csproj", "Saturn.CommonLibrary/"]
RUN dotnet restore "Saturn.UsersService/Saturn.UsersService.csproj"
COPY . .
WORKDIR "/src/Saturn.UsersService"
RUN dotnet build "Saturn.UsersService.csproj" -c Docker -o /app/build

FROM build AS publish
RUN dotnet publish "Saturn.UsersService.csproj" -c Docker -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Saturn.UsersService.dll"]