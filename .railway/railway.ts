import { defineRailway, github, postgres, project, service, volume } from "railway/iac";

export default defineRailway((ctx) => {
  const prod = ctx.environment === "production";
  const branch = "main";

  const Postgres = postgres("Postgres");
  const postgresVolume = volume("postgres-volume", { alerts: { usage: { "100": {}, "80": {}, "95": {} } }, allowOnlineResize: true, region: "sfo", sizeMB: 500 });
  const api = service("api", {
    source: github("bmja62/padylife", { branch, rootDirectory: "." }),
    build: {
      builder: "DOCKERFILE",
      dockerfilePath: ".deploy/api/Dockerfile",
    },
    start: "ASPNETCORE_URLS=http://0.0.0.0:$PORT dotnet PadyLife.Api.dll",
    healthcheck: "/swagger/index.html",
    healthcheckTimeout: 300,
    replicas: 1,
    env: {
      ASPNETCORE_ENVIRONMENT: ctx.environment,
      ConnectionStrings__PostgreSQL: Postgres.env.DATABASE_URL,
    },
  });
  const app = service("app", {
    source: github("bmja62/padylife", { branch, rootDirectory: "app" }),
    build: "pnpm run generate",
    start: "npx --yes serve@14 -s .output/public -l $PORT",
    replicas: 1,
    env: {
      NUXT_PUBLIC_API_ADDRESS: prod ? "https://api.padylife.ir" : `https://${api.env.RAILWAY_PUBLIC_DOMAIN}`,
    },
  });
  const admin = service("admin", {
    source: github("bmja62/padylife", { branch, rootDirectory: "admin" }),
    build: "yarn build",
    start: "npx --yes serve@14 -s dist -l $PORT",
    replicas: 1,
    env: {
      VITE_BASE_API_URL: prod ? "https://api.padylife.ir" : `https://${api.env.RAILWAY_PUBLIC_DOMAIN}`,
    },
  });

  return project("padylife", {
    resources: [Postgres, api, app, admin, postgresVolume],
  });
});
