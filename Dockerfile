FROM node:22-slim

WORKDIR /app

COPY package*.json ./
RUN npm install --omit=dev

COPY src/ ./src/

VOLUME ["/app/data"]

CMD ["node", "--experimental-sqlite", "src/index.js"]
