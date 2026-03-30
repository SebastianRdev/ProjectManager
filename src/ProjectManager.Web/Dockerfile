FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/ProjectManager.Web/ProjectManager.Web.csproj", "src/ProjectManager.Web/"]
COPY ["src/ProjectManager.Application/ProjectManager.Application.csproj", "src/ProjectManager.Application/"]
COPY ["src/ProjectManager.Domain/ProjectManager.Domain.csproj", "src/ProjectManager.Domain/"]
COPY ["src/ProjectManager.Infrastructure/ProjectManager.Infrastructure.csproj", "src/ProjectManager.Infrastructure/"]
RUN dotnet restore "src/ProjectManager.Web/ProjectManager.Web.csproj"

# Build the project
COPY . .
WORKDIR "/src/src/ProjectManager.Web"
RUN dotnet build "ProjectManager.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProjectManager.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProjectManager.Web.dll"]
