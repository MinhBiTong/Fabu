#!/usr/bin/env bash
set -Eeuo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="${ENV_FILE:-${ROOT_DIR}/.env.production}"

cd "${ROOT_DIR}"

if [[ ! -f "${ENV_FILE}" ]]; then
  echo "Missing env file: ${ENV_FILE}" >&2
  exit 1
fi

echo "Building FABU production images..."
docker compose --env-file "${ENV_FILE}" build

echo "Starting FABU production stack..."
docker compose --env-file "${ENV_FILE}" up -d --remove-orphans

echo "Waiting for services..."
sleep 10
"${ROOT_DIR}/scripts/healthcheck.sh"

echo "FABU deployment completed."
