# PadyLife Monorepo

This repository contains multiple deployable services:

- `api` (ASP.NET Core 8 Web API)
- `app` (Nuxt 3 client)
- `admin` (Vue 3 + Vite admin panel)
- `www` (padylife.ir homepage static site)

## Railway Deployment

This repo is prepared for Railway with Dockerfiles in each deployable project:

- `api/Dockerfile`
- `app/Dockerfile`
- `admin/Dockerfile`
- `www/Dockerfile`

## Branch and Environment Strategy

Use this branch mapping in Railway environments:

- `mainbranch` -> `staging`
- `main` -> `production`

Auto deploy should stay disabled because you want to deploy manually.

## 1. Create Railway Project and Environments

1. Create a Railway project (for example: `padylife`).
2. Create two environments in Railway:
	- `staging`
	- `production`
3. In each service, open Settings -> Deploy and disable Auto Deploy.

## 2. Create Services from This Monorepo

Create four Railway services in the same project:

1. API service
	- Root Directory: `api`
	- Dockerfile: `Dockerfile` (inside `api`)
2. App service
	- Root Directory: `app`
	- Dockerfile: `Dockerfile` (inside `app`)
3. Admin service
	- Root Directory: `admin`
	- Dockerfile: `Dockerfile` (inside `admin`)
4. Homepage service
	- Root Directory: `www`
	- Dockerfile: `Dockerfile` (inside `www`)

## 3. Configure Branch per Environment

For each Railway service:

If `mainbranch` does not exist yet, create it from `main` and push it first.
1. Open `staging` environment and set Source Branch to `mainbranch`.
2. Open `production` environment and set Source Branch to `main`.

## 4. Required Environment Variables

Set variables per service and per environment.

### API (`api` service)

Required:

- `ASPNETCORE_ENVIRONMENT` = `staging` (in staging environment)
- `ASPNETCORE_ENVIRONMENT` = `production` (in production environment)

Recommended to override using Railway secrets:

- `ConnectionStrings__PostgreSQL`
- `SiteSettings__JwtSettings__SecretKey`
- `SiteSettings__JwtSettings__EncryptKey`
- `IpHashSalt`

Note: This API fails to boot if `ASPNETCORE_ENVIRONMENT` is missing.

### Nuxt App (`app` service)

- `NUXT_PUBLIC_API_ADDRESS` (example: `https://<api-domain>`)
- `NUXT_PUBLIC_SEQ_ADDR` (optional)
- `NUXT_PUBLIC_SEQ_API_KEY` (optional)
- `NUXT_PUBLIC_GOOGLE_CLIENT_ID` (optional)
- `ENABLE_IPX` (`true` or `false`, optional)

### Admin (`admin` service)

- `VITE_BASE_API_URL` (example: `https://<api-domain>`)

### Homepage (`www` service)

- No required environment variables.

## 5. Manual Deploy Flow

You will deploy manually in Railway:

1. Push branch updates to GitHub.
2. Open Railway project.
3. Select target environment (`staging` or `production`).
4. For each service, click Deploy -> Deploy Latest Commit.

## 6. Domain Wiring

After first successful deploy:

1. Add a public domain for each service.
2. Update frontend variables so both `app` and `admin` point to the API domain.
3. Redeploy `app` and `admin` after variable changes.

## Notes

- Dockerfiles use Railway `PORT` automatically.
- Frontends are built as static assets and served with `serve`.
- Homepage is served as static files with `serve` from `www/src`.
- API listens on Railway-assigned port and runs with `dotnet PadyLife.Api.dll`.
