FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["organization_back_end.csproj", "./"]
RUN dotnet restore "organization_back_end.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "organization_back_end.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "organization_back_end.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY aspnet-dev1.pfx /app/aspnet-dev1.pfx
ENV ASPNETCORE_URLS="https://+:8081;http://+:8080"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="./aspnet-dev1.pfx"
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="kath"
CMD ["dotnet", "organization_back_end.dll"]
