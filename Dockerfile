# Etapa usada para compilar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copia primeiro os projetos para restaurar as dependências
COPY ["SistemaDeCadastro/SistemaDeCadastro.csproj", "SistemaDeCadastro/"]
COPY ["SistemaDeCadastro.APP/SistemaDeCadastro.APP.csproj", "SistemaDeCadastro.APP/"]
COPY ["SistemaDeCadastro.Domain/SistemaDeCadastro.Domain.csproj", "SistemaDeCadastro.Domain/"]
COPY ["SistemaDeCadastro.Infra/SistemaDeCadastro.Infra.csproj", "SistemaDeCadastro.Infra/"]

RUN dotnet restore "SistemaDeCadastro/SistemaDeCadastro.csproj"

# Copia o restante do código
COPY . .

# Gera os arquivos de publicação
RUN dotnet publish "SistemaDeCadastro/SistemaDeCadastro.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Etapa que executará a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

# Porta esperada pelo Render
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

ENTRYPOINT ["dotnet", "SistemaDeCadastro.dll"]