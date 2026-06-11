# Ubuntu 24.04 Production Server Setup

Run these commands as a sudo-capable user on a fresh Ubuntu 24.04 VPS.

## 1. Create Deploy User

```bash
adduser deploy
usermod -aG sudo deploy
usermod -aG docker deploy || true
```

Copy SSH key:

```bash
mkdir -p /home/deploy/.ssh
nano /home/deploy/.ssh/authorized_keys
chown -R deploy:deploy /home/deploy/.ssh
chmod 700 /home/deploy/.ssh
chmod 600 /home/deploy/.ssh/authorized_keys
```

## 2. Harden SSH

```bash
sudo cp /etc/ssh/sshd_config /etc/ssh/sshd_config.bak
sudo nano /etc/ssh/sshd_config
```

Recommended values:

```text
PermitRootLogin no
PasswordAuthentication no
PubkeyAuthentication yes
MaxAuthTries 3
ClientAliveInterval 300
ClientAliveCountMax 2
```

Restart SSH:

```bash
sudo systemctl restart ssh
```

## 3. UFW Firewall

```bash
sudo apt update
sudo apt install -y ufw
sudo ufw default deny incoming
sudo ufw default allow outgoing
sudo ufw allow OpenSSH
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw enable
sudo ufw status verbose
```

Monitoring ports are bound to `127.0.0.1` in `docker-compose.yml`. Access Grafana, Prometheus, RabbitMQ, and Seq through an SSH tunnel.

## 4. Fail2Ban

```bash
sudo apt install -y fail2ban
sudo tee /etc/fail2ban/jail.local >/dev/null <<'EOF'
[sshd]
enabled = true
port = ssh
filter = sshd
logpath = /var/log/auth.log
maxretry = 3
bantime = 1h
findtime = 10m
EOF
sudo systemctl enable --now fail2ban
sudo fail2ban-client status sshd
```

## 5. Docker and Compose

```bash
sudo apt install -y ca-certificates curl gnupg
sudo install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
sudo chmod a+r /etc/apt/keyrings/docker.gpg
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list >/dev/null
sudo apt update
sudo apt install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
sudo systemctl enable --now docker
sudo usermod -aG docker deploy
```

Log out and log in again so the `deploy` user receives Docker group membership.

## 6. NTP and Timezone

```bash
sudo timedatectl set-timezone Asia/Ho_Chi_Minh
sudo timedatectl set-ntp true
timedatectl
```

## 7. Directory Layout

```bash
sudo mkdir -p /opt/fabu
sudo chown -R deploy:deploy /opt/fabu
cd /opt/fabu
```

The production repo layout should be:

```text
/opt/fabu
├── src/backend
├── src/frontend
├── nginx
├── monitoring
├── backup
├── logs
├── scripts
├── docker-compose.yml
└── .env.production
```

## 8. Log Rotation

```bash
cd /opt/fabu
chmod +x scripts/install-logrotate.sh
./scripts/install-logrotate.sh
```

## 9. Cron Jobs

PostgreSQL daily backup:

```bash
crontab -e
```

Add:

```cron
15 2 * * * cd /opt/fabu && /opt/fabu/scripts/backup-postgres.sh >> /opt/fabu/logs/backup.log 2>&1
30 3 * * 0 cd /opt/fabu && /opt/fabu/scripts/renew-letsencrypt.sh >> /opt/fabu/logs/certbot-renew.log 2>&1
```
