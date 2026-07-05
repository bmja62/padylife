import { defineRailway, github, preserve, project, service, volume, postgres} from "railway/iac";

export default defineRailway((ctx) => {
  const prod = ctx.environment === "production";
  const branch = "main";

  
  const PostgresWTEN = postgres("Postgres-WTEN");
  const postgresVolume = volume("postgres-volume", { alerts: { usage: { "100": {}, "80": {}, "95": {} } }, allowOnlineResize: true, region: "sfo", sizeMB: 500 });
  const postgresVolumeJ38W = volume("postgres-volume-j38W", { alerts: { usage: { "100": {}, "80": {}, "95": {} } }, allowOnlineResize: true, region: "sfo", sizeMB: 5000 });
 
  
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
      ConnectionStrings__PostgreSQL: preserve(),
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
        resources: [api, app, admin, www, PostgresWTEN, postgresVolume, postgresVolumeJ38W]
  });
});
