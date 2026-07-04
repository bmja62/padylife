FROM node:20-alpine AS build
WORKDIR /app

RUN corepack enable

COPY package.json yarn.lock .yarnrc.yml .npmrc ./
RUN yarn install --frozen-lockfile

COPY . .
RUN yarn build

FROM node:20-alpine AS runtime
WORKDIR /app

RUN npm install -g serve@14

COPY --from=build /app/dist ./dist

EXPOSE 3000
CMD ["sh", "-c", "serve -s dist -l ${PORT:-3000}"]
