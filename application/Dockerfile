# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar arquivos de projeto e restaurar dependências
COPY ["DesafioBackend.sln", "./"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Presentation/Presentation.csproj", "src/Presentation/"]
RUN dotnet restore

# Copiar todo o código e publicar a aplicação
COPY . .
RUN dotnet publish "src/Presentation/Presentation.csproj" -c Release -o /app/out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS runtime
WORKDIR /app

# Instalar dependências no runtime
RUN apt-get update && apt-get install -y --no-install-recommends \
    libssl-dev \
    && rm -rf /var/lib/apt/lists/*

# Instalar a ferramenta dotnet-ef para migrações
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copiar a saída do build
COPY --from=build /app/out ./

# Copiar os arquivos do projeto para rodar migrações
COPY --from=build /app/src /app/src
COPY --from=build /app/DesafioBackend.sln /app/DesafioBackend.sln

# Executar as migrações e iniciar a aplicação
ENTRYPOINT ["sh", "-c", "dotnet ef database update --project src/Infrastructure/Infrastructure.csproj --startup-project src/Presentation/Presentation.csproj && dotnet Presentation.dll"]
