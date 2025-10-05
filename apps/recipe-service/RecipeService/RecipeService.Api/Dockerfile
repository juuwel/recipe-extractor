FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RecipeService.Api/RecipeService.Api.csproj", "RecipeService.Api/"]
COPY ["RecipeService.Infrastructure/RecipeService.Infrastructure.csproj", "RecipeService.Infrastructure/"]
COPY ["RecipeService.Domain/RecipeService.Domain.csproj", "RecipeService.Domain/"]
COPY ["RecipeService.Core/RecipeService.Core.csproj", "RecipeService.Core/"]
RUN dotnet restore "RecipeService.Api/RecipeService.Api.csproj"
COPY . .
WORKDIR "/src/RecipeService.Api"
RUN dotnet build "./RecipeService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RecipeService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecipeService.Api.dll"]
