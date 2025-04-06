# Build stage
FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build:skip-typescript

# Production stage
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html

# For environment variable handling at runtime
COPY .env.production /usr/share/nginx/html/.env

# Add script to replace environment variables at runtime
RUN apk add --no-cache bash
COPY docker-entrypoint.sh /
RUN chmod +x /docker-entrypoint.sh

# Create empty env-config.js that will be populated at runtime
RUN echo "window.env = window.env || {};" > /usr/share/nginx/html/env-config.js

# Expose port
EXPOSE 80

# Start command
ENTRYPOINT ["./docker-entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]