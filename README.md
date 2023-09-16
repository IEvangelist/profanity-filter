# 🤬 GitHub Action: Profanity filter

[![.NET](https://github.com/IEvangelist/profanity-filter/actions/workflows/dotnet.yml/badge.svg)](https://github.com/IEvangelist/profanity-filter/actions/workflows/dotnet.yml) [![Dogfood](https://github.com/IEvangelist/profanity-filter/actions/workflows/dogfood.yml/badge.svg)](https://github.com/IEvangelist/profanity-filter/actions/workflows/dogfood.yml)

## 🤓 Usage

The following is an example of how to use the action in your workflow.

```yaml
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
      issues: write
      pull-requests: write

    steps:

    - name: Scan issue or pull request for profanity
      if: ${{ github.actor != 'dependabot[bot]' && github.actor != 'github-actions[bot]' }}
      uses: IEvangelist/profanity-filter@main
      id: profanity-filter
      with:
        token: ${{ secrets.GITHUB_TOKEN }}
        replacement-type: Emoji # Asterisk or Emoji
```

## 👀 Inputs

| Input | Description | Required |
|--|--|--|
| `token` | The GitHub token used to update the issues or pull requests with. Example, `secrets.GITHUB_TOKEN`. | `true` |
| `replacement-type` | The type of replacement method to use when profane content is filtered. Valid values are defined in the _ReplacementType.cs_ | `false` (default: `Asterisk`) |