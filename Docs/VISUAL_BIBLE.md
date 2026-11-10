# VISUAL_BIBLE — JoJo Stand System

> Status: **Visual direction v1.0 / approved design target, not implemented art.**
> Game: RimWorld 1.7. Baseline: vanilla RimWorld sprites, camera and world rendering. This specification covers *style*; canonical lore and mechanical effects are documented separately.
> This document governs all future Stand visuals. Exceptions require a recorded entry in the catalog, not a silent change in renderer code.

## 1. Art direction

**RimWorld first, JoJo recognizable.** Stands should feel authored for the game's oblique top-down camera, simplified silhouettes and subdued materials, rather than like anime screenshots pasted on the map. Preserve each Stand's recognizable proportions or signature attribute; do not require one common anatomy for unrelated Stands.

**Visual priorities, in order:**
1. Readable silhouette at normal play zoom and 1080p, without selecting the pawn.
2. Legible relationship to owner / target / bound object.
3. A single recognizable characteristic of the canonical Stand.
4. Clear manifestation, activity, and end-state communication.
5. Polish: subtle motion, VFX and transitions.

### Allowed / disallowed

| Do | Don't |
| --- | --- |
| Top-down, RimWorld-like simplified painterly forms | Full-height anime portrait as a billboard |
| 2–4 large value groups, controlled highlights | Small high-frequency muscles, armor line clutter |
| Distinct head, chest, shoulder and arm masses (for humanoids) | Identical generic shape recolored for every Stand |
| Soft dark contour and readable silhouette | Thick comic outlines and exaggerated external glow |
| VFX anchored to action and world cells | Constant screen-wide emissive or particle spam |
| Per-Stand exceptions when the fiction requires them | Force every Stand into a floating humanoid sprite |
| Asset drawn / licensed for this mod | Scraped anime frames, fan art or unlicensed mod assets |

Colors are **reference directions**, not globally enforced RGB values. Use desaturated midtones, a dark structural shade and one restrained accent; accent can briefly saturate during action.

## 2. Size, camera, composition

RimWorld measures world placement in cells. All following scale values are **initial art targets** to be tuned in the actual 1.7 renderer, not assertions about Unity pixels-to-world conversion.

| Asset type | Suggested authored canvas | Initial draw footprint |
| --- | --- | --- |
| Humanoid 4-way base | 128 × 128 px per direction | 0.75–1.10 cell width; 0.9–1.35 cell depth |
| Humanoid hand / attack overlays | 128 × 128 px | Match base local anchor |
| Small colony member | 48 × 48 or 64 × 64 px | 0.18–0.36 cell |
| Body overlay / suit | 128 × 128 px | Follows pawn drawing; scale per body size |
| Icon / inspect portrait | 64 × 64 px | UI-only; no world scale implied |
| Zone / effect textures | power-of-two textures | Defined by gameplay area, not Stand sprite scale |

- Transparent PNG, un-premultiplied alpha unless pipeline explicitly states otherwise; avoid colored pixels outside alpha edges.
- Use **four facings** (North/East/South/West) for humanoids once production assets begin. Only use one view for intentionally radial/abstract forms.
- Character sprite must read at standard zoom; verify with terrain, snow, nighttime, fire and clutter backgrounds.
- Humanoid face and silhouette should survive downscaling; never rely on tiny eyes or patterns as the *only* identifier.
- Owner usually remains readable in front. Stand anchors are **per-direction**, not a single hardcoded `x += 0.5`.
- Draw depth / occlusion / elevation must be checked at all four rotations. Never hide owner hands/weapon unnecessarily.

## 3. Art archetypes

Archetypes are **visual rendering groups**, not a Stand's combat class. One Stand may use several primitives.

| Render archetype | Presentation rule | Example |
| --- | --- | --- |
| `Humanoid` | Compact figure with head/torso/arms, normally behind owner | Star Platinum |
| `Swarm` | Pooled small visuals, sampled count rather than full entity count | Harvest |
| `Object` | World object / overlay, obeying ordinary visibility where canonical | Anubis |
| `Field` | Area VFX bound to gameplay region, not permanent portrait | Green Day |
| `Overlay` | Applied to pawn appearance; merges with owner's pose | White Album |
| `Symbolic` | Brief UI/world sign or action effect | Pearl Jam |

**Behavioral exception:** a Stand may be humanoid in appearance yet use a phenomenon-style *ability*. Example: Killer Queen = Humanoid + bomb mark; Wonder of U = humanoid representation + calamity effects.

## 4. Presence, anchoring and layer policy

- `Persistent`: world visualization when summoned, respecting owner visibility settings.
- `OnAbility`: short effect only while an ability is executing.
- `OnCondition`: appear while a canonical trigger is active.
- `WorldBound`: anchor to spawned Thing or cell; handle despawn.
- `OwnerOverlay`: overlay after owner renderer stage without affecting hitboxes.
- `TargetOverlay`: effect on affected pawn, Thing or area.

Anchors: `OwnerRear`, `OwnerSide`, `OwnerBody`, `Target`, `Thing`, `Cell`, `Area`. Per-direction offsets and per-body-size adjustments are allowed; **never assume all pawns are human-sized**.

Opacity/motion are **render-only** and must never change combat damage or game physics. Default starting opacity: idle ~0.60–0.75, active strike ~0.80–0.95, fading 0→idle and idle→0 over short transitions. Exceptions: real bound objects or canonically visible objects can remain opaque.

## 5. Motion vocabulary

| Motion | Effect | Constraints |
| --- | --- | --- |
| `Hover` | Low-amplitude idle bob, no tile movement | Deterministic by ticks/owner ID |
| `Lunge` | Short forward motion on confirmed attack | Follows attack target/facing; no teleport as world logic |
| `Burst` | Single impact smear and return | Not continuous particles |
| `Orbit` | Small units orbit / scatter | Cap count, distance and update cost |
| `Pulse` | Alpha/scale pulse on trigger | No photosensitive flash/flicker |
| `Trail` | Brief fading afterimage | Limit draw calls |
| `None` | Object or passive effect stays fixed | Valid design, not unfinished work |

Every persistent Stand must define `Idle`, `Manifest`, `Recall`, and its relevant action state(s). A static `Idle` is allowed; no mandatory keyframe animation.

### Readability over spectacle
- Never animate position if it makes the source of damage ambiguous.
- The attack effect should occur when `StandSystem.Punch` actually succeeds; no fake hit before damage confirmation.
- Avoid very rapid alpha toggling. Reduced VFX setting should be supported by renderer later.
- For a swarm, render a representative number of units at a distance; show more only when zoomed in or active.

## 6. Perception and world truth

**Current v0.2.0 is omniscient rendering only.** It is a placeholder policy, not canonical invisibility.

Future systems must separate:
1. `CanPerceiveStand(observer, stand)`: gameplay knowledge and targetability.
2. `ShouldRenderStand(viewerContext, stand)`: what the player sees.
3. `HasPhysicalVisibility(stand)`: exceptions for bound/material Stands.
4. `RenderEffectVisibleToAll(effect)`: visible physical consequences even when source is hidden.

A non-user seeing a physical effect does *not* automatically perceive its Stand source. In player omniscient mode, rendering a hidden Stand does *not* grant AI targetability. This contract is nonnegotiable.

## 7. Stand-specific signature rules

- **Star Platinum:** broad shoulders, dark violet / cool blue blocks, compact recognizable hair/head silhouette; a controlled gold head accent. Heavy fists, short forward strike.
- **The World:** armored angular silhouette, muted ochre + dark olive; strong forearm guard shape, no constant gold bloom.
- **Crazy Diamond:** softened armor plates, purple-pink and mint accent; repair ability uses restrained reconstruction fragments.
- **Killer Queen:** slim feline-eared silhouette, dusky pink, restrained skull insignia; bombs show a target mark, not an ambient explosion aura.
- **Harvest:** small, legible insectoid units in clusters; never draw hundreds of detailed creatures simultaneously.
- **Cheap Trick:** small figure attached to owner's upper back; always framed as a burden and condition, not a free-floating companion.
- **Pearl Jam:** appears in preparation/food/healing events, not as a permanent escort.
- **Hermit Purple:** vine overlays extend from body or ability target; no persistent humanoid avatar.

These signatures are constraints and examples, not claims that assets are already produced.

## 8. Naming / asset delivery

Suggested directory convention (future pipeline; does not reflect files currently in repository):

```text
Textures/
  Stands/
    JJ_StarPlatinum/
      N.png
      E.png
      S.png
      W.png
      Hit.png
      Portrait.png
    JJ_Harvest/
      Unit.png
      Cluster.png
      Portrait.png
```

The production loader will resolve paths from a visual profile, **not** hardcode each Stand name. Current `Textures/Things/StandManifestation/StarPlatinum.png` is a temporary placeholder scheduled for replacement.

## 9. Acceptance criteria for EVERY new Stand asset

- [ ] Consistent with RimWorld-like visual mass, shading and outline.
- [ ] Character recognizable at unselected game zoom.
- [ ] Four-way directional and owner-body-size placement tested where applicable.
- [ ] Spawn / summon, action, end-state / recall tested.
- [ ] Does not obstruct owner or weapon readability.
- [ ] Respects player presentation mode; no AI visibility leakage.
- [ ] Works after save/load, map transitions, death/despawn and on large maps.
- [ ] Draw-cost budget measured with large groups / raiders.
- [ ] Screenshot and author/source license recorded.
- [ ] Approved as `ArtReady` in catalog; until then remains `Planned` or `Prototype`.

## 10. Explicit boundaries

This document freezes shared **style principles**, not final art forever. Later Stand-specific exceptions and asset quality upgrades are expected; avoid reworking the shared renderer for each one. All new profile types should be reviewed against `STAND_VISUAL_SCHEMA.md` and entered in `STAND_VISUAL_CATALOG.md`.
