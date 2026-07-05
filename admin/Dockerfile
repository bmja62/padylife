FROM node:20-alpine AS build
WORKDIR /app

ARG RAILWAY_ENVIRONMENT_NAME
ENV RAILWAY_ENVIRONMENT_NAME=${RAILWAY_ENVIRONMENT_NAME}

RUN corepack enable

COPY . .
RUN cp ".env-${RAILWAY_ENVIRONMENT_NAME}" .env
RUN yarn install --immutable
RUN yarn build --mode "${RAILWAY_ENVIRONMENT_NAME}"

FROM node:20-alpine AS runtime
WORKDIR /app

RUN npm install -g serve@14

COPY --from=build /app/dist ./dist

EXPOSE 3000
CMD ["sh", "-c", "serve -s dist -l ${PORT:-3000}"]
