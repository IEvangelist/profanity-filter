# GitHub Action: 🤬 Profane content filter

[![.NET](https://github.com/IEvangelist/profanity-filter/actions/workflows/dotnet.yml/badge.svg)](https://github.com/IEvangelist/profanity-filter/actions/workflows/dotnet.yml) [![Dogfood](https://github.com/IEvangelist/profanity-filter/actions/workflows/dogfood.yml/badge.svg)](https://github.com/IEvangelist/profanity-filter/actions/workflows/dogfood.yml)

The GitHub Action: Profane content filter maintains over 4,970 swear words from nine different languages. This tool can be used to scan issues and pull requests for profanity. It can be configured to replace any profane content with either asterisks or emojis. This action can be useful for maintaining a professional and respectful environment in your GitHub repository.

## ⁉️ Why

But why is this important? Let's be honest, not everyone who creates issues or pull requests use appropriate language (it's not always rainbows and ponies, am I right?) With this action in your repositories GitHub workflow, it can be 🌈 and 🐎.

## 🤓 Usage

The following is an example of how to use the action as a standalone workflow:

```yml
name: Profanity filter

on:
  issues:
    types:
      - opened
      - edited
      - reopened
  pull_request:
    types:
      - opened
      - edited
      - reopened

jobs:
  eat:

    runs-on: ubuntu-latest
    permissions:
      # Required permissions.
      issues: write
      pull-requests: write

    steps:

    - name: Scan issue or pull request for profanity
      if: ${{ github.actor != 'dependabot[bot]' && github.actor != 'github-actions[bot]' }}
      uses: IEvangelist/profanity-filter@main
      id: profanity-filter
      with:
        token: ${{ secrets.GITHUB_TOKEN }}
        replacement-type: Emoji # See Replacement types
```

If you already have an existing workflow that is triggered `on/issues|pull_request/types/opened|edited|reopened` feel free to simply add a step to the existing job:

```yml
- name: Scan issue or pull request for profanity
  if: ${{ github.actor != 'dependabot[bot]' && github.actor != 'github-actions[bot]' }}
  uses: IEvangelist/profanity-filter@main
  id: profanity-filter
  with:
    token: ${{ secrets.GITHUB_TOKEN }}
    replacement-type: Emoji # See Replacement types
```

## 👀 Inputs

| Input | Description | Required |
|--|--|--|
| `token` | The GitHub token used to update the issues or pull requests with. Example, `secrets.GITHUB_TOKEN`. | `true` |
| `replacement-type` | The type of replacement method to use when profane content is filtered. Valid values are, `Asterisk` or `Emoji`. | `false` (default: `Asterisk`) |

### Replacement types

Each replacement type corresponds to a different way of replacing profane content. The following represents the available replacement types:

| `ReplacementType` | Valid string value | Description |
| --- | --- | --- |
| `ReplacementType.Asterisk` | `"Asterisk"` | Replaces profane content with asterisks. For example, a swear word with four letters would look like this `****`. |
| `ReplacementType.Emoji` | `"Emoji"` | Replaces profane content with a random swear emoji. For example, a swear word with four letters could look like this `💩`. |
| `ReplacementType.RandomAsterisk` | `"RandomAsterisk"` | Replaces profane content with a random number of asterisks. For example, a swear word with four letters could look like any value between `*` and `****`. |
| `ReplacementType.MiddleAsterisk` | `"MiddleAsterisk"` | Replaces profane content with asterisks, but only in the middle of the word. For example, a swear word with four letters could look like this `f**k`. |
| `ReplacementType.MiddleSwearEmoji` | `"MiddleSwearEmoji"` | Replaces profane content with a random swear emoji, but only in the middle of the word. For example, a swear word with four letters could look like this `f🤬k`. |
| `ReplacementType.VowelAsterisk` | `"VowelAsterisk"` | Replaces profane content with asterisks, but only the vowels. For example, a swear word with four letters could look like this `sh*t`. |

## Label requirements

This action will look for a label with the following verbatim name `"profane content 🤬"`, if found this label is applied to any issue or pull request where profane content filtration occurs.

## What happens?

When profane content is detected, the action will update the issue or pull request by:

- replacing any found profane content with the configured replacement type
- reacting to the issue or pull request with the [confused 😕 reaction](https://docs.github.com/rest/reactions/reactions)
- and conditionally applying the `profane content 🤬` label if found in the repository.