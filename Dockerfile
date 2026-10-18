# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/PortfolioBuilder.Web/PortfolioBuilder.Web.csproj src/PortfolioBuilder.Web/
RUN dotnet restore src/PortfolioBuilder.Web/PortfolioBuilder.Web.csproj

COPY src/ src/
RUN dotnet publish src/PortfolioBuilder.Web/PortfolioBuilder.Web.csproj -c Release -o /app/publish --no-restore

# Stage 2: run
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "PortfolioBuilder.Web.dll"]