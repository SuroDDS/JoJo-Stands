# TIME_STOP_SPEC.md — JoJo Stand System

**Target:** RimWorld 1.7 / experimental prototype  
**Status:** agreed design contract; **not implemented or compiled**  
**First Stand:** Star Platinum. Shared temporal infrastructure should allow The World later.

## 1. Decisions fixed by the user

| Parameter | Value | Meaning |
| --- | --- | --- |
| Nominal duration | **5 in-game seconds = 300 simulation ticks** at normal 60 ticks/s | Not 5 seconds of real time at every game speed |
| Deferred damage | **Enabled** | Accepted Stand strikes are stored and resolved when the stop ends |
| Freeze global world processes | **Disabled** | Day/night, storyteller, weather, production, growth, world map, and other maps continue their regular processing |

**Scope:** In v1, only *combat actors and flying projectiles on the activating map* are suspended. No `TickManager` global pause. This is deliberately an entity-level time stop, not a universal freeze of every RimWorld timer.

**Not decided:** cooldown, activation costs, unlock requirements, AI usage, and whether frozen enemies can physically obstruct movement. These are balance/design choices for a later phase; no values are invented here.

## 2. Gameplay behavior

1. Owner with awakened/manifsted Star Platinum requests Time Stop. Validate owner, Stand, active map, no other Time Stop, unlocked ability/cooldown when defined.
2. Ability becomes active at a controlled tick boundary. Store owner/map and `endTick = currentTick + 300`.
3. All other spawned pawns on that map are frozen in place: no movement, jobs, combat decisions, ability use, status progress owned by their pawn tick (subject to verified engine hooks). Projectiles already in flight suspend position and impacts.
4. Activator moves and attacks normally. Its **Stand-sourced** damage is queued as an ordered record. Target HP/hediffs are not changed during Time Stop; feedback previews pending hit count. Target validity is checked again at resolution.
5. On expiry, process the pending damage queue **once**, in deterministic sequence, then release the freeze and resume regular ticks. Combat logs should attribute Stand, owner, target and time-stop event.
6. Pause menu pauses the ability because game simulation ticks no longer advance. Save/load must preserve the active state, remaining ticks, identity and pending hits.

At x3 game speed the 300 simulation ticks pass more quickly in real-world time; this is expected.

### Cancellation semantics

- Owner becomes unavailable, map unloads or save fails to resolve owner: terminate the effect at the next safe boundary, resolve valid queued hits **once**, discard invalid targets with debug log, release all frozen actors/projectiles.
- If Time Stop is active when a save is made, never save a list of externally patched methods or C# runtime object instances. Store only game-save-compatible refs + primitives, rebuild indices after load.
- Removing the mod mid-save is unsupported; install/update migration must avoid permanent frozen Hediffs, jobs or state mutations.
- Multiple simultaneous Time Stops: v1 rejects additional activation requests. Do not queue or nest freezes.

### Deferred-hit accounting

A pending entry records `sequenceNumber`, `owner`, `target`, `sourceStandDef`, `damageDef`, `baseDamage`, `armorPenetration`, `hitPart` (if needed), `queuedAtTick`, and one-time `resolved` state. Entries are bounded per activation to prevent pathological spam (exact cap TBD).

At resolution, apply damage using RimWorld's normal damage pipeline rather than changing HP directly. Armor, body-part selection, death, stagger, damage reactions and kill credit are calculated on resolution. Avoid duplicating an ordinary melee attack through interception: **only explicitly identified Stand attack events participate in deferred damage** for the first release.

Because damage is deferred, the attacker can queue more hits than a target would survive, and the target will not fall until release. Entries after death are discarded with a clear reason. Never silently refund consumed attacks unless specified by gameplay balance.

## 3. Architecture: isolated temporal service

```text
StandSystem / ability gizmo
       |
       v
TimeAbilityService.RequestActivate()
       |
       v
TemporalWorldController (GameComponent: global exclusion, state)
       |
       +---- TimeStopMapState (MapComponent: owner, map, endTick, actor/projectile scope)
       |
       +---- TemporalFreezePolicy.CanAdvance(Thing, tick)
       |
       +---- DeferredStandDamageQueue (ordered, save-safe events)
       |
       +---- VisualTimeEffect (UI-only overlay; no simulation authority)
```

**Do not confuse `StandKind` (manifestation type) with temporal abilities.** Time Stop should be an ability definition attached to a Stand. The World should use the same controller with distinct duration/cooldown once available.

### Major RimWorld 1.7 engineering risk

There is no assumed single safe universal freeze hook. Freezing `Pawn.Tick()` alone can leave jobs, health, pathfinding, needs, status and projectiles inconsistent; not all world behavior runs exclusively through individual Thing ticks.

**Required discovery before coding:** inspect the user's actual 1.7 assemblies and decompile/inspect method signatures and call paths for `TickManager`, `TickList`, `Thing.Tick`, `Pawn.Tick`, `Projectile.Tick`, `Pawn_JobTracker`, health/hediff ticks and mod-defined tickables. Check Harmony version and patch compatibility. Do not claim method names/patch coverage are confirmed from design notes alone.

Prefer the narrowest reliable per-entity tick suppression hook with an explicit allowlist for the owner. Do not patch the global tick to halt all processing. If patching vanilla tick lists would break invariants, use a separate simulated action freeze policy and explicitly document which secondary systems continue.

Projectiles require a verified hook suppressing motion/impact, not just a visual freeze.

## 4. Implementation phases

### Phase A — inspect and characterize engine
- Build a call graph of Pawn movement, jobs, damage, projectiles and tick sources using RimWorld 1.7 assemblies.
- Probe base game and Harmony mod environment; record exact patch targets and signatures.
- Prototype diagnostic-only freeze decision logs with no behavioral modifications.
- Define feature flag and safe-disable recovery path.

### Phase B — single-map actor freeze
- Implement controller, 300-tick expiry, global exclusion and debug UI activation.
- Freeze two hostile pawns while owner can move.
- Verify stationary pawn does not shoot, melee or resume a stale animation.
- Explicitly skip the v0.1.2 `StandGameComponent.TickNpcCombat` autonomous Stand scan for frozen NPCs.

### Phase C — projectiles and edge cases
- Suspend existing bullets, rockets and other launched projectiles on target map.
- Verify bullets resume trajectory and damage after unfreeze.
- Check explosions, melee verbs, doors and traps; if inconsistent, narrow scope rather than claiming full time control.

### Phase D — deferred Stand damage
- Add `StandSystem` attack pipeline interception **only** when Time Stop active and Stand-source verified.
- Queue typed, serializable event records; display pending strike markers.
- Resolve in order, exactly once, applying armor and normal damage effects.
- Include hit/miss/invalid target/death/no-double-application tests.

### Phase E — persistence, UI, regression
- Scribe active state, refs and pending queue in a `GameComponent` or `MapComponent`, compatible with pawn/map refs.
- Test save at ticks 0, 150 and 299; load, finish, no duplicate damage or permanent freeze.
- Add controls, translations, distinct Time Stop VFX and accessibility option to reduce screen effects.
- Verify mod compatibility and performance with multiple maps, raids, caravans and combat mods.

## 5. Acceptance scenarios

| Scenario | Expected |
| --- | --- |
| Activator and two enemies, melee encounter | Activator moves; enemies neither move nor attack for 300 ticks |
| Enemy projectile already airborne | Same position throughout stop, continues afterwards |
| Other colony map + growing crops / world time | Advance normally |
| 3 accepted Star Platinum hits during stop | Target HP unchanged until expiry; then damage events applied sequentially **once** |
| Save at middle of stop and reload | Remaining ticks and queue preserved, no duplicated hits |
| Target despawned or dead on release | Skip safely, log reason, no exception |
| Owner lost during stop | Safe cancellation, queue resolution, freeze removed |
| Press Time Stop twice | Second request rejected; first not extended |
| NPC with a Stand also on affected map | NPC's autonomous Stand attack does not bypass freeze |
| Pause during Time Stop | Ability remaining game ticks unchanged |
| 20 frozen pawns plus active projectiles | No runaway logs, allocations, or save corruption |
| Multiple mods affecting ticks | Diagnose incompatibility explicitly; no silent world freeze |

## 6. Interaction with other temporal Stands

Temporal abilities should expose distinct **policies**, not manipulate one shared game-speed multiplier:

- `Stop` — allow only configured participants; used by Star Platinum and The World.
- `Accelerate` — hypothetical future Made in Heaven: accelerates selected actor/world-facing systems under controlled policies; **must not** simulate thousands of real RimWorld global ticks per frame.
- `Rewind` — if ever supported, requires explicit reversible state snapshots or scripted ability effects; not equivalent to moving `TicksGame` backward.
- `UniverseReset` — **Made in Heaven endgame narrative operation**, not a tick-freeze feature. Implement as an explicit, opt-in new-scenario or world-transition operation with a backup of the existing save; never rewrite the original save without an irreversible-action warning and approval.

In particular, Made in Heaven is a reason to separate **time policy**, **simulation authority** and **narrative world transitions**. Universe reset cannot be accomplished safely by just increasing RimWorld's speed setting.

## 7. Out of scope for first build

- NPC autonomously deciding when to stop time.
- The World concurrently overriding or counteracting Time Stop.
- Time Stop affecting all maps and global calendar.
- Full preservation of every modded tick component, unless verified.
- World reset, acceleration or timeline rewinds.
- Unlimited queued hits or full cinematic multi-frame animations.

**Milestone:** deliver a minimal, testable 300-tick actor/projectile freeze with serializable deferred Stand damage, without disturbing normal colonies outside the affected map. Do not merge into main until real RimWorld 1.7 build and in-game regression pass.
