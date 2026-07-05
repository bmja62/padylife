import { defineRailway, github, preserve, project, service, volume, postgres } from "railway/iac";

export default defineRailway((ctx) => {
  const prod = ctx.environment === "production";
  const branch = "main";

  const postgresProd = postgres("Postgres-Production");
  const postgresStaging = postgres("Postgres-Staging");
  const postgresService = prod ? postgresProd : postgresStaging;

  const postgresVolumeProd = volume("postgres-volume-production", { alerts: { usage: { "100": {}, "80": {}, "95": {} } }, allowOnlineResize: true, region: "sfo", sizeMB: 1000 });
  const postgresVolumeStaging = volume("postgres-volume-staging", { alerts: { usage: { "100": {}, "80": {}, "95": {} } }, allowOnlineResize: true, region: "sfo", sizeMB: 100 });
  const postgresVolume = prod ? postgresVolumeProd : postgresVolumeStaging;
 
  
  const api = service("api", {
    source: github("bmja62/padylife", { branch, rootDirectory: "api" }),
    domains: [prod ? "api.padylife.ir" : "staging-api.padylife.ir"],
    build: {
      buildCommand: "dotnet publish MyApi/PadyLife.Api.csproj -c Release -o publish /p:UseAppHost=false",
      buildEnvironment: "V3",
      builder: "DOCKERFILE",
      dockerfilePath: "api/Dockerfile",
    },
    healthcheck: "/swagger/index.html",
    healthcheckTimeout: 300,
    replicas: 1,
    env: {
      ASPNETCORE_ENVIRONMENT: preserve(),
      aspnetcore_url: preserve(),
      ConnectionStrings__PostgreSQL: preserve(),
    },
  });

  const app = service("app", {
    source: github("bmja62/padylife", { branch, rootDirectory: "app" }),
    domains: [prod ? "app.padylife.ir" : "staging-app.padylife.ir"],
    build:{
      ...(prod ? {} : { buildCommand: "pnpm run generate" }),
      buildEnvironment: "V3",
      builder: "DOCKERFILE",
      dockerfilePath: "app/Dockerfile",
    },
    replicas: 1,
    env: {
      NUXT_PUBLIC_API_ADDRESS: preserve(),
    },
  });
  const admin = service("admin", {
    source: github("bmja62/padylife", { branch, rootDirectory: "admin" }),
    domains: [prod ? "admin.padylife.ir" : "staging-admin.padylife.ir"],
    build:{
      ...(prod ? {} : { buildCommand: "yarn build" }),
      buildEnvironment: "V3",
      builder: "DOCKERFILE",
      dockerfilePath: "admin/Dockerfile",
    },
    replicas: 1,
    env: {
      VITE_BASE_API_URL: preserve(),
    },
  });

  const www = service("www", {
    source: github("bmja62/padylife", { branch, rootDirectory: "www" }),
    domains: [prod ? "www.padylife.ir" : "staging-www.padylife.ir"],
    build: {
      buildEnvironment: "V3",
      builder: "DOCKERFILE",
      dockerfilePath: "www/Dockerfile",
    },
    replicas: 1,
    env: {
      NUXT_PUBLIC_API_ADDRESS: preserve(),
    },
  });

  return project("padylife", {
      resources: prod
        ? [api, app, admin, postgresService, www, postgresVolume]
        : [api, app, admin, www, postgresService, postgresVolume]
  });
});
