#!/usr/bin/env bash
set -Eeuo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="${ENV_FILE:-${ROOT_DIR}/.env.production}"

if [[ -z "${IMAGE_TAG_PREVIOUS:-}" ]]; then
  echo "Usage: IMAGE_TAG_PREVIOUS=<previous-tag> $0" >&2
  exit 1
fi

cd "${ROOT_DIR}"

echo "Rolling back FABU to image tag: ${IMAGE_TAG_PREVIOUS}"
IMAGE_TAG="${IMAGE_TAG_PREVIOUS}" docker compose --env-file "${ENV_FILE}" up -d --remove-orphans
"${ROOT_DIR}/scripts/healthcheck.sh"

echo "Rollback completed."
