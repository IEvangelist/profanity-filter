# Profanity Filter Documentation

This directory contains the documentation website for the Profanity Filter GitHub Action, built with [Astro](https://astro.build).

## 🚀 Local Development

### Prerequisites
- Node.js 20.x or higher
- npm 10.x or higher

### Installation

```bash
cd docs
npm install
```

### Development Server

Start the development server:

```bash
npm run dev
```

The site will be available at `http://localhost:4321/profanity-filter/`

### Building for Production

Build the static site:

```bash
npm run build
```

The built site will be in the `dist/` directory.

### Preview Production Build

Preview the production build locally:

```bash
npm run preview
```

## 📁 Project Structure

```
docs/
├── public/                 # Static assets (favicon, etc.)
├── src/
│   ├── layouts/           # Page layouts
│   │   └── BaseLayout.astro
│   └── pages/             # Page routes
│       ├── index.astro           # Homepage
│       ├── getting-started.astro # Getting Started guide
│       ├── api.astro            # API documentation
│       └── examples.astro       # Usage examples
├── astro.config.mjs       # Astro configuration
├── package.json           # Dependencies
└── tsconfig.json         # TypeScript configuration
```

## 🌐 Deployment

The documentation is automatically deployed to GitHub Pages via the `.github/workflows/deploy-docs.yml` workflow when changes are pushed to the `main` branch.

**Live Site:** https://ievangelist.github.io/profanity-filter

## 📝 Adding Content

### Creating a New Page

1. Create a new `.astro` file in `src/pages/`
2. Use the `BaseLayout` component for consistent styling
3. Add a link to the navigation in `BaseLayout.astro`

Example:

```astro
---
import BaseLayout from '../layouts/BaseLayout.astro';
---

<BaseLayout title="My New Page - Potty Mouth">
  <div class="docs-container">
    <article class="docs-content">
      <h1>My New Page</h1>
      <p>Content goes here...</p>
    </article>
  </div>
</BaseLayout>
```

### Styling

The site uses CSS custom properties for theming and supports both light and dark modes automatically. Global styles are defined in `BaseLayout.astro`.

## 🎨 Design System

- **Colors**: Defined via CSS custom properties in `BaseLayout.astro`
- **Typography**: System font stack for optimal performance
- **Layout**: Responsive design with mobile-first approach
- **Components**: Reusable Astro components for consistency

## 📚 Tech Stack

- **Framework**: [Astro](https://astro.build) - Static site generator
- **Language**: TypeScript
- **Styling**: CSS with custom properties
- **Deployment**: GitHub Actions + GitHub Pages
- **Package Manager**: npm

## 🛠️ Maintenance

### Updating Dependencies

```bash
cd docs
npm update
```

### Checking for Outdated Packages

```bash
npm outdated
```

## 📄 License

This documentation is part of the Profanity Filter project and is licensed under the MIT License.
