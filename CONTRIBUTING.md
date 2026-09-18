# Contributing to Firebot

Thanks for your interest in contributing to Firebot! This document explains how to report issues, suggest features, and submit code changes.

## Ways to Contribute

- **Bug reports** — open an issue on the [GitHub Issue Tracker](https://github.com/danilogmoura/firestone-bot/issues/new/choose).
- **Feature requests** — same place, using the feature request template.
- **Code contributions** — submit a pull request.
- **Documentation** — improvements to the README, ADVANCED.md, or this file are always welcome.

## Before You Start

1. Check the [existing issues](https://github.com/danilogmoura/firestone-bot/issues) to avoid duplicates.
2. Make sure you are using the latest version of Firebot.
3. For bug reports, attach your **MelonLoader log** (`Firestone/MelonLoader/Latest.log`) if the game crashed or the bot failed.

## Development Setup

Firebot is a C# mod for Firestone Idle RPG, built with MelonLoader.

### Prerequisites

- **Windows**
- **MelonLoader V0.7.3 or higher** installed in your Firestone game folder.
- **.NET SDK** (version compatible with the project — see the `.csproj` files).
- **Visual Studio** or any C# editor of your choice.

### Building from Source

See [ADVANCED.md](ADVANCED.md) → *Installation (From Source)* for detailed steps, including how to set the game path in `src/Directory.Build.props`.

### Running Your Build

After building, copy your `firebot.dll` into the game's `Mods/` folder, replacing the released version. Launch the game and test your changes.

## Code Style

- Follow the existing code style in the repository.
- Keep changes focused and minimal.
- Use meaningful names for variables, methods, and classes.
- Add comments only where the code is not self-explanatory.
- Prefer small, composable methods over large ones.

## Commit Messages

This project uses [Conventional Commits](https://www.conventionalcommits.org/). Examples:

- `feat(tasks): hold a task back until the character reaches the required level`
- `fix(bot): bound the wait for a clickable combo key`
- `chore(log): move detailed output behind debug_mode`
- `docs(readme): document level requirements`

Use the same prefixes you see in the changelog: `feat`, `fix`, `chore`, `docs`, `refactor`, `test`.

## Pull Request Process

1. Fork the repository and create a branch from `main`.
2. Make your changes, following the code style and commit message guidelines.
3. Ensure the project builds without errors.
4. Test your changes in-game if possible.
5. Open a pull request against `main`.
6. In the PR description, explain **what** you changed and **why**.
7. Link any related issues (e.g. `Closes #12`).

A maintainer will review your PR. Please be patient — this is a volunteer-driven project.

## Code of Conduct

Be respectful. Harassment, discrimination, or toxic behavior will not be tolerated. We are here to build something useful together.

## Questions?

Join our [Discord server](https://discord.gg/JB8RJAs6J3) if you need help or want to discuss an idea before opening an issue or PR.
