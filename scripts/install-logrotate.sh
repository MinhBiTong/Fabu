#!/usr/bin/env bash
set -Eeuo pipefail

sudo tee /etc/logrotate.d/fabu >/dev/null <<'LOGROTATE'
/opt/fabu/logs/**/*.log /opt/fabu/logs/nginx/*.log {
    daily
    rotate 14
    compress
    missingok
    notifempty
    copytruncate
}
LOGROTATE

sudo logrotate -d /etc/logrotate.d/fabu
echo "Logrotate config installed at /etc/logrotate.d/fabu"
