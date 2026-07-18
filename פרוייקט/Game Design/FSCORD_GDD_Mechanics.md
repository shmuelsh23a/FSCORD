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

## 2026-07-18 — Control points: capture rules (IN DEVELOPMENT)

Stage A implementation of the 2015 baseline mechanic; the specific rules chosen:

- **Capture by uncontested presence.** While exactly one faction has living units
  inside a point's radius, its capture meter fills (default 8 s). Full meter →
  the point flips to that faction.
- **Contested = frozen.** Both factions present inside the radius freezes the
  meter — you must clear the circle to take the point.
- **Empty or owner-held = drain.** An abandoned or reinforced point drains any
  attacker's progress back to zero; ownership never decays on its own.
- **Defeat threshold is data.** A match with the HoldControlPoints victory
  condition declares defeat when the enemy holds N points (per-match value;
  0 = all of them). Clearing every wave still wins. Radius/capture-seconds are
  per-point values.

## 2026-07-18 — Fog of war: uncertainty model implementation (IN DEVELOPMENT)

Implements the "Fog of war: uncertainty model" design note above. Decisions made
at build time:

- **Sight radius = engagement range.** In the original data the prefabs'
  sightRange fed the engagement range, so revealed area == engageable area. No
  separate sight stat is invented; if design later wants sight ≠ gun range, the
  master stats table gets a new column first (process rule 1).
- **Firing into uncertainty is allowed — at your own risk.** The old
  "fire missions only in revealed area" choice is dropped, as the design note
  anticipated: the player may shell a possible-location area on a gamble. No
  hard gate anywhere in the fire path.
- **Contacts exist from spawn.** A new enemy immediately appears as an
  uncertainty area at its entry point (the player has intel on where the enemy
  enters the map), not as nothing.
- **Unobserved contacts degrade.** Last-known position with an uncertainty
  radius that grows over time (defaults: base 10, +5/s, capped at 80 world
  units); direct observation collapses it to an exact, identified contact.
  Observation-quality tiers (sound vs visual) remain future work.
- **Destruction always resolves.** A contact that dies is removed even if
  unobserved — the strike/battle that killed it is assumed apparent.
- **AI perception is unaffected.** Fog is player-side only; units fight with
  their own sensors (TargetRegistry). Enemy-side fog remains a future design
  question.

**SUPERSEDED in part by the rev. 2 entry below** (owner play-test feedback):
the time-growth model and contacts-from-spawn are replaced by the sensor model;
the other decisions (sight = engagement range, no fire gate, destruction
resolves, AI unaffected) stand.

## 2026-07-18 — Fog of war rev. 2: full-map fog + hearing + shrinking red circle (IN DEVELOPMENT)

Owner direction (Shmuel, after the first play-test of rev. 1):

- **Full-map fog.** The entire map is under fog of war; only friendly *sight*
  areas are revealed. An enemy outside every sensor shows **nothing at all**
  (rev. 1's "contacts exist from spawn" is dropped).
- **Two-range sensor model.** Every friendly sensor (vehicles, listening posts;
  later SIGINT and recon UAVs as consumables) has:
  - **sight** = visual range (unchanged: = engagement range from the stats);
    full detection — exact position and identity;
  - **hearing = 2 × sight**, deliberately NOT limited by line of sight; yields
    only an approximate location. (No LOS occlusion exists yet on the flat
    demo map; when terrain arrives, sight becomes LOS-limited and hearing
    stays omnidirectional.) The 2× is a global rule, not a per-unit stat; if
    design later wants per-unit hearing, the master stats table gets a column
    first (process rule 1).
- **The red circle.** A heard-but-unseen enemy is a red possible-location
  circle whose radius **shrinks as the enemy closes** from the hearing edge
  (max radius, default 50 units) to the sight edge (min radius, default 6),
  where it resolves into a visible unit.
- **Heisenberg placement.** The enemy is *somewhere inside* the circle, never
  at its centre: the circle is anchored off the true position by a random
  offset (up to 75% of the radius), re-rolled each time a contact re-enters
  the heard state. Firing into the circle can still land on the real tank —
  damage always resolves against true positions (no fire gate, as before).
- **Camera.** Because the player can shell heard contacts beyond the visible
  line, the battle camera is movable (pan/zoom/rotate via the gesture intents;
  keyboard + scroll on desktop) and pitched at **80°** instead of straight-down.

Status: implemented in the demo (red rings + opaque grey fog overlay); real HUD
presentation is Stage A remainder work.

## 2026-07-18 — Listening posts are destructible (IN DEVELOPMENT)

Owner decision: listening posts are combatants, not scenery.

- A listening post has hit points and registers as an American combatant, so
  **enemy AI acquires and fires on it** like any other target; area weapons can
  hit it too. When destroyed, its reveal stops — the fog closes back over its
  area — making forward posts a real risk/reward placement.
- The player's own artillery cannot hurt it (fire missions only damage enemies).
- **Stats recorded (2026-07-18):** 150 HP, no armour, confirmed by owner and
  recorded as the 'Listening post' row in the master stats table
  ('Planned Units' sheet) — the code mirrors the table (process rule 1).
  Points-per-kill value still TBD in the row.
- Side effect to keep in mind: as a combatant, a post inside a control point's
  radius counts as American presence (it can hold/contest a point like a
  garrison). Judged acceptable for now; revisit if it distorts capture play.

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
