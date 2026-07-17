# FSCORD — Game Design Document (mechanics, living)

**Created:** July 2026.
**Rule:** every change or addition to game mechanics is recorded HERE. Unit stats
live in `Project FSCORD.xlsx` (same folder) — the **single source of truth** for
numbers; this document never duplicates stats, it references them.
Implementation planning lives in the repo: `FASCORD Modern/docs/FSCORD_StageB_FeaturePlan.md`.

Entries are dated and marked PLANNED / IN DEVELOPMENT / SHIPPED.

---

## 2026-07 — Era system (PLANNED)

Six playable eras, each a **ruleset expressed as data** (availability flags +
modifiers per unit/weapon class — no code per era):

| # | Era | Ruleset sketch (availability) |
|---|-----|-------------------------------|
| 1 | World War 2 | No drones, no ATGMs, no helicopters; tanks, infantry, AT rifles, artillery, prop CAS |
| 2 | Korea | + transport helicopters, recoilless AT, early jets CAS; no ATGMs/drones |
| 3 | Vietnam | + attack helicopters, early ATGMs, napalm-heavy CAS; no drones |
| 4 | Late Cold War | Baseline FSCORD (1983): MBTs, IFVs, ATGMs, attack helos, tactical nukes; no drones (recon only via listening posts) |
| 5 | Post Cold War | + recon UAVs, precision munitions, modern MBTs |
| 6 | Future | + strike UAVs/loitering munitions, drone counters (jamming), next-gen armour |

An era defines: which unit classes and ammunition types exist, which fire
missions are available, and tuning modifiers. Historical campaign **events** are
assembled from: a map set + a generated story + an era ruleset (see live-ops
plan). Exact per-era availability matrix: to be finalized in `Project FSCORD.xlsx`
('Planned Units' sheet, "Available in eras" column is the draft).

## 2026-07 — Air power integration (PLANNED)

New **Air domain** alongside ground combat:

- **UAVs:** recon (fog-of-war reveal, no weapon) and strike (Future era).
- **Helicopters:** attack (standoff anti-armour) and transport (delivers infantry).
- **Air-delivered munitions:** CAS strikes called in like fire missions; payload
  varies by era (bombs → napalm → precision).
- Mechanics questions to resolve at design time: altitude/targetability of air
  units by ground forces, air defence counters per era, whether the player or
  the AI (or both) get air assets.

## 2026-07 — Infantry (PLANNED)

New unit classes: **dismounted squads** (capture/garrison, weak vs armour),
**ATGM teams** (long-range AT; era-equivalent variants back to WW2 AT rifles),
**APCs** (battle taxi, carries one squad), **IFVs** (carries a squad and fights).
New mechanic: **mount/dismount** (transports carry squads; squads deploy).
Stats: `Project FSCORD.xlsx` → 'Planned Units'.

## 2026-07 — Fog of war: uncertainty model (DESIGN NOTE, PLANNED)

Design intent (Shmuel): think of fog of war in terms of **Heisenberg's
uncertainty principle** — enemy presence is *probabilistic until observed*.

- The player knows the **general possible location** of enemy forces: an
  uncertainty area (e.g., a blob/heat zone on the map), not a hidden-but-exact
  dot. Unobserved contacts are displayed as regions of possibility.
- **Exact position AND type/strength resolve only on direct observation**
  (line of sight from friendly units / listening posts / recon assets). Until
  observed, a contact might be a T-55 platoon or a T-72 company — the player
  commits fire missions against probabilities, not certainties.
- Implications to explore at implementation time: uncertainty areas can grow/
  drift while unobserved (contacts move); observation quality could affect how
  much resolves (sound contact → "armour, strength unknown"; visual → full ID);
  smoke and terrain re-widen uncertainty; recon (listening posts, later UAVs)
  is the counter-play and gains a clear role in the loop.
- Interaction with existing plans: fire-missions-only-in-revealed-area (HANDOFF
  §10 design choice) may soften into "fire into uncertainty at your own risk" —
  decide when building. F2's generator and F5 era recon rules should treat the
  uncertainty system as the perception layer they compose against.

Status: design lens for the Stage A fog-of-war task — record here first, per
process rule; nothing implemented.

## 2026-07 — Smoke ammunition (PLANNED)

Two artillery ammo types added to the damage sheet (zero damage, utility):
**smoke single shell** (blocks line of sight in a radius) and **smoke screen**
(line mission, smoke wall). First non-damage fire missions; interact with fog
of war and AI perception. Radius/duration TBD in the damage sheet footnotes.

## 2026-07 — Consumables (PLANNED)

Run-scoped limited items (roguelite): **better tank shells** (temporary friendly
buff), **fast smoke dischargers** (defensive auto-smoke on targeted tanks),
**repair trucks** (spawnable support unit, heals armour). Detail:
`FSCORD_StageB_FeaturePlan.md` §"Planned content backlog".

---

## Shipped mechanics baseline (2015 → Stage A parity)

Gesture fire missions (HE / concentrated / napalm / daisy cutter / mines / nuke),
wave defence, control points, fog of war + listening posts, tank-vs-tank ground
combat. Stats baseline: `Project FSCORD.xlsx` (damage + tank sheets) as extracted
into the repo (`docs/original-settings.md`).
