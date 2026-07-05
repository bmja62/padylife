FROM node:20-alpine AS build
WORKDIR /app

RUN corepack enable

COPY . .
RUN if [ -f ".env.${RAILWAY_ENVIRONMENT_NAME:-production}" ]; then cp ".env.${RAILWAY_ENVIRONMENT_NAME:-production}" .env.production; fi
RUN yarn install --immutable
RUN yarn build

FROM node:20-alpine AS runtime
WORKDIR /app

RUN npm install -g serve@14

COPY --from=build /app/dist ./dist

EXPOSE 3000
CMD ["sh", "-c", "serve -s dist -l ${PORT:-3000}"]
