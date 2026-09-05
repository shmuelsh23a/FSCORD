# FSCORD — archive & design repo

**This file is for any coding agent** (Claude Code, Codex/ChatGPT, Cursor,
Aider, a human).

This repo holds the **original 2015 Unity 5 project, the design authorities,
and the planning documents**. It is not where the game is built. The active
project is the Unity 6 rebuild in `פרוייקט/Unity Folder/FASCORD Modern/` — an
**independent nested git repo**, gitignored here.

**If you are here to work on the game, go there instead** and read
`פרוייקט/Unity Folder/FASCORD Modern/AGENTS.md`.

## The authorities live here

- `פרוייקט/Game Design/Project FSCORD.xlsx` — the master stats table, **single
  source of truth** for every unit and ammo number. Nothing invents numbers;
  new units get a row in 'Planned Units' before implementation. The game's
  baked config is *generated* from this workbook, never hand-written.
- `פרוייקט/Game Design/FSCORD_GDD_Mechanics.md` — the living GDD. **Every
  mechanics change and every owner ruling gets one dated entry.** Never
  silently reverse a recorded ruling; propose an amendment and say it is one.
- Root: `FSCORD_Modernization_Roadmap.docx`, `FSCORD_StageA_Backlog.xlsx/.csv`.

**A stale duplicate of `Game Design/` exists** at
`פרוייקט/Unity Folder/Game Design/`. Do not edit that one. Check which copy is
current before touching either, and don't let them drift.

## Do not

- Modify the 2015 project in place (`פרוייקט/Unity Folder/FSCORD/`). It is
  reference only — data comes out through
  `FASCORD Modern/tools/extract_original.py`.
- `git add -A` from this root expecting it to pick up the nested modern repo.
  Two repos, committed separately, each from its own root. A mechanics change
  usually means two commits: code there, the dated GDD entry here.
- Trust any retired Google Drive copy of this project. Local disk is
  authoritative; the multi-machine workflow is git push/pull only.

## Environment

Windows, PowerShell 5.1 — **no `&&`**, chain with `;`. Python via the `py`
launcher. These paths contain Hebrew: set `PYTHONUTF8=1`, and read files as
UTF-8 explicitly (PowerShell 5.1 misreads no-BOM UTF-8 as ANSI).
