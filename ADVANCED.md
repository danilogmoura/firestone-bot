# Firebot — Advanced Guide

This file covers everything that is not needed for normal use: manual configuration, recovery methods, and building from source.

For installation, quick start, and everyday usage, see the main [README](README.md).

---

## Configuration File Reference

Settings live in `Firestone/UserData/FirebotPreferences.cfg`, and the in-game panel (`F1`) writes to that same file.

> **Disclaimer:** the panel is the recommended way to change any setting. Editing the file by hand is supported, but do it with the game **closed** — the panel rewrites the file whenever a setting changes, so a change made while the game is running overwrites what you typed outside.

### When to edit the file

The panel covers everyday cases. Opening the file is worth it when you want to:

- keep your settings under version control, or copy them to another machine;
- configure Firebot before its first launch;
- recover a setting by hand when the game does not open.

The file is read on the next launch, so a change takes effect only when you reopen the game.

### Format

```toml
[firebot_settings]      # global settings
shortcut_key = "F7"     # bot on/off hotkey; "None" frees the key

[alchemist]             # one section per task
enabled = true          # false = the task is skipped in the execution loop
resource_type = "0,1"   # comma-separated ids; empty = the task's default
```

- **The file documents itself.** The description of every option is written as a comment right above it, examples included, so the keys and their accepted values are always one line away and always current. The panel shows the short version of that same text, because the box at the bottom of the window holds about three lines — what does not fit there stays in the file.
- **Section names differ between the two surfaces.** The panel names a section after the feature it belongs to (`General`, `AutoSkill`, `Alchemist`), while the file keeps the identifier it always had (`[firebot_settings]`, `[auto_skill]`, `[alchemist]`). Renaming a section in the panel never renames it in the file, so an existing configuration keeps working.
- **A missing entry is added on the next launch**, which is how an older file keeps working after an update. There is no need to delete the file to get the new options.
- **An invalid value is never fatal.** The option is dropped and the task falls back to its default; the task that needs a value to know what to do, such as the alchemist resources, disables itself rather than guessing.
- **A task can be enabled and still not run.** Features the game only offers later are held back until then — Engineer tools and Free Pickaxes at level 50, Warfront Campaign at level 50, Alchemist at 120, Oracle at 200 — and everything else is available from level 1. This is a runtime condition and not a setting: the bot never rewrites your `enabled`, the task's own row on the status screen shows the level it is waiting for (`Locked (level)`), and it resumes by itself when the level arrives.

---

## Recovery Methods

Use these **only** if the game changes version and Firebot stops working. If Firebot is working normally, do not run any recovery method.

### Before You Start

1. Close the game completely.
2. Confirm your MelonLoader version is **V0.7.3 or newer**. If you are not sure which version is installed, follow Method 2 (clean reinstall).
3. Back up `UserData/FirebotPreferences.cfg` if you customized it. Neither method below touches this file.
4. In the `Mods` folder, keep only one `firebot.dll`. Delete old or duplicated copies from previous downloads (for example `firebot (1).dll` or `firebot.old.dll`).
5. Optional: delete `MelonLoader/Latest.log` so the next launch generates a clean log for troubleshooting.

### Method 1: Assembly Cache Cleanup

Starting from a closed game:

1. Delete all contents inside `MelonLoader/Il2CppAssemblies`.
2. In `MelonLoader/Dependencies/Il2CppAssemblyGenerator`, keep only:
   - `Il2CppAssemblyGenerator.deps.json`
   - `Il2CppAssemblyGenerator.dll`
3. Delete `MelonLoader/Dependencies/AssemblyUnhollower` (if it exists).
4. Start the game again.

### Method 2: Reinstall MelonLoader

Starting from a closed game:

1. Delete the `MelonLoader` folder from the game root. This removes the old MelonLoader version, its generated assemblies and its logs.
2. Reinstall MelonLoader **V0.7.3+** as described in the main README.
3. After reinstalling, delete `MelonLoader/Dependencies/AssemblyUnhollower` if it exists (leftover from older MelonLoader versions).
4. Start the game again.

> **Important:** no matter which method you choose, always replace only `Mods/firebot.dll` to update Firebot.

---

## Finding your Firestone Folder

MelonLoader, the Firebot zip and the `Mods/firebot.dll` of an update all go to the folder that contains `Firestone.exe`. To open it without hunting through the disk:

- **Steam:** right-click Firestone in your library → **Manage** → **Browse local files**.
- **Epic Games:** in the **Library**, click the `...` on the Firestone card → **Manage** → **Open install location**.

The folder you want is the one containing `Firestone.exe`: that is where the MelonLoader installer points and where the Firebot zip is extracted.

---

## Installation (From Source)

1. Clone the repository:

   ```bash
   git clone https://github.com/danilogmoura/firestone-bot.git
   ```

2. Navigate to the project directory:

   ```bash
   cd firestone-bot
   ```

3. Configure the path to your Firestone Idle RPG game directory by editing the `src/Directory.Build.props` file if needed:
   - By default, the path is set to `C:\Program Files (x86)\Steam\Firestone` (Windows). If your game is installed elsewhere, change the `<GameRoot>` property in this file to the correct path.
   - You can also set the environment variable `COMMON_DIR` to override the base directory. In this case, the game path will be `$(COMMON_DIR)\Firestone`.

   Example:

   ```xml
   <GameRoot>C:\Program Files (x86)\Steam\Firestone</GameRoot>
   ```

4. Build the project using your preferred method (e.g., Visual Studio, command line).

---

## Contributing

Contributions are welcome! Please submit a pull request or open an issue for suggestions or improvements.

---

## Bug Reporting & Feature Requests

Found a bug or have an idea for a new feature? Please open a ticket on our GitHub Issue Tracker.

**Before submitting a bug report:**

1. Check if the issue has already been reported.
2. Ensure you are using the latest version of Firebot.
3. Attach your **MelonLoader log** file (`Firestone/MelonLoader/Latest.log`) if the game crashed or the bot failed.

[**Open a New Issue**](https://github.com/danilogmoura/firestone-bot/issues/new/choose)
