# Profanity Filter Documentation

The marketing + documentation site for the **🤬 Potty Mouth** GitHub Action, built with
[Astro](https://astro.build) and [Tailwind CSS v4](https://tailwindcss.com).

🌐 Live: <https://ievangelist.github.io/profanity-filter>

## 🚀 Local development

### Prerequisites

- **Node.js** 22.12 or higher (Astro 6 requirement)
- **npm** 10.x or higher

### Install

```bash
cd docs
npm install
```

### Develop

```bash
npm run dev
```

Open <http://localhost:4321/profanity-filter/>.

### Build

```bash
npm run build
```

Static output lands in `dist/`.

### Preview the production build

```bash
npm run preview
```

## 📁 Project layout

```
docs/
├── public/                       # Static assets (favicon, etc.)
├── src/
│   ├── components/               # Reusable Astro components
│   │   ├── Navbar.astro
│   │   ├── Footer.astro
│   │   ├── CodeBlock.astro       # <Code> wrapper with copy button
│   │   ├── Callout.astro         # note/tip/important/warning
│   │   ├── FeatureCard.astro
│   │   ├── StrategyCard.astro
│   │   └── Stat.astro
│   ├── layouts/
│   │   └── BaseLayout.astro      # Shared shell, theme toggle, fonts
│   ├── pages/
│   │   ├── index.astro           # Marketing landing page
│   │   ├── getting-started.astro # 3-step quick start
│   │   ├── api.astro             # Full API reference
│   │   └── examples.astro        # Copy-paste recipes
│   └── styles/
│       └── global.css            # Tailwind v4 entry + design tokens
├── astro.config.mjs              # Astro + Tailwind Vite plugin
├── package.json
└── tsconfig.json
```

## 🌐 Deployment

`.github/workflows/deploy-docs.yml` builds and publishes the site to GitHub Pages on every push
to `main`.

## 🎨 Design system

Tailwind v4 is configured entirely from CSS — there is **no `tailwind.config.js`**. All design
tokens live in `src/styles/global.css` under `@theme`:

- **Brand palette** — `brand-50` … `brand-950` (red accent)
- **Ink palette** — `ink-50` … `ink-950` (neutral / surface)
- **Fonts** — Inter (sans) and JetBrains Mono (mono), loaded from Google Fonts
- **Dark mode** — driven by `data-theme="dark"` on `<html>`, toggled via the navbar button and
  persisted in `localStorage`

Custom utilities defined in `global.css`:

- `bg-grid` — subtle grid backdrop
- `glow` — radial brand-color glow for hero backgrounds
- `text-shimmer` — animated gradient text
- `animate-float` — gentle floating animation for the hero emoji

## ✏️ Adding a page

1. Create a new `.astro` file in `src/pages/`.
2. Import the layout and the components you need:

   ```astro
   ---
   import BaseLayout from '../layouts/BaseLayout.astro';
   import CodeBlock from '../components/CodeBlock.astro';
   import Callout from '../components/Callout.astro';
   ---

   <BaseLayout title="My new page — Potty Mouth" description="Short description for SEO.">
     <section class="max-w-4xl mx-auto px-6 lg:px-8 py-16">
       <h1 class="text-4xl font-bold tracking-tight">My new page</h1>
       <p class="mt-4 text-ink-600 dark:text-ink-300">Content goes here…</p>

       <Callout variant="tip" title="Pro tip">
         Use the supplied components for visual consistency.
       </Callout>

       <CodeBlock lang="yaml" code={`name: hello\nrun: echo hi`} />
     </section>
   </BaseLayout>
   ```

3. Add a link to it in `src/components/Navbar.astro` if it should appear in the nav.

## 📚 Tech stack

- **Framework:** [Astro 6](https://astro.build)
- **Styling:** [Tailwind CSS v4](https://tailwindcss.com) via `@tailwindcss/vite`
- **Code highlighting:** [Shiki](https://shiki.style) (built into Astro's `<Code>` component)
- **Hosting:** GitHub Pages
- **Language:** TypeScript

## 🛠️ Maintenance

```bash
# Refresh dependencies to the versions in package.json
npm install

# See what's outdated
npm outdated

# Update everything inside the allowed semver range
npm update
```

## 📄 License

MIT, same as the parent repository.
