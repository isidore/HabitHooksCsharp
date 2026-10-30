# habit-hooks-csharp

The C# plugin for [Habit Hooks](https://github.com/habit-hooks/habit-hooks): finds structural
code smells in C# and turns each one into guidance an AI coding agent can act on.

## Install

Until it is on PyPI, install it from this repo. Building it needs the .NET 8 SDK or later.

```sh
uv tool install habit-hooks --with git+https://github.com/isidore/HabitHooksCsharp
```

`uv tool install --force` replaces every plugin, so add a `--with` for each plugin you already use.

## Enable

```toml
# .habit-hooks/config.toml
plugins = ["csharp", "generic"]
```

Installing a plugin does not switch it on: it has to be named in `plugins`.
The `generic` plugin adds oversized files and duplication.

## What it finds

| Smell | Sensor | Reported when |
| --- | --- | --- |
| `too-many-parameters` | smells | a method or constructor has 4 or more parameters |
| `oversized-function` | smells | a method has 12 or more statements |
| `deep-nesting` | smells | `if`/`for`/`foreach`/`while`/`do`/`switch`/`try` nest 3 deep (`else if` doesn't add depth) |
| `high-complexity` | smells | a method's cyclomatic complexity is 10 or more |
| `swallowed-exception` | smells | a `catch` block has no statements |
| `non-essential-comment` | comment | a comment is 10 or more characters (blocks: 15) |
| `unused-member`, `unused-field`, `unused-private-member`, `unused-parameter`, `unused-variable`, `unused-import` | roslyn | the matching Roslyn analyzer warns (CA1822, CA1823, IDE0051, IDE0052, IDE0060, CS0168, CS0219, IDE0005) |
| `high-coupling` | roslyn | CA1506 warns (a method coupled to 30 or more types) |

The thresholds match the Java plugin.

1. **smells** reads the syntax tree; it doesn't build anything, so it is fast.
1. **comment** skips the comments a file opens with (license headers) and `///` XML doc comments.
1. **roslyn** runs `dotnet build` in Debug with the analyzers above turned on, through a targets file that
   only this build imports. It works even when the project treats warnings as errors or turns analyzers off,
   and it doesn't change the project's files.

## Configure

```toml
# .habit-hooks/config.toml

# Also report license headers and `///` doc comments.
[sensors.comment]
args = ["--include-file-header", "--include-doc-comments"]

# Name the solution or project to build when the folder has more than one.
[sensors.roslyn]
args = ["--project", "MySolution.sln"]

# Skip the build-based sensor entirely.
# [sensors.roslyn]
# disabled = true
```

## Detectors

- `dotnet`: the [.NET SDK](https://dotnet.microsoft.com/download), 8 or later.

## Develop

```sh
dotnet test HabitHooksCsharp.slnx                    # approval tests for csharp-tool
uv build --wheel                                     # builds csharp-tool into the wheel
uv tool install --force habit-hooks --with ./        # try it locally
```

The sensors in `src/habit_hooks_csharp/sensors/` all call `csharp-tool` (`comments`, `smells`, `analyzers`).
Commit messages use [Arlo's commit notation](InternalDocs/commit_notation.md), built with `tools/create-commit-message.sh`.
