import { defineRailway, github, project, service, volume, postgres } from "railway/iac";

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
    build: {
      builder: "DOCKERFILE",
      dockerfilePath: "api/Dockerfile",
    },
    healthcheck: "/swagger/index.html",
    healthcheckTimeout: 300,
    replicas: 1,
    env: {
      ASPNETCORE_ENVIRONMENT: prod ? "Production" : "Staging",
      aspnetcore_url: prod ? "https://api.padylife.ir" : "https://staging-api.padylife.ir",
      ConnectionStrings__PostgreSQL: postgresService.env.DATABASE_URL,
    },
  });

  const app = service("app", {
    source: github("bmja62/padylife", { branch, rootDirectory: "app" }),
    build:{
      builder: "DOCKERFILE",
      dockerfilePath: "app/Dockerfile",
    },
    replicas: 1,
    env: {
      NUXT_PUBLIC_API_ADDRESS: prod ? "https://app.padylife.ir" : "https://staging-app.padylife.ir",
    },
  });
  const admin = service("admin", {
    source: github("bmja62/padylife", { branch, rootDirectory: "admin" }),
    build:{
      builder: "DOCKERFILE",
      dockerfilePath: "admin/Dockerfile",
    },
    replicas: 1,
    env: {
      VITE_BASE_API_URL: prod ? "https://admin.padylife.ir" : "https://staging-admin.padylife.ir",
    },
  });

  const www = service("www", {
    source: github("bmja62/padylife", { branch, rootDirectory: "www" }),
    build: {
      builder: "DOCKERFILE",
      dockerfilePath: "www/Dockerfile",
    },
    replicas: 1,
    env: {
      NUXT_PUBLIC_API_ADDRESS: prod ? "https://www.padylife.ir" : "https://staging-www.padylife.ir",
    },
  });

  return project("padylife", {
      resources: [api, app, admin, www, postgresService, postgresVolume]
  });
});
