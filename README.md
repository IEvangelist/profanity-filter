![profanity-filter](https://socialify.git.ci/IEvangelist/profanity-filter/image?description=1&descriptionEditable=%F0%9F%A4%AC%20Potty%20Mouth%3A%20A%20GitHub%20Action%20profanity%20filter%20written%20in%20.NET%2C%20leveraging%20Native%20AOT%20compilation.&font=Raleway&language=1&name=1&owner=1&pattern=Signal&theme=Auto)
 
# Potty Mouth: GitHub Action

> This repository contains the source code for a profane content filter 🤬.

[![.NET](https://github.com/IEvangelist/profanity-filter/actions/workflows/dotnet.yml/badge.svg)](https://github.com/IEvangelist/profanity-filter/actions/workflows/dotnet.yml) [![Dogfood](https://github.com/IEvangelist/profanity-filter/actions/workflows/dogfood.yml/badge.svg)](https://github.com/IEvangelist/profanity-filter/actions/workflows/dogfood.yml)

The GitHub Action: Profane content filter maintains over 4,900 swear words from nine different languages. For a list of all supported languages and swear words, [see the data directory](https://github.com/IEvangelist/profanity-filter/tree/main/src/ProfanityFilter.Services/Data) raw newline-delimited text files with each corresponding language's alphabetically sorted swear word list. This tool is used to scan issues, pull requests and comments in either for profanity. It can be configured to replace any profane content with a [strategy](#-replacement-strategies) and renders a complete job summary for tracking profanity filter results. This action can be useful for maintaining a professional and respectful environment in your GitHub repository.

## ⁉️ Why

But why is this important? Let's be honest, not everyone who creates issues or pull requests use appropriate language (it's not always rainbows and ponies, am I right?)

> [!NOTE]
> With this action in your repositories GitHub workflow, it can be 🌈 and 🐎.

## 🤓 Usage

The following is an example of how to use the action as a standalone workflow:

```yml
# The name of the workflow
name: Profanity filter

# Trigger on issue or pull requests, that are opened, edited, or reopened
on:
  issue_comment:
    types: [created, edited]
  issues:
    types: [opened, edited, reopened]
  pull_request:
    types: [opened, edited, reopened]

# Required permissions
permissions:
  issues: write
  pull-requests: write

jobs:
  # Name the job whatever you'd like
  apply-filter:

    runs-on: ubuntu-latest

    steps:

    # Name the step anything that makes sense to you
    - name: Scan issue or pull request for profanity
      # Conditionally run the step if the actor isn't a bot
      if: ${{ github.actor != 'dependabot[bot]' && github.actor != 'github-actions[bot]' }}
      uses: IEvangelist/profanity-filter@main
      id: profanity-filter
      with:
        token: ${{ secrets.GITHUB_TOKEN }}
        # See https://bit.ly/potty-mouth-replacement-strategies
        replacement-strategy: Emoji # See Replacement strategy
```

If you already have an existing workflow that is triggered `on/issues|pull_request/types/opened|edited|reopened` feel free to simply add a step to the existing job:

```yml
- name: Scan issue or pull request for profanity
  # Conditionally run the step if the actor isn't a bot
  if: ${{ github.actor != 'dependabot[bot]' && github.actor != 'github-actions[bot]' }}
  # Use the profanity filter action
  uses: IEvangelist/profanity-filter@main
  id: profanity-filter
  with:
    token: ${{ secrets.GITHUB_TOKEN }}
    # See https://bit.ly/potty-mouth-replacement-strategies
    replacement-strategy: FirstLetterThenAsterisk
```

> [!IMPORTANT]
> You'll still need to ensure that the existing GitHub workflow has the appropriate `permissions`, with `issues: write` and `pull-requests: write` such that the profanity filter's `${{ secrets.GITHUB_TOKEN }}` will be capable of applying filters.

## 👀 Inputs

This action has several inputs. Only the `token` is required, and the `replacement-strategy` defaults to `asterisk` when not specified.

The following table describes each input:

| Input | Description | Required |
|--|--|--|
| `token` | The GitHub token used to update the issues or pull requests with.<br><br>Example, `${{ secrets.GITHUB_TOKEN }}`. | `true` |
| `replacement-strategy` | The type of replacement method to use when profane content is filtered. | `false` (default: `asterisk`) |
| `include-updated-note` | A `boolean` value to indicate if the action should include a note in the issue or pull request body when profane content is replaced. | `false` (default: `true`) |
| `include-confused-reaction` | A `boolean` value to indicate if the action should react to the issue or pull request with the confused 😕 reaction. | `false` (default: `false`) |
| `manual-profane-words` | A `string` value, with a comma-separated list of additional profane words to include in the filter. | `false` |
| `custom-profane-words-url` | A URL that returns a `string` value, with a newline-separated list of custom profane words to include in the filter. | `false` |

### 😵 Replacement strategies

Each replacement strategy corresponds to a different way of replacing profane content. The following represents the available replacement types:

| Strategy input | Four letter profane word example | Description |
| --- | --- | --- |
| `asterisk` | `****` | Replaces profane content with asterisks. |
| `emoji` | `💩` | Replaces profane content with a random emoji. |
| `grawlix` | `#%$!` | Replaces profane content with grawlix symbols. |
| `bold-grawlix` | __`#%$!`__ | Replaces profane content with bold grawlix symbols. |
| `bleep` | `bleep` | Replaces profane content with the word "bleep". |
| `redacted-rectangle` | `████` | Replaces profane content with rectangles to redact their content. |
| `anger-emoji` | `😡` | Replaces profane content with a random anger emoji. |
| `middle-asterisk` | `f**k` | Replaces profane content with asterisk, but only in the middle of the word. |
| `middle-swear-emoji` | `f🤬k` | Replaces profane content with a random swear emoji, but only in the middle of the word. |
| `random-asterisk` | `*` - `****` | Replaces profane content with a random number of asterisks. |
| `first-letter-then-asterisk` | `f***` | Replaces profane content with asterisks after the first letter. |
| `vowel-asterisk` | `sh*t` | Replaces profane content with asterisks, but only the vowels. |
| `strike-through` | ~~`shit`~~ | Encloses profane content with `~~` causing strikethrough rendering. |
| `underscores` | `____` | Replaces profane content with underscores `_`. |

The value of the `replacement-strategy` input is case-insensitive, and accepts hyphened alternatives (such as `anger-emoji`) as well.

> [!TIP]
> All asterisk replacement strategies are escaped with a backslash `\` to prevent markdown from rendering the asterisks as bold text. No need for you the escape these values yourself. This excludes title updates, as markdown is supported there.

## 🏷️ Label requirements

This action will look for a label with the following verbatim name `"profane content 🤬"`, if found this label is applied to any issue or pull request where profane content filtration occurs.

Consider the following automatically applied label to an issue that contains profane content:

![A screenshot of a user experience where the GitHub Actions `bot` added the profane content label 6 hours ago.](assets/label-applied-lght.png#gh-light-mode-only)
![A screenshot of a user experience where the GitHub Actions `bot` added the profane content label 6 hours ago.](assets/label-applied-dark.png#gh-dark-mode-only)

## 🎬 What happens?

When profane content is detected, the action will update the issue or pull request by:

- Replacing any found profane content with the configured replacement strategy.
- Reacting to the issue or pull request with the [confused 😕 reaction](https://docs.github.com/rest/reactions/reactions).
- Conditionally applying the `profane content 🤬` label if found in the repository.
- Reporting the profane content in the workflow summary as a detailed table.

Consider the following diagram, that represents the workflow of this GitHub action:

```mermaid
flowchart TD
    A(Issue or Pull Request)
      --> B[Contains Profane Content?]
    B -->|YES| C(Apply Filter)
      --> E(All swear words are filtered, for example sw**r)
      --> F(React to and label profane content)
      --o G[Job Summary]
    B --o|NO| D{{Stop}} ~~~A
```

### ⚠️ Example job summary

The following is an example of a job summary that is rendered when profane content is detected:

![A screenshot of an example job summary, showing the application process of the profanity filter.](assets/job-summary-lght.png#gh-light-mode-only)
![A screenshot of an example job summary, showing the application process of the profanity filter.](assets/job-summary-dark.png#gh-dark-mode-only)
