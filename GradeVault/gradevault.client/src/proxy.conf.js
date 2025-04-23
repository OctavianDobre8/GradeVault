const { env } = require('process');

// Change this line to set port 5240 as the default when no environment variables are present
const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:5240';

const PROXY_CONFIG = [
  {
    context: [
      "/api/auth",
      // Add any other API paths you need to proxy
      "/api/grades",
      "/api",
    ],
    target,
    secure: false,
    // Add these options for improved proxy stability
    changeOrigin: true,
    logLevel: "debug"
  }
]

module.exports = PROXY_CONFIG;