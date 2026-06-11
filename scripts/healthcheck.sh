#!/usr/bin/env bash
set -Eeuo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="${ENV_FILE:-${ROOT_DIR}/.env.production}"

set -a
# shellcheck disable=SC1090
source "${ENV_FILE}"
set +a

cd "${ROOT_DIR}"

echo "Checking containers..."
docker compose --env-file "${ENV_FILE}" ps

echo "Checking public frontend: https://${APP_DOMAIN}"
curl -fsS --retry 5 --retry-delay 3 "https://${APP_DOMAIN}" >/dev/null

echo "Checking API health: https://${API_DOMAIN}/health"
curl -fsS --retry 5 --retry-delay 3 "https://${API_DOMAIN}/health" >/dev/null

echo "Healthcheck passed."
