# FABU

Production deployment assets are available in:

- `docker-compose.yml`
- `.env.production.example`
- `src/backend/Dockerfile`
- `src/frontend/Dockerfile`
- `nginx/`
- `monitoring/`
- `scripts/`
- `docs/`

Start with:

1. `docs/production-architecture.md`
2. `docs/ubuntu-server-setup.md`
3. `docs/deployment-runbook.md`
4. `docs/security-disaster-recovery.md`
5. `docs/docker-nginx-reference.md`

Important: the production target is PostgreSQL, but the current backend code still uses EF Core SQL Server provider and SQL Server migrations. Read the database reality check in `docs/production-architecture.md` before deploying to PostgreSQL.
