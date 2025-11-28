FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Bookstore.Api/Bookstore.Api.csproj Bookstore.Api/
COPY Bookstore.Infrastructure/Bookstore.Infrastructure.csproj Bookstore.Infrastructure/
COPY Bookstore.Domain/Bookstore.Domain.csproj Bookstore.Domain/
COPY Bookstore.Appication/Bookstore.Appication.csproj Bookstore.Appication/

RUN dotnet restore Bookstore.Api/Bookstore.Api.csproj

COPY . .
RUN dotnet publish Bookstore.Api/Bookstore.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080 8081
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Bookstore.Api.dll"]
