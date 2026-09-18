# Firebot

[![Discord](https://img.shields.io/badge/Discord-Join%20Server-5865F2?logo=discord&logoColor=white)](https://discord.gg/JB8RJAs6J3)

Automation bot mod for Firestone Idle RPG, using MelonLoader.

> **Just want to play?** Install [MelonLoader V0.7.3+](https://github.com/LavaGang/MelonLoader/releases/latest), extract the Firebot zip into your game folder, launch the game, press `F1` to configure the bot, then press `F7` to toggle it on.

---

## About

Firebot is an automation bot mod for Firestone Idle RPG, built with MelonLoader. It only automates actions a player could perform manually. It is open source and does not modify game resources or server logic.

---

## Quick Start

1. Install **MelonLoader V0.7.3 or higher** in `Firestone.exe`.
2. Download the latest Firebot release and extract it into the Firestone root folder (the one with `Firestone.exe`).
3. Launch the game. Press `F1` to open the settings panel and enable the tasks you want, then press `F7` to toggle Firebot on. Press `F2` to see the task status.

> **Note:** by default, all tasks are **disabled**. You must open the panel (`F1`) and enable the ones you want before turning the bot on (`F7`).

---

## Requirements

- **Windows** only.
- Firestone installed via **Steam** or **Epic Games**.
- **MelonLoader V0.7.3 or higher** — older versions are not compatible and will cause errors.
- Works in any game language, any resolution, and can run in the background.

---

## Installation

### 1) Install MelonLoader

1. Download the installer: [direct download](https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.Installer.exe) or from the [releases page](https://github.com/LavaGang/MelonLoader/releases/latest) (click **Assets** → `MelonLoader.Installer.exe`).
2. Run the installer.
3. When asked for the game executable, select your `Firestone.exe`.
4. Wait until it confirms success.

> Keep **Enable Nightly builds** checked during installation.

### 2) Install Firebot

1. Open the [Firebot releases page](https://github.com/danilogmoura/firestone-bot/releases/latest).
2. Click **Assets** and download the `.zip` named after the version (example: `0.4.0-alpha.1.zip`).
3. Extract the zip into the Firestone root folder (same folder as `Firestone.exe`).
4. Allow overwrite if Windows asks.

After this, the folder `Mods/` contains `firebot.dll`.

> **Never download the source code.** The rows `Source code (zip)` and `Source code (tar.gz)` inside **Assets** are the project code, not the mod.

### 3) First launch

1. Start Firestone normally.
2. Wait for the game to fully load.
3. Press `F1` to open the settings panel.
4. Enable the tasks you want to use. **By default, all tasks are disabled.**
5. Press `F7` to toggle Firebot on.
6. Optionally press `F2` to see the task status screen.

> Firebot stays loaded and running, but it will only automate the tasks you enable in the panel.

---

## How to Use

| Key | Action |
| ----- | -------- |
| `F1` | Open the settings panel (configure tasks) |
| `F7` | Toggle Firebot on/off |
| `F2` | Open the task status screen |

<p align="center">
   <img src="docs/panel.png" alt="Firebot settings panel (F1)" width="45%" />
   &nbsp;&nbsp;
   <img src="docs/status.png" alt="Firebot task status screen (F2)" width="45%" />
</p>

<p align="center">
   <sub>Left: the settings panel (F1) | Right: the task status screen (F2)</sub>
</p>

- **Settings panel (`F1`):** every task can be enabled, disabled and tuned inside the game. No file editing needed. **All tasks start disabled**, so enable the ones you want before turning the bot on.
- **Status screen (`F2`):** one row per task with its state, time left, next run and last run.
- **AutoSkill** and **AutoUpgrade** have their own hotkeys (`F8` and `F6` by default).

---

## How to Update

1. **Close the game completely.**
2. Download the new release `.zip`.
3. Replace **only** `Mods/firebot.dll` with the new one.
4. Start the game again.

- Do **not** replace the whole `MelonLoader` folder.
- Do **not** clean the cache. It is not needed for a bot update.
- Keep only **one** `firebot.dll` in `Mods/`. Delete old or duplicated copies (e.g. `firebot (1).dll`, `firebot.old.dll`).

If a new version adds options, they are added automatically to your `FirebotPreferences.cfg` on the next launch.

---

## Log and Debug Mode

- The normal log is short by design: it only carries errors, file changes, and the start/stop of the bot and its actions.
- For detailed info (task table, timings, popup sweep measurements, reasons a task is held back), enable **Debug Mode** in the panel: **General → Debug Mode**.

---

## Configuration

Use the **in-game panel (`F1`)**. Every setting is there, sections start collapsed, and what you change is saved immediately.

The panel writes to `Firestone/UserData/FirebotPreferences.cfg`. Editing the file by hand is supported, but you must close the game first — the panel rewrites the file whenever a setting changes. See [ADVANCED.md](ADVANCED.md) for the full file reference.

---

## FAQ

**The bot did not load, or `F7` does nothing.**
Check `Firestone/MelonLoader/Latest.log`. It usually points to an installation or configuration issue.

**The game updated and Firebot stopped working.**
See [ADVANCED.md](ADVANCED.md) → *Recovery Methods*.

**Do I need to clean the cache to update Firebot?**
No. Cache cleanup is only for when the **game** updates and Firebot stops working.

**Where is my Firestone folder?**

- **Steam:** right-click Firestone in your library → **Manage** → **Browse local files**.
- **Epic Games:** Library → `...` on the Firestone card → **Manage** → **Open install location**.

**The log looks shorter than before. Is that a bug?**
No. That is the new rule. Turn on **Debug Mode** if you want the detailed log back.

---

## Help

- **Discord:** [discord.gg/JB8RJAs6J3](https://discord.gg/JB8RJAs6J3)
- **Bug reports / feature requests:** [open an issue](https://github.com/danilogmoura/firestone-bot/issues/new/choose)

---

## For Developers

Building from source, project structure, and contribution guidelines live in [CONTRIBUTING.md](CONTRIBUTING.md).
