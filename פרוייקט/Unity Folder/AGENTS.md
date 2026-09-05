# FSCORD workspace — session root

**This file is for any coding agent** (Claude Code, Codex/ChatGPT, Cursor,
Aider, a human). `CLAUDE.md` beside it is a pointer to this file, not a second
version.

This folder is the launch point for FSCORD work. The active project is
**`FASCORD Modern/`** (the Unity 6 rebuild, its own git repo) — **read
`FASCORD Modern/AGENTS.md` first**: it is the operating manual (authorities,
agent boundaries, commands, conventions, gotchas). Then
`FASCORD Modern/HANDOFF.md` for current state.

## What the sibling folders are

- `FSCORD/` — the original 2015 Unity 5 project. Reference only: data is
  extracted from it via `FASCORD Modern/tools/extract_original.py`, never
  modified in place.
- `Game Design/` — **stale partial duplicate, do not edit.** It holds only
  concept and game-text documents plus an old roadmap copy; the two
  authorities are *not* in it. Don't go looking for them here.
- **The canonical `Game Design/` is one level up**, at `..\Game Design\`:
  `Project FSCORD.xlsx` (the master stats table — single source of truth for
  every unit and ammo number) and `FSCORD_GDD_Mechanics.md` (the living GDD —
  one dated entry per mechanics change, owner rulings included). Edit only
  that copy, and don't let the two drift.

## Two repos, committed separately

The parent folder `C:\Unity\תיק עבודות\FSCORD` is its own git repo (design
docs, roadmap, backlog). `FASCORD Modern/` is an independent nested repo,
gitignored by the parent. Never `git add -A` from the parent expecting it to
pick up project code — commit each repo separately, from its own root. A
mechanics change usually means **two commits**: the code in `FASCORD Modern/`,
the dated GDD entry in the parent repo.

## Environment

- Windows. Interactive shell is PowerShell 5.1 — **no `&&`**; chain with `;`
  or separate lines. Git Bash is available for Bash-style commands.
- Python via the `py` launcher. Anything Hebrew needs `PYTHONUTF8=1` — these
  paths contain Hebrew, so assume it.
- Secrets live only in a gitignored `.env`. Never put a key or token on a
  command line, in a settings file, or in an MCP `--env` flag — pass an
  environment-variable reference instead.
- `.mcp.json` here loads the **Unity CLI** (`unity mcp`), which replaced the
  deprecated in-Editor MCP relay on 2026-07-20. Install it with
  `$env:UNITY_CLI_CHANNEL='beta'; irm https://public-cdn.cloud.unity3d.com/hub/prod/cli/install.ps1 | iex`
  — user-scope, into `%LOCALAPPDATA%\Unity\bin`, added to the user PATH. It is
  **beta-only**; no stable channel is published yet. Driving a running Editor is surfaced by
  `com.unity.pipeline`, now in the project at `0.6.0-exp.1` (experimental).
  Batchmode test runs need the editor **closed** (project lock); pick one mode
  per task and don't fight it.
