FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Skjera.csproj", "./"]
RUN dotnet restore "Skjera.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Skjera.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Skjera.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Skjera.dll"]
