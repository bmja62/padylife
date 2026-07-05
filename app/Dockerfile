FROM node:20-alpine AS build
WORKDIR /app

ARG RAILWAY_ENVIRONMENT_NAME
ENV RAILWAY_ENVIRONMENT_NAME=${RAILWAY_ENVIRONMENT_NAME}

RUN corepack enable
RUN corepack prepare pnpm@9.15.4 --activate

COPY . .
RUN ENV_NAME="${RAILWAY_ENVIRONMENT_NAME:?RAILWAY_ENVIRONMENT_NAME is required}" && \
	cp ".env.${ENV_NAME}" .env
RUN pnpm install --frozen-lockfile

RUN pnpm run generate

FROM node:20-alpine AS runtime
WORKDIR /app

RUN npm install -g serve@14

COPY --from=build /app/.output/public ./public

EXPOSE 3000
CMD ["sh", "-c", "serve -s public -l ${PORT:-3000}"]
