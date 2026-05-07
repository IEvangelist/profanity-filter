import { defineConfig } from 'astro/config';
import mdx from '@astrojs/mdx';
import tailwindcss from '@tailwindcss/vite';

// https://astro.build/config
export default defineConfig({
  site: 'https://ievangelist.github.io',
  base: '/profanity-filter',
  integrations: [mdx()],
  outDir: './dist',
  vite: {
    plugins: [tailwindcss()],
  },
});
