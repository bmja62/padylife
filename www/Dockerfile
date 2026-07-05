FROM node:20-alpine AS build
WORKDIR /app

COPY package.json ./
RUN npm install

COPY . .
RUN if [ -f ".env.${RAILWAY_ENVIRONMENT_NAME:-production}" ]; then cp ".env.${RAILWAY_ENVIRONMENT_NAME:-production}" .env; fi
RUN npm run build

FROM node:20-alpine AS runtime
WORKDIR /app

COPY package.json ./
RUN npm install --omit=dev

COPY --from=build /app/dist ./dist

EXPOSE 3000
CMD ["npm", "run", "start"]
