#!/usr/bin/env bash
set -Eeuo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="${ENV_FILE:-${ROOT_DIR}/.env.production}"
BACKUP_DIR="${BACKUP_DIR:-${ROOT_DIR}/backup/postgres}"

set -a
# shellcheck disable=SC1090
source "${ENV_FILE}"
set +a

mkdir -p "${BACKUP_DIR}"
cd "${ROOT_DIR}"

TIMESTAMP="$(date -u +%Y%m%dT%H%M%SZ)"
BACKUP_FILE="${BACKUP_DIR}/fabu-postgres-${TIMESTAMP}.dump"
SHA_FILE="${BACKUP_FILE}.sha256"

echo "Creating PostgreSQL backup: ${BACKUP_FILE}"
docker compose --env-file "${ENV_FILE}" exec -T postgres \
  pg_dump -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -Fc > "${BACKUP_FILE}"

sha256sum "${BACKUP_FILE}" > "${SHA_FILE}"

RETENTION_DAYS="${BACKUP_RETENTION_DAYS:-14}"
find "${BACKUP_DIR}" -type f \( -name "*.dump" -o -name "*.sha256" \) -mtime +"${RETENTION_DAYS}" -delete

echo "Backup completed: ${BACKUP_FILE}"
