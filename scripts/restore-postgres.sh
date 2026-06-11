#!/usr/bin/env bash
set -Eeuo pipefail

if [[ $# -ne 1 ]]; then
  echo "Usage: $0 /path/to/fabu-postgres-YYYYMMDDTHHMMSSZ.dump" >&2
  exit 1
fi

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="${ENV_FILE:-${ROOT_DIR}/.env.production}"
BACKUP_FILE="$1"

if [[ ! -f "${BACKUP_FILE}" ]]; then
  echo "Backup file not found: ${BACKUP_FILE}" >&2
  exit 1
fi

set -a
# shellcheck disable=SC1090
source "${ENV_FILE}"
set +a

cd "${ROOT_DIR}"

echo "Restoring PostgreSQL database ${POSTGRES_DB} from ${BACKUP_FILE}"
echo "This will clean existing objects before restore."
read -r -p "Type RESTORE to continue: " CONFIRM
if [[ "${CONFIRM}" != "RESTORE" ]]; then
  echo "Restore cancelled."
  exit 1
fi

docker compose --env-file "${ENV_FILE}" exec -T postgres \
  pg_restore -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" --clean --if-exists --no-owner < "${BACKUP_FILE}"

echo "Restore completed."
