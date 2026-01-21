/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly API_HTTPS: string | undefined;
  readonly API_HTTP: string | undefined;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
