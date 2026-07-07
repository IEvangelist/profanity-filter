export const versions = {
  release: '13.4.6.1',
  aspire: '13.4.0',
  legacyPreviewPattern: '9.0.7-alpha.*',
} as const;

export const images = {
  action: 'ievangelist/profanity-filter',
  api: 'ievangelist/profanity-filter-api',
  registry: 'ghcr.io',
} as const;

export const aspireAppHost = {
  csharpFile: 'apphost.cs',
  typescriptFile: 'apphost.mts',
  generatedModulePath: './.aspire/modules',
  builderModule: './.aspire/modules/aspire.mjs',
} as const;

export const imageReference = (image: string, tag = versions.release) =>
  `${images.registry}/${image}:${tag}`;
