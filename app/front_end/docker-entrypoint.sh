#!/bin/bash

# Replace environment variables in the .env file (optional if you still want .env for debugging)
if [ -f /usr/share/nginx/html/.env ]; then
  [ ! -z "$VITE_API_URL" ] && sed -i "s|VITE_API_URL=.*|VITE_API_URL=$VITE_API_URL|g" /usr/share/nginx/html/.env
  [ ! -z "$VITE_SOCKET_URL" ] && sed -i "s|VITE_SOCKET_URL=.*|VITE_SOCKET_URL=$VITE_SOCKET_URL|g" /usr/share/nginx/html/.env
  [ ! -z "$VITE_ORGANIZATION_API_URL" ] && sed -i "s|VITE_ORGANIZATION_API_URL=.*|VITE_ORGANIZATION_API_URL=$VITE_ORGANIZATION_API_URL|g" /usr/share/nginx/html/.env
  [ ! -z "$VITE_ORGANIZATION_SOCKET_URL" ] && sed -i "s|VITE_ORGANIZATION_SOCKET_URL=.*|VITE_ORGANIZATION_SOCKET_URL=$VITE_ORGANIZATION_SOCKET_URL|g" /usr/share/nginx/html/.env
fi

# Generate a valid env-config.js file
cat <<EOF > /usr/share/nginx/html/env-config.js
window.env = {
  VITE_API_URL: "${VITE_API_URL}",
  VITE_SOCKET_URL: "${VITE_SOCKET_URL}",
  VITE_ORGANIZATION_API_URL: "${VITE_ORGANIZATION_API_URL}",
  VITE_ORGANIZATION_SOCKET_URL: "${VITE_ORGANIZATION_SOCKET_URL}"
};
EOF

# Run the command passed to the container (usually starts nginx)
exec "$@"
