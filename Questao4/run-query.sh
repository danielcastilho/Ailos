#!/bin/bash

# Carregar variáveis do arquivo postgres.env
# shellcheck disable=SC1091
source postgres.env

# Caminho do arquivo no container
QUERY_FILE_PATH="/queries/query.sql"

# Verifica se o arquivo existe no host antes de executar
if [[ ! -f "queries/query.sql" ]]; then
  echo "Arquivo de consulta não encontrado em queries/"
  exit 1
fi

# Executar o script SQL diretamente no container
docker exec -i postgres_dev psql -U "$POSTGRES_USER" -d "$POSTGRES_DB" -f "$QUERY_FILE_PATH"
