# Docker and Nginx Reference

## Backend Dockerfile Explanation

`# syntax=docker/dockerfile:1.7` enables modern Dockerfile features.

`FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS restore` uses the SDK only for restore/build.

`WORKDIR /src` sets the build working directory.

`COPY *.csproj ...` copies project files first so Docker can cache NuGet restore.

`RUN dotnet restore Api/greenginger.csproj` restores NuGet packages before copying all code.

`FROM restore AS build` reuses restored dependencies.

`COPY . .` copies the full backend source.

`dotnet publish --configuration Release --output /app/publish /p:UseAppHost=false` produces optimized runtime output without a native host executable.

`FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime` switches to a smaller runtime-only image.

`apk add curl icu-libs tzdata` adds healthcheck and globalization/timezone support.

`addgroup/adduser` creates a non-root runtime user.

`ASPNETCORE_ENVIRONMENT=Production` makes ASP.NET load production config.

`ASPNETCORE_URLS=http://+:8080` binds the API inside the container.

`COPY --from=build /app/publish .` copies only publish output, not source code.

`USER fabu` prevents the API from running as root.

`HEALTHCHECK` lets Docker Compose and Nginx wait for a real API health signal.

`ENTRYPOINT ["dotnet", "greenginger.dll"]` starts the published API.

## Frontend Dockerfile Explanation

`FROM node:22-alpine AS deps` installs dependencies in a separate cacheable layer.

`COPY package.json package-lock.json ./` copies lockfiles first.

`npm ci` installs exact dependency versions for reproducible builds.

`FROM node:22-alpine AS builder` builds the Next.js app.

`ARG NEXT_PUBLIC_API_URL` allows CI/Compose to inject the public API URL at build time.

`NEXT_TELEMETRY_DISABLED=1` disables Next telemetry in CI/production image builds.

`npm run build` creates `.next/standalone` because `next.config.ts` sets `output: "standalone"`.

`FROM node:22-alpine AS runner` creates the small production image.

`addgroup/adduser` creates non-root user `nextjs`.

`COPY .next/standalone`, `.next/static`, and `public` copies only runtime assets.

`HOSTNAME=0.0.0.0` makes Next listen on the container network interface.

`HEALTHCHECK` verifies that the Next server is responding.

`CMD ["node", "server.js"]` starts the standalone Next server.

## Nginx Directive Explanation

`worker_processes auto` uses available CPU cores.

`worker_connections 4096` allows many concurrent client connections.

`server_tokens off` hides Nginx version details.

`client_max_body_size 10m` limits upload/request body size.

`gzip on` compresses text, JSON, JavaScript, CSS, and SVG responses.

`limit_req_zone` defines shared memory zones for rate limiting.

`limit_conn_zone` defines per-IP connection limiting.

`upstream fabu_frontend` points to the Next.js container.

`upstream fabu_backend` points to the ASP.NET API container.

The port `80` server handles:

- `/nginx-health` for container health,
- `/.well-known/acme-challenge/` for Let's Encrypt,
- HTTP to HTTPS redirect.

The `fabu.company.com` HTTPS server proxies user traffic to Next.js.

The `api.fabu.company.com` HTTPS server proxies API traffic to ASP.NET.

`ssl_protocols TLSv1.2 TLSv1.3` disables obsolete TLS versions.

`proxy_set_header X-Forwarded-Proto https` tells ASP.NET the original request used HTTPS.

`proxy_set_header X-Forwarded-For` preserves client IP chain.

`proxy_read_timeout` protects long API requests from being cut off too aggressively.

`location ~* ^/api/v1/auth|^/v1/Auth` applies stricter auth endpoint rate limits.

`/_next/static/` receives long immutable cache headers because Next static assets are content-hashed.

Security headers are isolated in `nginx/snippets/security-headers.conf` so both frontend and API servers reuse the same policy.

## Operational Notes

Before first SSL issue, `scripts/init-letsencrypt.sh` creates a temporary self-signed certificate so Nginx can boot. Certbot then replaces it with a real Let's Encrypt certificate.

Grafana, Prometheus, Seq, and RabbitMQ management ports are bound to `127.0.0.1` only. Use SSH tunnels:

```bash
ssh -L 3001:127.0.0.1:3001 -L 9090:127.0.0.1:9090 -L 5341:127.0.0.1:5341 deploy@your-server
```
