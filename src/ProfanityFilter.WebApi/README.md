# Potty Mouth — Profanity Filter WebApi container

[![Container image](https://img.shields.io/badge/ghcr.io-IEvangelist%2Fprofanity--filter.webapi-blue?logo=docker&logoColor=white)](https://github.com/IEvangelist/profanity-filter/pkgs/container/profanity-filter.webapi)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/IEvangelist/profanity-filter/blob/main/LICENSE)
[![Docs](https://img.shields.io/badge/docs-ievangelist.github.io%2Fprofanity--filter-brightgreen)](https://ievangelist.github.io/profanity-filter/)

A minimal Web API + SignalR hub that wraps the [Profanity Filter](https://github.com/IEvangelist/profanity-filter) engine — the same engine that powers the GitHub Action. Stream text in over HTTP or a SignalR hub, get the filtered result out, and pick from **14 replacement strategies** while you do it.

## Live demo

A short, silent recording of the playground filtering live over SignalR — the input pane on the left, the filtered output on the right, with the replacement strategy dropdown switching between Asterisk, Emoji, Redacted Rectangle, Middle Swear Emoji, Bleep, and First Letter + Asterisk.

<!-- GitHub renders <video> tags inline. GHCR and most markdown viewers do too. -->
<video src="https://ievangelist.github.io/profanity-filter/demo.webm"
       poster="https://ievangelist.github.io/profanity-filter/demo-poster.png"
       width="960"
       controls
       muted
       loop
       playsinline>
  Your browser does not support embedded video.
  <a href="https://ievangelist.github.io/profanity-filter/demo.webm">Download the demo</a>.
</video>

> [!TIP]
> If the embed above doesn't render in your container registry, watch it directly on the docs site: <https://ievangelist.github.io/profanity-filter/#see-it-in-action> — or grab the raw `.webm` here: <https://ievangelist.github.io/profanity-filter/demo.webm>.

A captions track (WebVTT) describing every step is also published alongside the video: <https://ievangelist.github.io/profanity-filter/demo.vtt>.

## Quick start

```bash
# Pull the latest WebApi container
docker pull ghcr.io/ievangelist/profanity-filter.webapi:latest

# Run it on http://localhost:8080
docker run --rm -p 8080:8080 ghcr.io/ievangelist/profanity-filter.webapi:latest
```

Then `POST` some text and pick a strategy:

```bash
curl -X POST http://localhost:8080/filter \
  -H "Content-Type: application/json" \
  -d '{"text":"This shit is broken, what the hell are we doing here?","strategy":"emoji"}'
```

## Replacement strategies

All 14 are implemented in [`MatchEvaluators.cs`](https://github.com/IEvangelist/profanity-filter/blob/main/src/ProfanityFilter.Services/Internals/MatchEvaluators.cs):

| Strategy                       | Example                |
| ------------------------------ | ---------------------- |
| `asterisk`                     | `****`                 |
| `random-asterisk`              | `* — ****`             |
| `middle-asterisk`              | `f**k`                 |
| `first-letter-then-asterisk`   | `f***`                 |
| `vowel-asterisk`               | `sh*t`                 |
| `emoji`                        | 💩                     |
| `anger-emoji`                  | 😡                     |
| `middle-swear-emoji`           | `f🤬k`                 |
| `bleep`                        | `bleep`                |
| `redacted-rectangle`           | `████`                 |
| `strike-through`               | <del>shit</del>        |
| `underscores`                  | `____`                 |
| `grawlix`                      | `#%$!`                 |
| `bold-grawlix`                 | **`#%$!`**             |

## Learn more

* 🏠 Docs site — <https://ievangelist.github.io/profanity-filter/>
* 🧑‍🚀 Aspire AppHost — [`playground/apphost.cs`](https://github.com/IEvangelist/profanity-filter/blob/main/playground/apphost.cs)
* 🔌 SignalR hub — [`ProfanityHub.cs`](https://github.com/IEvangelist/profanity-filter/blob/main/src/ProfanityFilter.WebApi/Hubs/ProfanityHub.cs)
* 🌐 Vite client — [`api.ts`](https://github.com/IEvangelist/profanity-filter/blob/main/src/ProfanityFilter.Web/src/api.ts)
* 📦 All container tags — <https://github.com/IEvangelist/profanity-filter/pkgs/container/profanity-filter.webapi>

## License

MIT — see [`LICENSE`](https://github.com/IEvangelist/profanity-filter/blob/main/LICENSE).
