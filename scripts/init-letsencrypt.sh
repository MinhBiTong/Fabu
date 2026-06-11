#!/usr/bin/env bash
set -Eeuo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="${ENV_FILE:-${ROOT_DIR}/.env.production}"

set -a
# shellcheck disable=SC1090
source "${ENV_FILE}"
set +a

cd "${ROOT_DIR}"

if [[ -z "${APP_DOMAIN}" || -z "${API_DOMAIN}" || -z "${LETSENCRYPT_EMAIL}" ]]; then
  echo "APP_DOMAIN, API_DOMAIN, and LETSENCRYPT_EMAIL must be set in ${ENV_FILE}" >&2
  exit 1
fi

CERT_DIR="${ROOT_DIR}/certbot/conf/live/${APP_DOMAIN}"
mkdir -p "${CERT_DIR}" "${ROOT_DIR}/certbot/www"

if [[ ! -f "${CERT_DIR}/fullchain.pem" || ! -f "${CERT_DIR}/privkey.pem" ]]; then
  echo "Creating temporary self-signed certificate so nginx can start..."
  openssl req -x509 -nodes -newkey rsa:2048 -days 1 \
    -keyout "${CERT_DIR}/privkey.pem" \
    -out "${CERT_DIR}/fullchain.pem" \
    -subj "/CN=${APP_DOMAIN}"
fi

echo "Starting stack with temporary certificate..."
docker compose --env-file "${ENV_FILE}" up -d nginx

echo "Requesting Let's Encrypt certificate for ${APP_DOMAIN} and ${API_DOMAIN}..."
docker compose --env-file "${ENV_FILE}" run --rm certbot \
  certonly \
  --webroot \
  --webroot-path=/var/www/certbot \
  --email "${LETSENCRYPT_EMAIL}" \
  --agree-tos \
  --no-eff-email \
  --force-renewal \
  -d "${APP_DOMAIN}" \
  -d "${API_DOMAIN}"

echo "Reloading nginx with real certificate..."
docker compose --env-file "${ENV_FILE}" exec nginx nginx -s reload

echo "Let's Encrypt initialization completed."
