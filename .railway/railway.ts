import { defineRailway, github, preserve, project, service } from "railway/iac";

export default defineRailway((ctx) => {
  const prod = ctx.environment === "production";
  const branch = "main";

  const api = service("api", {
    source: github("bmja62/padylife", { branch, rootDirectory: "." }),
    build: {
      builder: "DOCKERFILE",
      dockerfilePath: ".deploy/api/Dockerfile",
    },
    healthcheck: "/swagger/index.html",
    healthcheckTimeout: 300,
    replicas: 1,
    env: {
      ASPNETCORE_ENVIRONMENT: ctx.environment,
      ConnectionStrings__PostgreSQL: preserve(),
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

  const www = service("www", {
    source: github("bmja62/padylife", { branch, rootDirectory: "." }),
    build: {
      builder: "DOCKERFILE",
      dockerfilePath: ".deploy/www/Dockerfile",
    },
    replicas: 1,
  });

  return project("padylife", {
    resources: [api, app, admin, www],
  });
});
