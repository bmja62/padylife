FROM node:20-alpine AS build
WORKDIR /app

RUN corepack enable

COPY package.json pnpm-lock.yaml .npmrc* ./
RUN pnpm install --frozen-lockfile

COPY . .
RUN pnpm run generate

FROM node:20-alpine AS runtime
WORKDIR /app

RUN npm install -g serve@14

COPY --from=build /app/.output/public ./public

EXPOSE 3000
CMD ["sh", "-c", "serve -s public -l ${PORT:-3000}"]
