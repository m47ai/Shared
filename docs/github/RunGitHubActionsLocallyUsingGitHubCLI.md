# Run `GitHub Actions` locally

Optional we do using `Act` through `GitHub CLI`

- Official Github CLI website: <https://cli.github.com/>
- Official ACT Repo: <https://github.com/nektos/act>

## Prerequisites

Installation instructtions of GitHub CLI: <https://github.com/cli/cli#installation>

### Install ACT through GitHub CLI

```bash
gh extension install https://github.com/nektos/gh-act
```

### Login into CLI

```bash
gh auth login
```

### List all workflows

```bash
gh extension exec act -l
```

### Execute workflow

```bash
gh extension exec act -j coverage-backend
```

### Pass through secrets

```bash
gh extension exec act -j coverage-backend --secret-file aws_credentials.hal2b.env
```

## Default settings

The settings are placed inside `.actrc`, more [info](https://github.com/nektos/act#configuration)

Example of `.actrc`:

```bash
# sample .actrc file
-P ubuntu-latest
--env-file aws_credentials.hal2b.env
```
