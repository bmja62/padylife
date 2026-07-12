import { defineRailway, github, project, service, postgres } from "railway/iac";

export default defineRailway((ctx) => {
  const prod = ctx.environment === "production";
  const branch = prod ? "release" : "main";

  const postgresService = {
    ...(prod ? postgres("Postgres-Production") : postgres("Postgres-Staging")),
    deploy: {
      sleepApplication: true,
    },
  };

  const api = service("api", {
    source: github("bmja62/padylife", { branch, rootDirectory: "api" }),
    domains: [prod ? "api.padylife.ir" : "staging-api.padylife.ir"],
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

  return project("padylife", {resources: [api, app, admin, postgresService, www]});
});
