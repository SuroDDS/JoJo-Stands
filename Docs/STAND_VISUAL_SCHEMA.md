# STAND_VISUAL_SCHEMA — data contract

> **Design specification v1.0, not an implemented XML parser.** Current v0.2.0 uses a single hardcoded Star Platinum texture and renderer. This document describes the target renderer configuration for v0.2.1+ and must not be confused with existing `StandDef` fields.

## 1. Objectives

1. Existing `StandDef` remains the gameplay definition (`kind`, damage, range, cooldown, weighting).
2. A new data-only `StandVisualDef` (proposed) references its gameplay `StandDef` by `standDefName`.
3. Renderer selects a provider by `archetype`, plus optional VFX layers. **No giant switch on `JJ_StarPlatinum`**.
4. Missing art / profile must fail gracefully: a visible debug marker in developer mode, otherwise no extra rendered object; logging is throttled.
5. Visual state is **derived** from gameplay state and transient events; visuals never determine hit chance, faction hostility, damage or ownership.
6. Older saves using `Hediff_StandBond` remain valid. No serialization of texture/material references.

## 2. Proposed profile fields

| Field | Type | Required | Semantics |
| --- | --- | --- | --- |
| `defName` | string | yes | Stable unique visual profile ID, e.g. `JJV_StarPlatinum` |
| `standDefName` | string | yes | Existing gameplay `StandDef.defName` |
| `archetype` | enum | yes | `Humanoid`, `Swarm`, `Object`, `Field`, `Overlay`, `Symbolic` |
| `presence` | enum | yes | `Persistent`, `OnAbility`, `OnCondition`, `WorldBound`, `OwnerOverlay`, `TargetOverlay` |
| `anchor` | enum | yes | `OwnerRear`, `OwnerSide`, `OwnerBody`, `Target`, `Thing`, `Cell`, `Area` |
| `renderMode` | enum | yes | `DirectionalSprite`, `SingleSprite`, `ParticleCluster`, `Decal`, `ObjectOverlay`, `Procedural` |
| `spriteBasePath` | string? | conditional | Path relative to `Textures`, no `.png` for directional sprites |
| `palette` | `PaletteDef` | yes | Main, shadow, accent as hex RGB/RGBA, art direction reference |
| `scale` | float | yes | World-space multiplier, default 1; checked against sprite dimensions |
| `offsetNorth/East/South/West` | Vector2 | for owner anchor | Offset in map X/Z cells, relative to owner |
| `altitudeLayer` | enum/string | yes | Renderer-resolved draw ordering, not arbitrary physics layer |
| `baseOpacity` | float | yes | Clamp 0..1 |
| `activeOpacity` | float | yes | Clamp 0..1 |
| `manifestTicks` | int | yes | Fade-in duration, >=0 |
| `recallTicks` | int | yes | Fade-out duration, >=0 |
| `motionProfile` | enum | yes | `Hover`, `Lunge`, `Burst`, `Orbit`, `Pulse`, `Trail`, `None` |
| `motionAmplitude` | float | optional | Default 0; render-only world cells |
| `motionPeriodTicks` | int | optional | Positive if motion is cyclic |
| `attackAnimTicks` | int | optional | >=0; depends on confirmed attack event |
| `swarmVisibleUnits` | int | conditional | Capped representative visuals for `Swarm` |
| `fx` | list of `StandFxDef` refs | optional | Ability-specific short VFX profiles |
| `physicalVisibility` | enum | yes | `StandUsersOnly`, `VisibleToAll`, `Conditional` |
| `visibilityCondition` | string? | conditional | Rule key for `Conditional`; no executable arbitrary XML code |
| `assetStatus` | enum | yes | `Planned`, `Prototype`, `ArtReady` |
| `notes` | string | optional | Nonruntime art review and canonical caveats |

### Independent dimensions

- `archetype`: how to draw it.
- `presence`: when it exists visually.
- `anchor`: relative to what.
- `physicalVisibility`: who can perceive the Stand itself.
- `fx`: what happens **during an action**.

These must not be conflated into one overly broad `StandKind`.

## 3. Proposed schema sketch (NOT currently loadable)

This is an **illustrative future XML**, not a current RimWorld Def. Do **not** paste it into `Defs/` until C# `StandVisualDef`, XML parsing, validation and renderer plumbing are implemented.

```xml
<JoJoStand.StandVisualDef>
  <defName>JJV_StarPlatinum</defName>
  <standDefName>JJ_StarPlatinum</standDefName>
  <archetype>Humanoid</archetype>
  <presence>Persistent</presence>
  <anchor>OwnerRear</anchor>
  <renderMode>DirectionalSprite</renderMode>
  <spriteBasePath>Stands/JJ_StarPlatinum</spriteBasePath>
  <scale>0.95</scale>
  <offsetNorth>(-0.40,0.20)</offsetNorth>
  <offsetEast>(-0.45,0.32)</offsetEast>
  <offsetSouth>(0.42,0.30)</offsetSouth>
  <offsetWest>(0.45,0.32)</offsetWest>
  <altitudeLayer>BehindPawn</altitudeLayer>
  <baseOpacity>0.70</baseOpacity>
  <activeOpacity>0.90</activeOpacity>
  <manifestTicks>12</manifestTicks>
  <recallTicks>12</recallTicks>
  <motionProfile>Hover</motionProfile>
  <motionAmplitude>0.035</motionAmplitude>
  <motionPeriodTicks>110</motionPeriodTicks>
  <attackAnimTicks>18</attackAnimTicks>
  <physicalVisibility>StandUsersOnly</physicalVisibility>
  <assetStatus>Planned</assetStatus>
</JoJoStand.StandVisualDef>
```

The exact XML shape and `Vector2` parser implementation may change during development. In particular, shader selection, sprite directional naming and draw-order API must be verified against the running RimWorld 1.7 client. `BehindPawn` is a semantic layer label in this specification, **not a known 1.7 AltitudeLayer enum value**.

## 4. Visual state machine

```text
Hidden -- summon --> Manifesting --> Idle
Idle -- successful attack --> Attacking --> Idle
Idle -- recall --> Recalling --> Hidden
Any -- owner despawn/death/map change --> Hidden
```

- Cosmetic transition state is runtime only.
- Gameplay stores `Awakened`, `Manifested`, and cooldown on `Hediff_StandBond`.
- When loading a save with `Manifested=true`, start in Idle or short appearance blend; **do not** grant an extra attack.
- `AttackEvent(owner, stand, target, tick, direction)` is emitted **only after** an attack is accepted by combat logic.
- No texture should be written to a save file.
- Repeated attack events replace a previous animation cleanly; prevent overdraw buildup.
- World-bound visuals disappear when their anchoring Thing is despawned, even if owner still exists.

## 5. Rendering interfaces (design only)

```csharp
// Pseudocode — not compiled in current project.
public interface IStandVisualProvider
{
    bool CanRender(StandVisualContext ctx);
    void Draw(StandVisualContext ctx, StandVisualDef profile);
}

public readonly struct StandVisualContext
{
    // Owner, StandDef, Map, facing, world anchor, viewer presentation mode,
    // game ticks and last confirmed combat/ability event.
}
```

Proposed providers: `HumanoidSpriteProvider`, `SwarmProvider`, `ObjectOverlayProvider`, `FieldProvider`, `OwnerOverlayProvider`, `EventVfxProvider`.

**Performance:** registry resolves `StandDef -> StandVisualDef` once and caches it. Materials loaded lazily/cached per texture, not once per pawn, not once per draw. No texture load in per-frame loops. List updates should avoid allocating every frame. Cap swarm render counts and effects. Profile validation logs once per invalid Def.

## 6. Perception API boundaries (future)

```text
CanPerceiveStand(observerPawn, standEntityOrOwner) -> bool
ShouldRenderStand(playerViewContext, standVisual) -> bool
CanTargetStand(attacker, standEntityOrOwner) -> bool
CanSeePhysicalConsequence(observer, effect) -> bool
```

Do not use `ShouldRenderStand` as combat AI targeting authorization. `VisibleToAll` for a bound sword or vehicle does not automatically mean its supernatural nature is understood by nonusers.

## 7. Validation

**At data load:**
- `defName` unique; `standDefName` points to a loaded gameplay Def.
- Presence/anchor combination legal (`WorldBound` requires Thing/Cell/Area, etc.).
- Required textures present for chosen render mode; verify four directional sprites when requested.
- Opacity range 0..1, positive cycle, nonnegative animation ticks, bounded swarm count.
- No cyclic FX inheritance or recursive prefab reference.
- Unknown enum values produce a clear diagnostic and fallback.
- Localization and credits tracked separately from raw art textures.

**In-game smoke tests:**
- Four pawn rotations, time pause, speed x1/x3, zoom min/max.
- Small/large humanlike owners and animal owner where enabled.
- Summon/recall, attack event, reload, caravan, capture, death and map removal.
- Normal view vs omniscient view after perception feature exists.
- 1 / 20 / 100 simultaneous manifested users, measure FPS and allocations.
- Missing visual profile must not break gameplay.

## 8. Migration plan

1. **v0.2.1 documentation**: approve this contract and catalog.
2. **v0.2.2 runtime**: add `StandVisualDef`, loader, validator and provider registry; retain `JJ_StarPlatinum` fallback renderer temporarily.
3. **v0.2.3 art**: production-quality four-facing Star Platinum; replace hardcoded material and tune positions in game.
4. **v0.2.4 perception**: separate renderer visibility from gameplay targeting.
5. **next**: implement second archetype (Harvest swarm), proving schema does not assume humanoids.

The art specification is stable; C# and XML APIs are explicitly **not committed** until compilation and game testing.
