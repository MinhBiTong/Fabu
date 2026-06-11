# FABU Security and Disaster Recovery

## Security Classification

Critical:

- JWT secret shorter than 64 random characters.
- Production secrets committed to Git.
- Database exposed to the public internet.
- Missing backup restore test.
- Payment or telecom callback without signature verification.
- Admin endpoints without role authorization.
- Docker containers running as root when not required.

High:

- No rate limiting on auth/topup endpoints.
- Redis/RabbitMQ management ports public.
- Missing audit logs for topup and SIM activation.
- Refresh tokens stored without rotation/revocation.
- Lack of idempotency keys for topup/provider calls.
- No monitoring alert for disk usage.

Medium:

- Weak security headers.
- No log retention policy.
- No dependency vulnerability scan.
- No staging environment.
- No documented rollback procedure.

## JWT Security

- Use asymmetric signing or a long random HMAC key.
- Access token lifetime: 5-15 minutes.
- Refresh token lifetime: 7-30 days.
- Rotate refresh tokens on use.
- Store token hashes if refresh tokens are persisted.
- Set cookies `Secure`, `HttpOnly`, and an appropriate `SameSite`.

## Docker Security

- Run app containers as non-root.
- Do not mount Docker socket into app containers.
- Keep DB/Redis/RabbitMQ on private Docker networks.
- Bind admin tools to `127.0.0.1` only.
- Scan images before deployment.
- Pin major image versions and upgrade on schedule.

## PostgreSQL Security

- Use strong password and SCRAM auth.
- Do not expose `5432` publicly.
- Use least-privilege app user.
- Enable daily backups and monthly restore drills.
- Consider managed PostgreSQL for larger production workloads.

## Linux Security

- Disable SSH password login.
- Enable UFW.
- Enable Fail2Ban.
- Keep unattended security upgrades enabled.
- Use a non-root deploy user.
- Rotate logs and monitor disk.

## OWASP API Controls

- Validate all request DTOs.
- Apply role-based authorization.
- Rate-limit login, OTP, topup, and payment callbacks.
- Use idempotency keys for topup and external provider APIs.
- Mask PII in logs.
- Verify webhook signatures.
- Return generic auth errors.

## Disaster Recovery Plan

Backup schedule:

- Daily: PostgreSQL custom dump, retained 14 days.
- Weekly: full backup copied to off-server storage, retained 8 weeks.
- Monthly: full backup copied to cold storage, retained 12 months.

Recommended RPO/RTO:

| Tier | RPO | RTO | Notes |
| --- | ---: | ---: | --- |
| Small production | 24h | 4h | Daily backup only |
| Standard telecom ops | 1h | 1h | Add WAL archiving or managed DB PITR |
| High-value production | 5-15m | 15-30m | Managed PostgreSQL, standby, automated failover |

Restore drill:

1. Provision a staging server.
2. Restore latest backup.
3. Run database migrations.
4. Start backend/frontend stack.
5. Verify login, recharge, transaction history, admin dashboard.
6. Record restore duration.

Incident steps:

1. Stop writes if data corruption is suspected.
2. Snapshot current server before changes.
3. Identify last known good backup.
4. Restore to staging first.
5. Promote restored stack only after smoke tests pass.
6. Write incident report with timeline and prevention actions.

## Deployment Checklist

- DNS points to VPS.
- Ports 80/443 open.
- `.env.production` contains real secrets.
- `JWT_KEY` rotated from template.
- PostgreSQL provider compatibility verified.
- `docker compose config` passes.
- `scripts/init-letsencrypt.sh` completed.
- `scripts/healthcheck.sh` passes.
- Backup script tested.
- Restore script tested on staging.
- Grafana reachable through SSH tunnel.
- Seq reachable through SSH tunnel.
- GitHub Actions deploy secrets configured.

## Release Checklist

- Backend build passed.
- Frontend lint/build passed.
- Docker images pushed.
- Database migration reviewed.
- Deployment window announced.
- Previous `IMAGE_TAG` recorded.
- Health check passed after deploy.
- Error rate checked in Seq.
- CPU/RAM/disk checked in Grafana.
