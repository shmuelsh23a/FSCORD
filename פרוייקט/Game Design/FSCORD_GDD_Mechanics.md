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

Status: implemented; the owner approved the demo visuals (red rings + opaque
grey fog overlay) in the 2026-07-18 play-test and they were promoted to the real
presentation layer (`FogOfWarView`). A shader-based overlay is Stage B polish.

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

## 2026-07-18 — HUD, menus, pause & restart (IN DEVELOPMENT)

First real presentation layer (state-event driven; placeholder visuals — real UI
art is Stage B). Mechanics-relevant decisions:

- **Start gate.** A match no longer begins at scene load: a start menu publishes
  a start request and the engine begins only then. Gives A4 (save/analytics)
  and Stage B (run setup, loadouts) a natural pre-match hook.
- **Pause is a full sim freeze.** `Time.timeScale = 0`; fire missions received
  while paused are **rejected, not queued** (no pause-aim exploit), and taps
  that land on UI never become fire gestures.
- **Restart** (pause or end screen) tears the match down and reloads the scene;
  persistent services survive.
- **Information surface.** The HUD shows: wave number/total, an **aggregate
  enemies-remaining count**, mission countdown, per-control-point ownership pips
  (tinted toward the capturer while a flip is in progress; contested points
  freeze, so a frozen arc/tint reads as contested), and a rate-limited CONTACT
  toast on first sight. Note: the aggregate enemy count is global knowledge the
  fog does not hide — judged acceptable (the original showed wave composition
  up front); revisit if it undermines the uncertainty model.
- **On-map control point markers.** Each point draws its capture radius as an
  owner-coloured ring plus an inner capture-progress arc in the capturer's
  colour — the map, not the HUD, carries the where/how-far of a flip.

## 2026-07-18 — Weapon behaviours ported (IN DEVELOPMENT)

All six fire missions now run their 2015 behaviours on the new engine, using
the shipped values from the master stats table (process rule 1 — numbers live
in the table/extracted config, not here):

- **Napalm** is a line-mission burn strip: curve damage **per second** for the
  burn duration. Burning resolves against true positions — it hurts tanks the
  fog is hiding, same no-fire-gate philosophy as everything else.
- **Mines** are discrete one-shot charges laid along the line (one per
  trigger-diameter). A triggering tank takes blast damage and is **stopped**
  for the stop duration (it neither moves nor fires — 2015 "stops tank 5 s").
  Unspent mines persist to match end and are visible to the player (their own
  ordnance is always known).
- **Concentrated strike** = slow motion (0.5×, 5 real seconds) plus a burst of
  6 HE-curve shells scattered uniformly inside the **actual circled area** —
  the circle gesture now measures its radius and passes it through. Slow-mo
  and pause coexist: pause always wins; slow-mo re-asserts on resume.
- **Daisy cutter / nuke** stay instant point detonations distinguished by
  their own damage curves.
- **Ammo is per weapon** and finite where the table says so (napalm 5,
  mines 5). The 2015 random drops of concentrated/daisy/nuke
  (chance-of-appearance) are NOT ported — that economy belongs to the
  roguelite consumables design (Stage B); the demo grants fixed test ammo
  (concentrated 2, daisy 1, nuke 1) until then.
**Owner rulings after first review (2026-07-18, same day — supersede the two
provisional notes above):**

- **Enemy AI vs napalm:** burn strips are VISIBLE ordnance — every tank sees
  them and steers around them while advancing.
- **Enemy AI vs mines:** mines are partly concealed. An enemy tank detects any
  given mine **20% of the time** (rolled once per tank per mine and
  remembered) — detected mines are steered around, undetected mines are driven
  through (and trigger). **The 20% is a PLACEHOLDER** to be tuned in
  play-testing; promote it to a master-stats-table column when formalized.
  Own-faction ordnance is always known (friendlies never blunder into their
  own mines by ignorance).
- **Friendly fire is ON:** all player fire missions — detonations, napalm
  burn, mine blasts — damage friendly units and listening posts exactly like
  enemies, and mines trigger on ANY tank regardless of faction (listening
  posts, being static emplacements, do not set them off). Shelling your own
  line is a player error the simulation honours.

## 2026-07-18 — Annihilation defeat (IN DEVELOPMENT)

Owner ruling after play-test: losing every friendly unit left the match in a
dead-air limbo (the Soviets still had to drive to the line and hold it 8 s
while the player watched with nothing to do). New rule:

- **When every friendly combatant is destroyed — tanks AND listening posts —
  the match ends in immediate defeat.** No one is left to contest anything;
  the line is functionally lost the moment it is annihilated.
- The rule is per-match data (`defeatOnFriendlyAnnihilation`, default ON) and
  only arms once friendlies have existed, so a future **artillery-only "last
  stand" mode** (player fights blind with fire missions alone — all sensors
  dead means full fog) stays possible by turning it off.
- With friendly fire ON, nuking your own line now loses the match on the
  spot. Working as intended.

## 2026-07-18 — Player role: you play the FSCORD (RULING)

Owner ruling, settling the "do blue tanks move?" question:

- **All tanks, both factions, are AI-driven — always.** The player never
  issues movement or unit orders. There is NO player unit-command layer, and
  none will be added; de-scope any future suggestion of one.
- **The player is the FSCORD** — the Fire Support COoRDinator the game is
  named for. The player's levers are exactly: fire missions (the six weapons),
  ordnance placement (mines/napalm as area denial), and sensors (listening
  posts, later SIGINT/UAV consumables). Influence the battle; never drive it.
- **Friendly maneuver intelligence is B3 work** (Stage B tactical AI, lands
  alongside F2 level generation): repositioning between cover, fallback
  lines, counterattacks — authored as Behavior graph tactics over the same
  TankBrain primitives, for both factions. Until then friendlies keep their
  static hold-the-line orders (a standing order, not a capability gap — the
  brain and movers are already faction-agnostic).

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

## 2026-07-18 — Daily challenge: seeded generated battlefields (IN DEVELOPMENT, F1+F2 P0)

The first live-ops content unit ships (investor-demo build): a **daily
challenge** = one seeded match, delivered as data with no client update.

- **Delivery:** Remote Config key `liveops.dailyChallenge` carries
  `{seed, era, label, modifiers{difficulty, waves}}`. Offline/unreachable
  backend falls back to a **date-derived seed** — same date, same battlefield,
  on every client. The start menu shows the challenge label; a challenge
  payload arriving after the battlefield is built applies next launch.
- **Generation:** the battlefield is assembled deterministically from the
  seed (same grammar as FuldaGap: southern friendly line, 3-6 northern spawn
  lanes with approach corridors, 3-5 midfield control points anchored on the
  corridors, 1-2 listening posts, line CP at the objective). Same seed →
  identical battlefield, any machine.
- **Waves as budget:** wave composition is bought from a difficulty budget
  priced in master-table `pointsValue` (T-55 100 / T-62 200 / T-72 500);
  the baseline wave budget equals the original FuldaGap wave (1700). Later
  waves get +12% budget each and a heavier mix. `modifiers.difficulty`
  multiplies the budget (clamped 0.25-4); `modifiers.waves` sets wave count
  (1-8, default 3). Time limit keeps the original pacing: 100 s per wave.
- **Defeat rule for generated maps:** generated control points sit ON the
  approach lanes (FuldaGap's three sit off-route), so the FuldaGap rule
  "one enemy capture = defeat" would end matches in transit. Ruling:
  **generated matches are lost when the enemy holds a MAJORITY of control
  points** (total/2 + 1, the line CP included). FuldaGap keeps its original
  rule. Placeholder pending play-test tuning.
- The hand-built FuldaGap demo is unchanged and stays the rehearsed fallback
  (`dailyChallengeMode` off on the demo bootstrap).

## 2026-07-18 — Demo era rulesets + battlefield validation (IN DEVELOPMENT, F5/F2 P1)

- **Era rulesets (demo slice of F5):** a challenge payload's `era` picks a
  pure-data ruleset multiplied over the OriginalStats baseline. Demo set:
  `ww2` (speed ×0.7, HP ×0.8, damage ×0.7, sight ×0.8; no tactical nuke, no
  concentrated strike), `late_cold_war` (the baseline, identity), `future`
  (speed ×1.4, HP ×1.1, damage ×1.2, sight ×1.5, everything fielded).
  Era-disabled weapons ship with ZERO AMMO (shown dry on the HUD) rather than
  vanishing. **Numbers are demo-throwaway, plausible-not-balanced** — per the
  demo plan they are recorded here, not in the master stats table; the full
  six-era F5 catalog gets proper master-table columns after the demo. Unknown
  era ids fall back to the baseline.
- **Battlefield validation:** every challenge seed walks a deterministic
  candidate chain (seed, then derived candidates); each candidate passes
  geometric checks (no spawn inside the defended zone) and a headless
  first-contact simulation (wave-1 attackers advance their corridors against
  the line, both sides trading fire on era-adjusted stats). Rejected as
  degenerate: line unreachable within the time limit, line annihilated inside
  60 s ("walkover"), empty waves. The generate→reject→accept funnel is logged;
  same seed → same survivor on every client. If every candidate fails, the
  base seed plays unvalidated (never refuse a challenge).

## 2026-07-18 — Two play modes + faction-war campaign (RATIFIED, owner design)

The product's mode structure is settled (engineering scoping: feature plan §F8
+ amended §F3/§F5):

- **Skirmish** — persistent roguelite. Player picks an era per run; each era
  has its own persistent upgrade ladder; ONE consumable inventory shared
  across eras, filtered by era availability. **Era-irrelevant consumables are
  HIDDEN, not shown empty** (ruling; the demo's dry-weapon presentation was a
  demo-only choice — post-demo everything era-gated hides).
- **Campaign** — live-ops faction-war events on famous battles, scheduled on
  the real calendar week the battle happened; overlapping history alternates
  wars by year. Side chosen at event onset — Axis playable per genre norm,
  **no Nazi symbols or glorification** (ratings-conscious). Victories score
  for the side **per-capita**; winning side announced at event end; global +
  individual rewards, plus personal milestone rewards on both sides. Event
  runs = 10 missions, in-run upgrades each mission, event ladder zeroes at
  event end; event consumables are a separate inventory that converts to
  skirmish at event end through a **capped/diminishing conversion**.
- **Runs (both modes):** generated story frame per run (public-domain corpus,
  pool pre-generated offline per event, human-reviewed, no real names);
  the arc fixes each mission as defensive or offensive in advance (offensive
  needs B3 friendly-maneuver AI — defensive-only Campaign v1 is the cut
  line); ammo/surviving-roster state carries within a run, never between;
  **mid-run autosave + mid-run abandon are P0**.
- **Terrain:** campaign maps derive from open geodata only (Copernicus/SRTM +
  OSM with attribution). Google Earth/Maps data prohibited (licensing).
- **Integrity:** event scoring and rewards are server-authoritative (Cloud
  Code validation against the analytics trail).
- **Economy philosophy:** relatively generous with consumables — reduce
  hoarding stress so spending feels natural — bounded by the conversion cap.

## 2026-07-18 — Playable skirmish runs, v1 rules (IN DEVELOPMENT, F3 wired)

The F3 run framework is now playable in the demo scene (`runMode` toggle;
daily-challenge mode required; OFF = the single-mission demo). V1 rules,
placeholders pending the F3 economy pass:

- **A run = N seeded missions** (product: 10) derived from the challenge seed;
  each mission's battlefield goes through the full generator + validation
  funnel. The start menu shows "MISSION k/N — <story title>".
- **Between missions: choose resupply.** Three deterministic options from the
  run seed (e.g. +2 napalm / +2 mines / +1 nuke) — the same run at the same
  mission always offers the same choices; restarting the app cannot reroll.
  V1 upgrades are ammunition only; stat/consumable upgrades join with the F3
  reward economy.
- **Carry:** remaining ammunition and the surviving tank line carry to the
  next mission. **No healing, no reinforcements** — losses are permanent for
  the run (repair trucks are future consumable territory). The next mission
  fields at most the layout's line, never fewer than one tank.
- **Defeat ends the run** (roguelite rule). Restarting mid-mission retries
  the CURRENT mission with the state you entered it with (the autosave is
  per-transition, not per-attempt).
- **Abandon** (pause menu, run mode): ends the run, currently keeps nothing —
  what an abandoned run keeps is a reward-policy decision for the F3 economy.
- Autosave after every transition; a run resumes across app restarts; a
  finished/abandoned run never resumes.

## 2026-07-19 — Terrain blocks sight and direct fire (real-relief maps)

Owner-reported after the first real-terrain play-test: units saw and fired
straight through hills. Ruling — on maps with relief (open-geodata modules),
the terrain participates in perception and gunnery:

- **Sight requires line of sight.** A fog sensor (friendly unit / listening
  post) only DETECTS an enemy inside its sight radius if the relief does not
  block the line between them (sight line at ~2.5 units eye height, so low
  crests don't flicker it). An enemy inside sight radius but LOS-blocked is
  a HEARD contact with the tightest uncertainty circle — close and loud,
  just unseen behind the crest.
- **Hearing is unchanged** — explicitly non-LOS (rev. 2 uncertainty model):
  hills muffle nothing.
- **Direct fire requires line of sight.** A tank does not stop and engage a
  target it cannot see over the relief; it keeps advancing until the terrain
  opens up. Applies to both factions symmetrically (the M-60 line benefits
  when hull-down, suffers when the approach is dead ground).
- **Fire missions are unaffected** — player artillery is indirect by nature;
  firing into dead ground is exactly the counter to it.
- **Flat maps are byte-identical in behaviour** (LOS is always clear when the
  world has no relief), so FuldaGap classic and generated maps play as before.
- Presentation follows the same day: control-point rings/arcs and napalm
  strips drape per-vertex over the relief instead of clipping into slopes.

## 2026-07-19 — Six-era master table: M60A3 anchor + rulings (RATIFIED, owner design)

The full F5 pass replaces the demo-throwaway era multipliers. Numbers live in
`Project FSCORD.xlsx` (as always); research, per-unit derivations and the
computed proposal live in `FASCORD Modern/docs/EraMasterTable_Proposal.md`.

**The anchor ruling:** the **M60A3 TTS is held at index 100 in every stat
column**; every unit in all six eras is indexed against it on absolute physical
scales — Armour = frontal RHA-e / 250 mm; Damage = penetration at era-typical
combat range / 400 mm (KE or HEAT, whichever is the type's primary killer);
Hit = fire-control band F0–F6 (WW2 telescopic 0.35 → ~2035 AI-assisted 0.95;
M60A3 = 0.70); Sight = effective engagement range / 2000 m; Speed = road km/h
/ 48; HP = passive survivability judgment (weight, ammo layout, crew capsule);
Points = one formula over the other stats — never hand-set. The anchor's
in-game row is unchanged by construction. Rosters are per-state (3–5 units),
so campaign events can pit any two states of an era (UK vs Germany '44,
US vs Japan '44...). Future-era (~2035) rows are speculative by declaration.

**Combat-model rulings:**
- **Per-era damage scale:** absolute indexing makes early-era time-to-kill
  balloon (a WW2 Sherman needs 30+ hits on a Pz IV). The master table keeps
  the pure index; `EraCatalog` carries a per-era `damageScale` (data) tuned so
  within-era TTK matches the Late-Cold-War baseline. Validator sim verifies.
- **Damage floor:** damage resolves as `max(1, damage − armour)` — under-gunned
  shots are near-useless (emergent no-pen stays meaningful) but never zero.
- **Legacy numbers superseded:** the original five tanks are re-derived under
  the same methodology — consistency over grandfathering.
- **Wave budget rebases per era:** the generator's flat 1700-point base is
  replaced by the cost of each era's canonical wave mix in NEW points.
- **Loader type is modeled:** autoloader = fixed load cycle (immune to fatigue
  and motion); manual = burst advantage that decays. Column in the table.
- **Horsepower affects climbing:** hp/t is a table column; on relief maps,
  slope speed scales with it.
- **Speed derives from road km/h** — the extracted 2015 NavMesh speeds
  (M1 = 3.5 u/s) are placeholder artifacts, superseded.

**Progression (owner design, new):**
- **Per-tank run XP:** each tank accumulates XP within a run; XP improves its
  speed, loading, aiming and effective range by ~5–15%, always within the
  era-relative scale. XP lives and dies with the tank (losses stay permanent).
- **Event upgrades:** 1–10% stat improvements offered as upgrades, accumulated
  over the course of a campaign event.

**Capabilities tracked now, mechanics later:**
- **APS** (Trophy / StrikeShield / GL-6 / Afganit / soft-kill Shtora-GALIX):
  a column, NOT baked into HP or Armour; the mechanic ships together with
  infantry ATGMs (F6/F7), where APS defeats incoming ATGM/top-attack shots.
- **Gun-launched ATGMs** (Kobra / Svir / Refleks / GP125 / Sprinter): column;
  ships with the same F6/F7 milestone.
- **Smoke dischargers:** per-tank tube count (+ Soviet TDA exhaust smoke);
  mechanic arrives in the polish phase.
- **Machine guns:** every tank row references coax/flex MG family rows;
  standalone MG rows enter 'Planned Units' for F7 infantry (damage-vs-infantry
  derived when infantry HP exists).

**Roster rulings:** Vietnam's 4th US slot is the M67A2 Zippo flame tank (the
M60 never served in Vietnam); Korea includes the IS-2 as a rare "boss" (no
confirmed tank duels — fiction-flagged) and the Cromwell as a limited unit;
Late Cold War includes France (AMX-30B2); Post Cold War includes the T-72B3
as an epilogue variant; the Future speculative tier (MGCS/CAPINT, RCV-L, T-14)
ships as rare/prototype units.

## 2026-07-20 — Campaign onset: side choice, per-state matchups, event terrain (IN DEVELOPMENT, F8; derivation rulings RATIFIED same day)

The campaign-mode client wiring lands (§F8 rules unchanged — this implements
them). The three derivation calls were put to the owner and RULED same day —
see the rulings block at the end of this entry.

- **Side is chosen at event onset and FIXED for the event** (§F8, ratified
  2026-07-18): a chooser screen appears once per event, before its first run;
  the choice persists locally (and later server-side) — every run of that
  event fights for it, and switching sides mid-event is not offered
  (per-capita scoring would invite side-hopping). Abandoning or finishing a
  run never re-opens the choice.
- **Sides map to real states** (`sideStates` in the battle registry, e.g.
  allies→USA, axis→Germany): the player's state DEFENDS (defensive-only
  Campaign v1), the opposing state attacks. Older payloads without the
  mapping fail soft to the era's default matchup.
- **Any state pair becomes a matchup** derived from the master table —
  derivation rules per the owner rulings below.
- **Event terrain by region id:** a campaign event's `terrainRegionId`
  resolves its baked open-geodata module (+relief) from the in-project module
  library; events without terrain (overlord, pending coastal grammar) play on
  generated battlefields. The skirmish/daily paths are unchanged.
- **Scheduling is live:** the active event is picked from the fetched
  `liveops.eventCalendar` by ISO week + year alternation + expiry (baked
  in-project calendar as the offline fallback) — no hand-assigned event ids
  outside dev hooks. **Schema-upgrade rule (2026-07-20):** the live payload
  is authoritative for everything it carries, but fields it predates
  (sideStates, historicAttacker) are backfilled per event id from the baked
  shipped copy — an old production payload can delay a new field, never
  mask it.
- **Runs now flow through the IMissionGateway seam** (F3 architecture
  promise): the run layer launches each mission through the gateway and
  receives its result — the scene flow is an implementation detail behind it.
  A run resumed mid-resupply re-offers the interrupted choice.

**Owner rulings on the matchup derivation (2026-07-20, RATIFIED):**

1. **Wave weights — RATIFIED as proposed.** Derived attacker rosters use
   every unit the state fields in the era, wave weights points-ranked on a
   linear ramp (cheapest leans early, priciest leans late). Hand-tuned era
   rosters remain the reference shape for skirmish.
2. **Line unit = the state's CHEAPEST tank, not the median.** Roguelite
   principle: **events start weak and you improve.** The intended economy
   (F3 pass): the player **buys more tanks between missions of the same
   run**, and **improves the starting tanks between runs of the same
   event** — so opening an event on the weakest chassis is the point, not a
   bug. Consequence: EVERY campaign matchup is derived — including the side
   whose states match the era's skirmish default (skirmish keeps its
   hand-picked mainstay lines; campaign starts both sides weak).
3. **NO per-matchup damage re-normalization.** The era's ratified
   damageScale (2026-07-19 anchor calibration) applies to every matchup in
   the era, unchanged. Matchup asymmetry is INTENDED: different sides and
   different eras must feel different — no reverting to the mean. It is
   fine that a Sherman needs many shots to kill a Tiger; **the player's
   fire support is meant to be the main tank-killer**, and a hard-to-crack
   enemy makes the player work their own guns harder. (The 6/3/1 wave
   budget IS still priced in the derived roster's own points — that is
   wave-size bookkeeping, not feel normalization.)

## 2026-07-20 — Event run progression: historic onset, role arcs, gap-driven mix (RATIFIED, owner design)

Owner design for how campaign runs evolve over an event's life:

- **Events open like the historic battle** — one side attacks, one defends
  (registry `historicAttacker`: bulge → axis, overlord → allies). At event
  start your runs open in your side's historic role.
- **Each run is a phase arc, not a flat list:** defend → hold → counterattack,
  or attack → hold → defend. **Hold** is a new mission role — the static
  middle phase (defence of the current positions; plays as a defend mission
  until the blue AI distinguishes them, see below).
- **The war standings shape the mix per run:** the WINNING side's runs open
  with attacks and transition to defence LATER the larger its lead — a
  runaway lead means most of the run is attacking; the losing side defends
  first and counterattacks late. Dead even → the historic onset decides the
  template. Implementation: `RunController.PlanRoleArc(missionCount,
  attacksAtOnset, scoreGap)` — deterministic; first phase grows from ⅓ to ⅔
  of the run with the normalized gap, hold ≈ ⅙, remainder tails.
  **War-standings feed (same day, IN DEVELOPMENT):** a read-only Cloud Code
  endpoint (`FSCORD_GetEventStandings`) reports each side's per-capita total
  from the score leaderboards; the client fetches it at event resolve and
  waits up to ~4 s at run-plan time. **Gap formula (design knob):** the
  relative per-capita lead over the strongest other side —
  (mine − theirs) / max(mine, theirs), clamped to ±1 — so a 2:1 lead reads
  +0.5 at any score scale; an unopposed side reads +1; a silent scoreboard
  (event just opened, or offline) reads 0 = the historic-onset template.
  Endpoint deploys once (registry-independent); until the owner publishes
  it, the null stand-in keeps every arc on the historic template.
- **Story frames keep supplying titles/briefs;** campaign mission ROLES come
  from the arc (skirmish runs keep the frames' authored roles). Pools may
  now also author 'hold' roles.
- **B3 gate unchanged:** Offensive missions still coerce to Defensive until
  the friendly-maneuver AI ships (sanctioned Campaign-v1 cut); Hold plays.

**Blue-force AI note (PLANNED, lands with B3 blue AI design):** blue AI and
STARTING POSITIONS must match the mission role — a defending blue force
falls back when overrun; an attacking blue force advances. Recorded now so
the B3 design starts from it.

---

## Shipped mechanics baseline (2015 → Stage A parity)

Gesture fire missions (HE / concentrated / napalm / daisy cutter / mines / nuke),
wave defence, control points, fog of war + listening posts, tank-vs-tank ground
combat. Stats baseline: `Project FSCORD.xlsx` (damage + tank sheets) as extracted
into the repo (`docs/original-settings.md`).
