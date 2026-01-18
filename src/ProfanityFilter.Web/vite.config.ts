import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// Resolve API URL from Aspire-injected environment variable
const apiHttps = process.env.API_HTTPS || '';
const apiHttp = process.env.API_HTTP || '';
const apiUrl = apiHttps || apiHttp;

console.log('🚀 Vite starting with API_HTTPS:', apiHttps || '(not set)');
console.log('🚀 Vite starting with API_HTTP:', apiHttp || '(not set)');

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  define: {
    // Expose the API URLs to client-side code
    'import.meta.env.API_HTTPS': JSON.stringify(apiHttps),
    'import.meta.env.API_HTTP': JSON.stringify(apiHttp),
  },
  server: {
    proxy: {
      // Always use proxy to forward /profanity requests to the API
      '/profanity': {
        target: apiUrl || 'http://localhost:5000',
        changeOrigin: true,
        secure: false,
        ws: true,
      },
    },
  },
  build: {
    outDir: 'dist',
    sourcemap: true,
  },
})
