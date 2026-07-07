import { defineRailway, github, project, service, volume, postgres } from "railway/iac";

export default defineRailway((ctx) => {
  const prod = ctx.environment === "production";
  const branch = "main";
  const watchPattern = prod ? ["/__never_trigger_deploy__/**"] : undefined;

  const postgresProd = postgres("Postgres-Production");
  const postgresStaging = postgres("Postgres-Staging");
  const postgresService = prod ? postgresProd : postgresStaging;
  
  const postgresResourceName = postgresService.name;
  const postgresVarRef = (key: string) => `\$\{\{${postgresResourceName}.${key}\}\}`;
  const postgresNpgsqlConnectionString = `Host=${postgresVarRef("PGHOST")};Port=${postgresVarRef("PGPORT")};Database=${postgresVarRef("PGDATABASE")};Username=${postgresVarRef("PGUSER")};Password=${postgresVarRef("PGPASSWORD")};`;
  
  const postgresVolumeProd = volume("postgres-volume-production", { alerts: { usage: { "100": {}, "80": {}, "95": {} } }, allowOnlineResize: true, region: "sfo", sizeMB: 1000 });
  const postgresVolumeStaging = volume("postgres-volume-staging", { alerts: { usage: { "100": {}, "80": {}, "95": {} } }, allowOnlineResize: true, region: "sfo", sizeMB: 100 });
  const postgresVolume = prod ? postgresVolumeProd : postgresVolumeStaging;
 
  
  const api = service("api", {
    source: github("bmja62/padylife", { branch, rootDirectory: "api" }),
    domains: [prod ? "api.padylife.ir" : "staging-api.padylife.ir"],
    env: {
      ConnectionStrings__PostgreSQL: postgresNpgsqlConnectionString,
    },
    build: {
      buildEnvironment: "V3",
      builder: "DOCKERFILE",
      dockerfilePath: "api/Dockerfile",
      watchPatterns: watchPattern,
    },
    healthcheck: "/swagger/index.html",
    healthcheckTimeout: 300,
    sleepApplication: true,
    replicas: 1,
  });

  const app = service("app", {
    source: github("bmja62/padylife", { branch, rootDirectory: "app" }),
    domains: [prod ? "app.padylife.ir" : "staging-app.padylife.ir"],
    build:{
      buildEnvironment: "V3",
      builder: "DOCKERFILE",
      dockerfilePath: "app/Dockerfile",
      watchPatterns: watchPattern,
    },
    sleepApplication: true,
    replicas: 1,
  });
  const admin = service("admin", {
    source: github("bmja62/padylife", { branch, rootDirectory: "admin" }),
    domains: [prod ? "admin.padylife.ir" : "staging-admin.padylife.ir"],
    build:{
      buildEnvironment: "V3",
      builder: "DOCKERFILE",
      dockerfilePath: "admin/Dockerfile",
      watchPatterns: watchPattern,
    },
    sleepApplication: true,
    replicas: 1,
  });

  const www = service("www", {
    source: github("bmja62/padylife", { branch, rootDirectory: "www" }),
    domains: [prod ? "www.padylife.ir" : "staging-www.padylife.ir"],
    build: {
      buildEnvironment: "V3",
      builder: "DOCKERFILE",
      dockerfilePath: "www/Dockerfile",
      watchPatterns: watchPattern,
    },
    sleepApplication: true,
    replicas: 1,
  });

  return project("padylife", {resources: [api, app, admin, postgresService, www, postgresVolume]});
});
