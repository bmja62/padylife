import { defineRailway, github, postgres, preserve, project, service, volume } from "railway/iac";

export default defineRailway(() => {
  const padylife = github("bmja62/padylife");

  const PostgresWTEN = postgres("Postgres-WTEN");
  const postgresVolume = volume("postgres-volume", { alerts: { usage: { "100": {}, "80": {}, "95": {} } }, allowOnlineResize: true, region: "sfo", sizeMB: 500 });
  const postgresVolumeJ38W = volume("postgres-volume-j38W", { alerts: { usage: { "100": {}, "80": {}, "95": {} } }, allowOnlineResize: true, region: "sfo", sizeMB: 5000 });
  const api = service("api", {
    source: padylife,
    root: ".",
    build: { buildCommand: "dotnet publish MyApi/PadyLife.Api.csproj -c Release -o publish /p:UseAppHost=false", buildEnvironment: "V3", builder: "DOCKERFILE", dockerfilePath: ".deploy/api/Dockerfile" },
    healthcheck: "/swagger/index.html",
    healthcheckTimeout: 300,
    replicas: 1,
    env: {
      ASPNETCORE_ENVIRONMENT: preserve(),
      ConnectionStrings__PostgreSQL: preserve(),
    },
  });
  const app = service("app", {
    source: padylife,
    root: "app",
    build: "pnpm run generate",
    start: "npx --yes serve@14 -s .output/public -l $PORT",
    replicas: 1,
    env: {
      NUXT_PUBLIC_API_ADDRESS: preserve(),
    },
  });
  const admin = service("admin", {
    source: padylife,
    root: "admin",
    build: "yarn build",
    start: "npx --yes serve@14 -s dist -l $PORT",
    replicas: 1,
    env: {
      VITE_BASE_API_URL: preserve(),
    },
  });
  const www = service("www", {
    source: padylife,
    root: ".",
    build: { builder: "DOCKERFILE", dockerfilePath: ".deploy/www/Dockerfile" },
    replicas: 1,
  });

  return project("padylife", {
    resources: [api, app, admin, www, PostgresWTEN, postgresVolume, postgresVolumeJ38W],
  });
});
