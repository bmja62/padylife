import { defineRailway, github, project, service, postgres, volume } from "railway/iac";

export default defineRailway((ctx) => {
  const prod = ctx.environment === "production";
  const branch = prod ? "release" : "main";
  
  const postgresVolume = volume("postgres-data");
  const postgresService = postgres("Postgres");
  postgresService.deploy = {
    sleepApplication: true,
  };
  postgresService.volumeMounts = {
    "postgres-data": {
      mountPath: "/var/lib/postgresql/data",
    },
  };

  const api = service("api", {
    source: github("bmja62/padylife", { branch, rootDirectory: "api" }),
    domains: [prod ? "api.padylife.ir" : "staging-api.padylife.ir"],
    env: {
      ConnectionStrings__PostgreSQL: "Host=${{postgres.PGHOST}};Port=${{postgres.PGPORT}};Database=${{postgres.PGDATABASE}};Username=${{postgres.PGUSER}};Password=${{postgres.PGPASSWORD}}",
    },
    build: {
      buildEnvironment: "V3",
      builder: "DOCKERFILE",
      dockerfilePath: "api/Dockerfile",
    },
    healthcheck: "/swagger/index.html",
    healthcheckTimeout: 300,
    deploy: {
      sleepApplication: true,
    },
    replicas: 1,
  });

  const app = service("app", {
    source: github("bmja62/padylife", { branch, rootDirectory: "app" }),
    domains: [prod ? "app.padylife.ir" : "staging-app.padylife.ir"],
    build:{
      buildEnvironment: "V3",
      builder: "DOCKERFILE",
      dockerfilePath: "app/Dockerfile",
    },
    deploy: {
      sleepApplication: true,
    },
    replicas: 1,
  });
  const admin = service("admin", {
    source: github("bmja62/padylife", { branch, rootDirectory: "admin" }),
    domains: [prod ? "admin.padylife.ir" : "staging-admin.padylife.ir"],
    build:{
      buildEnvironment: "V3",
      builder: "DOCKERFILE",
      dockerfilePath: "admin/Dockerfile",
    },
    deploy: {
      sleepApplication: true,
    },
    replicas: 1,
  });

  const www = service("www", {
    source: github("bmja62/padylife", { branch, rootDirectory: "www" }),
    domains: [prod ? "www.padylife.ir" : "staging-www.padylife.ir"],
    build: {
      buildEnvironment: "V3",
      builder: "DOCKERFILE",
      dockerfilePath: "www/Dockerfile",
    },
    deploy: {
      sleepApplication: true,
    },
    replicas: 1,
  });

  return project("padylife", {resources: [api, app, admin, postgresService, postgresVolume, www]});
});
