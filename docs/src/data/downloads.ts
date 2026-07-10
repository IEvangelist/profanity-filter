import { images } from './versions';

export interface ContainerDownloads {
  /** Exact total downloads for the GitHub Action runner image. */
  action: number;
  /** Exact total downloads for the standalone API image. */
  api: number;
  /** Combined total downloads across both images. */
  total: number;
  /** Compact display for the combined total, e.g. "239K+". */
  totalCompact: string;
  /** Compact display for the Action image total. */
  actionCompact: string;
  /** Compact display for the API image total. */
  apiCompact: string;
  /** `true` when both numbers came from a live scrape, `false` when a fallback was used. */
  live: boolean;
}

// GitHub does not expose download counts for GHCR (container) packages through
// its REST/GraphQL APIs, but it does render a "Total downloads" figure on each
// package's web page. We read that number at build time. These fallbacks are a
// snapshot (captured 2026-07-10) so the docs build never fails when the scrape
// is unavailable — offline builds, GHCR markup changes, or rate limiting.
const FALLBACK = {
  action: 238_810,
  api: 1_048,
} as const;

const packagePage = (image: string) =>
  `https://github.com/IEvangelist/profanity-filter/pkgs/container/${image.split('/')[1]}`;

// Matches: <span ...>Total downloads</span> <h3 title="238810">239K</h3>
// The `title` attribute holds the precise integer.
const TOTAL_DOWNLOADS_RE = /Total downloads<\/span>\s*<h3 title="(\d+)"/;

async function scrapeTotal(url: string, signal: AbortSignal): Promise<number | null> {
  try {
    const response = await fetch(url, {
      signal,
      headers: {
        'user-agent':
          'profanity-filter-docs (+https://github.com/IEvangelist/profanity-filter)',
      },
    });

    if (!response.ok) {
      return null;
    }

    const match = (await response.text()).match(TOTAL_DOWNLOADS_RE);
    return match ? Number(match[1]) : null;
  } catch {
    return null;
  }
}

/** Formats a download count: compact "at least" labels for big numbers, exact for small ones. */
export function formatCompact(value: number): string {
  if (value >= 1_000_000) {
    return `${(value / 1_000_000).toFixed(1).replace(/\.0$/, '')}M+`;
  }

  if (value >= 10_000) {
    return `${Math.floor(value / 1_000)}K+`;
  }

  return value.toLocaleString('en-US');
}

let cached: Promise<ContainerDownloads> | undefined;

/**
 * Resolves the total download counts GitHub reports for the two container
 * images. Runs at build time, is memoized for the lifetime of the build, and
 * always resolves (falling back to a known-good snapshot on any failure).
 */
export function getContainerDownloads(): Promise<ContainerDownloads> {
  cached ??= (async () => {
    const controller = new AbortController();
    const timeout = setTimeout(() => controller.abort(), 8_000);

    try {
      const [action, api] = await Promise.all([
        scrapeTotal(packagePage(images.action), controller.signal),
        scrapeTotal(packagePage(images.api), controller.signal),
      ]);

      const resolvedAction = action ?? FALLBACK.action;
      const resolvedApi = api ?? FALLBACK.api;
      const total = resolvedAction + resolvedApi;

      return {
        action: resolvedAction,
        api: resolvedApi,
        total,
        totalCompact: formatCompact(total),
        actionCompact: formatCompact(resolvedAction),
        apiCompact: formatCompact(resolvedApi),
        live: action !== null && api !== null,
      };
    } finally {
      clearTimeout(timeout);
    }
  })();

  return cached;
}
