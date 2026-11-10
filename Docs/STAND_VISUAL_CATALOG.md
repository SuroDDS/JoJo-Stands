# STAND_VISUAL_CATALOG — planning registry

> **Catalog v1.0 — design inventory, not implemented assets.** JoJo contains far more named Stands than can responsibly be drawn at once. This document covers the **first 85 prioritized canonical Stand identities** spanning Parts 3–8, plus named Act/Requiem forms as profile variants. It is deliberately a staged catalog, **not a claim that every canonical Stand is listed**. New Stand entries MUST follow the same schema and art rules.
>
> Gameplay status: currently only `JJ_StarPlatinum` exists as a playable StandDef; its v0.2.0 sprite is a temporary placeholder. **All other rows are planned visual designs, not working mechanics or available sprites.**

See [VISUAL_BIBLE.md](VISUAL_BIBLE.md) for shared style and [STAND_VISUAL_SCHEMA.md](STAND_VISUAL_SCHEMA.md) for proposed runtime data.

## 1. Catalog vocabulary

| Short label | Maps to schema |
| --- | --- |
| `H` | Humanoid |
| `S` | Swarm |
| `O` | Object |
| `F` | Field |
| `V` | Overlay |
| `Y` | Symbolic |
| `P` | Persistent |
| `A` | OnAbility |
| `C` | OnCondition |
| `W` | WorldBound |
| `OV` | OwnerOverlay |
| `TV` | TargetOverlay |
| `rear` / `side` / `body` / `target` / `thing` / `area` | Anchors |
| `SU` | StandUsersOnly |
| `ALL` | VisibleToAll physical manifestation |
| `COND` | Conditional visibility; needs rule |
| `Planned` | Art direction only |
| `Prototype` | Temporary sprite exists, not approved |
| `ArtReady` | Approved production asset (none yet) |

Palette notation `base / shade / accent` is an art direction in natural color names; final RGB is set by the art pipeline. *Canon variations in colors exist between media; final choices prioritize recognizability and consistent RimWorld presentation, not an assertion of one canonical color.*

All rows require 4-way artwork **when archetype = H and appearance is persistent** unless a documented exception is approved.

## 2. Priority A — production candidates

These define the reusable rendering archetypes before broad expansion.

| Stand | Archetype; presence; anchor | Palette (base / shade / accent) | Motion and signature FX | Visibility | Art status |
| --- | --- | --- | --- | --- | --- |
| **Star Platinum** | H; P; rear | muted violet / indigo / gold | hover, fist lunge, brief afterimage | SU | **Prototype** |
| The World | H; P; rear | ochre / olive-black / pale gold | rigid hover, heavy punch smear | SU | Planned |
| Crazy Diamond | H; P; rear | dusty lilac / deep violet / pale mint | short lunge, assembly fragments on repair | SU | Planned |
| Killer Queen | H; P; rear + TV | rose / burgundy / pale grey | quiet idle, hand touch, restrained bomb mark | SU (bomb consequence visible) | Planned |
| Harvest | S; P; side + area | mustard / dark ochre / aqua | bounded clustering, pickup trails | SU | Planned |
| Hermit Purple | Y; A; body + target | dark purple / plum / violet | extending vine stroke, brief pull | SU | Planned |
| Magician's Red | H; P; rear | vermilion / maroon / amber | bird-headed idle, fire arc | SU (fire visible) | Planned |
| Hierophant Green | H; P; rear + area | jade / deep green / pale aqua | elongated curl, emerald projectile | SU | Planned |
| Silver Chariot | H; P; rear | silver-grey / blue-grey / pale cyan | rapid point-thrust, foil streak | SU | Planned |
| Cheap Trick | V; C; body | grey-violet / dark plum / pale eyes | fixed at back, lean on trigger | SU / COND transfer | Planned |
| Pearl Jam | Y; A; thing + target | muted orange / burgundy / pearl | tiny cooking spark, healing motes | SU | Planned |
| Sticky Fingers | H; P; rear + TV | slate blue / navy / silver | zip line, seam opening | SU (physical seam visible) | Planned |
| The Hand | H; P; rear | dusty blue / graphite / terracotta | swipe, short space-tear | SU | Planned |
| King Crimson | H; P; rear | faded crimson / wine / white | head/arm silhouette, restrained time smear | SU | Planned |
| Gold Experience | H; P; rear + TV | muted gold / ochre / teal | soft life-creation flash | SU (creation visible) | Planned |
| Stone Free | H; P; rear + area | blue-cyan / slate / pale silver | thread unwind/recoil | SU (physical results visible) | Planned |

## 3. Part 3 — additional Stands

| Stand | Archetype; presence; anchor | Palette | Signature motion / FX | Visibility |
| --- | --- | --- | --- | --- |
| Tower of Gray | S; P; target | dark grey / red / pale eyes | darting insect streak | SU |
| Dark Blue Moon | H; C; area | teal / navy / pale foam | water ripple, claw slash | SU |
| Strength | O; W; thing | rusty hull / iron / pale cyan | ship/object transformation | ALL (bound object) |
| Ebony Devil | O; W; thing | dark burgundy / charcoal / cream | doll/weapon motion | COND |
| Yellow Temperance | V; OV; body | muddy yellow / umber / green | gelatin growth and drips | COND |
| Hanged Man | Y; A; target | silver / black / light blue | reflection-line jump | SU |
| Emperor | O; C; body | black steel / bronze / gold | manifested handgun and bullet line | COND |
| Empress | V; C; body | flesh red / maroon / pale pink | parasitic growing overlay | COND |
| Wheel of Fortune | O; W; thing | dark red / black / chrome | car transformation, tire skid | ALL |
| Justice | F; C; area | smoke grey / blue-black / pale white | fog curl / spectral threads | COND |
| Lovers | S; C; target | pink-grey / purple / pale cyan | tiny invasive sprite | SU |
| The Sun | F; W; area | yellow-white / ochre / orange | heat shimmer, bounded light pulse | ALL (effect) |
| Death Thirteen | H; C; area | blue / purple / pale yellow | dream-only scythe stroke | COND |
| Judgement | H; C; target | clay grey / brown / pale red | earth emergence | SU |
| High Priestess | O; C; thing | turquoise / dark teal / pale green | shifting material geometry | COND |
| Geb | F; C; area | pale blue / navy / white | water-tendril strike | ALL (water effect) |
| Horus | F; A; area | ice blue / slate / white | frost burst, ice shape | ALL (ice effect) |
| The Fool | F; P; side + area | sand beige / ochre / dark brown | sand cloud / paw motif | SU (sand consequence visible) |
| Cream | H; C; rear + area | dark purple / black / pale gold | contracting void sphere | SU / COND |
| Atum | Y; A; target | terracotta / dark gold / grey | wager/token cue | SU |
| Osiris | Y; A; target | brown / black / warm gold | chip/token capture | SU |

## 4. Part 4 — additional Stands

| Stand | Archetype; presence; anchor | Palette | Signature motion / FX | Visibility |
| --- | --- | --- | --- | --- |
| Echoes ACT1 | H; P; rear | pale mint / grey / pink | hovering glyph burst | SU |
| Echoes ACT2 | H; P; rear | yellow-green / olive / aqua | sound-effect tile mark | SU |
| Echoes ACT3 | H; P; rear | emerald / bronze / navy | compact hover, weight impact | SU |
| Heaven's Door | H; A; target | cream / slate / gold | paper / book opening | SU (affected pawn visible) |
| Bad Company | S; P; side + area | army green / olive / brown | tiny soldiers, tracer lines | SU |
| Red Hot Chili Pepper | H; C; area | pale yellow / dark gold / cyan | electricity travel | SU (electric effects visible) |
| Love Deluxe | V; OV; body | dark brown / chestnut / plum | segmented hair strands | ALL (hair) |
| Ratt | O; A; target | grey / gunmetal / pale lime | small turret dart | SU |
| Boy II Man | Y; A; target | muted red / black / gold | hand-sign icon | SU |
| Highway Star | F; C; target + area | violet / navy / white | footprint trail, reach | SU |
| Stray Cat | O; W; thing | pale cream / leaf green / violet | plant bud, air bullet | ALL (plant) |
| Super Fly | O; W; thing | steel grey / graphite / ochre | tower reaction sparks | ALL (pylon) |
| Surface | O; W; thing | timber brown / dark grey / skin | puppet pose mimic | ALL |
| Enigma | Y; C; target | aged ivory / brown / dark blue | fold-to-paper effect | SU / COND |
| Earth Wind and Fire | V; OV; body | beige / olive / soft turquoise | silhouette morph | ALL (resulting shape) |

## 5. Part 5 — additional Stands

| Stand | Archetype; presence; anchor | Palette | Signature motion / FX | Visibility |
| --- | --- | --- | --- | --- |
| Gold Experience Requiem | H; C; rear | ivory gold / deep gold / magenta | quiet aureole, causality return cue | SU |
| Black Sabbath | H; C; area | charcoal / violet-black / red | shadow emerge / retreat | SU |
| Moody Blues | H; P; rear | muted blue / navy / pale silver | replay rewind trails | SU |
| Sex Pistols | S; A; thing + target | orange / navy / cream | six small bullet escorts | SU (bullet visible) |
| Aerosmith | O; P; side + area | dark grey / olive / red | tiny aircraft, contrail | SU (physical damage visible) |
| Purple Haze | H; P; rear + area | pale purple / charcoal / dark magenta | spore capsule / cloud | SU (gas consequences visible) |
| Spice Girl | H; P; rear + target | rose / deep pink / gold | elastic impact rings | SU |
| Metallica | S; C; body + target | iron grey / maroon / rust | metallic flecks / blades | SU / COND |
| White Album | V; OV; body | ice blue / grey-blue / white | frost suit silhouette | COND |
| Oasis | V; OV; body + area | beige / grey / brown | ground softening ripple | COND |
| Notorious B.I.G | F; C; area | dark crimson / black / pale red | blob swelling, chase trail | COND |
| Green Day | F; C; area | mold green / dark olive / yellow | fungal bloom on surfaces | ALL (mold) |
| Beach Boy | O; P; thing + target | ocean green / charcoal / silver | line/hook lash | COND |
| Rolling Stones | O; W; thing | sandstone / ash grey / dark green | stone rolling / omen | ALL (stone) |

## 6. Part 6 — additional Stands

| Stand | Archetype; presence; anchor | Palette | Signature motion / FX | Visibility |
| --- | --- | --- | --- | --- |
| Foo Fighters | S; C; body | blue-green / navy / off-white | micro-organism scatter / body transition | COND |
| Burning Down the House | Y; C; area | pale cyan / slate / warm grey | ghost-object reveal | COND |
| Kiss | V; A; target | dusky pink / burgundy / white | split sticker, reunite snap | ALL (duplicated result) |
| Weather Report | H; P; rear + area | pale lilac / blue-grey / ivory | small cloud orbit, localized weather | SU (weather visible) |
| Whitesnake | H; P; rear + target | chalk white / muted grey / purple | disc extraction | SU |
| C-Moon | H; P; rear + area | olive / dark mint / cream | inverted gravity cues | SU (physical effect visible) |
| Made in Heaven | H; C; rear + area | pearl white / blue-grey / gold | speed streaks, controlled sky cadence | SU |
| Diver Down | H; P; rear + target | cobalt / deep blue / beige | body-enter ripple | SU |
| Green, Green Grass of Home | F; C; area | moss green / olive / dark umber | scale perspective cue | SU |
| Jail House Lock | Y; C; target | slate blue / grey / white | symbol over memory events | SU |
| Bohemian Rhapsody | F; C; area | dusty purple / grey / ink-black | bounded illustration-reveal | COND |

## 7. Part 7 — additional Stands

| Stand | Archetype; presence; anchor | Palette | Signature motion / FX | Visibility |
| --- | --- | --- | --- | --- |
| Tusk ACT1 | H; C; rear | dusty pink / cream / blue | small head and nail-shot cue | SU |
| Tusk ACT2 | H; C; rear | pink / lilac / teal | spiraling nail trail | SU |
| Tusk ACT3 | H; C; rear | dark pink / plum / cream | spatial hole mark | SU |
| Tusk ACT4 | H; P; rear | violet-pink / black / white | heavy mass / rotational ring | SU |
| D4C | H; P; rear | faded pink / deep rose / blue | planar seam / overlap silhouette | SU |
| Scary Monsters | V; OV; body | moss green / deep olive / beige | dinosaur morph overlay | ALL (body visible) |
| Mandom | Y; A; body | bronze / leather brown / gold | watch dial / rewind | SU |
| Catch the Rainbow | V; C; area | grey-blue / ink / pale cyan | raindrop steps / suspension | COND |
| Ball Breaker | H; A; target | bronze / dark grey / pale yellow | spinning sphere, trail | SU |
| Civil War | F; C; area | grey / ochre / violet | spectral object echoes | COND |
| Chocolate Disco | F; C; area | dark blue / silver / red | bounded grid-cell markers | SU / COND |

## 8. Part 8 — additional Stands

| Stand | Archetype; presence; anchor | Palette | Signature motion / FX | Visibility |
| --- | --- | --- | --- | --- |
| Soft & Wet | H; P; rear + target | pale blue / slate / soft ivory | soap bubble ring | SU (bubble consequence conditional) |
| Paisley Park | H; C; target | lilac / navy / pink | screen / route motif | SU / COND |
| Wonder of U | H; C; target + area | grey / indigo / dark red | silhouette in distance, calamity omen | COND |
| Born This Way | H; C; area | cool grey / purple / teal | vehicle silhouette and cold trail | SU |
| Vitamin C | F; C; area | yellow-green / olive / brown | liquefaction ripple | SU (effects visible) |
| Speed King | H; P; rear + target | red-orange / brown / muted yellow | precise heat mark | SU |
| Walking Heart | V; OV; body | magenta / deep violet / pale pink | heel-spike cue | COND |
| California King Bed | Y; C; target | yellow / cream / cobalt | chess-like / memory tile cue | SU |

**Inventory count:** 16 priority A + 18 Part 3 + 14 Part 4 + 14 Part 5 + 11 Part 6 + 11 Part 7 + 8 Part 8 = **92 entries** (individual Act/Requiem visual forms count as entries). This is an intended coverage set, not an authoritative canon index.

## 9. Cross-cutting per-Stand exceptions and approvals

- **Coco Jumbo / Mr. President:** the turtle is a physical animal, the Stand's room is an interior phenomenon. Do not draw the turtle itself as an intangible companion; when added, model the room/entry visual separately.
- **Anubis:** a physically present sword; artifact silhouette is visible to everyone, supernatural awareness follows separate rules. To be added as an object profile.
- **Cheap Trick:** attached to a victim's back; its ownership and transfer rules must be implemented with care. A rear overlay fits better than a freely mobile entity.
- **Wonder of U:** visually humanoid but its calamity mechanic is a phenomenon. Separate the two profiles.
- **The Sun / Strength / Wheel of Fortune:** their physical manifestations should be visible under suitable conditions even to normal pawns.
- **Requiem and Act forms:** do not treat as automatic palette swaps: body silhouette, event presentation and effects may change. Persistent gameplay `StandDef` mapping for evolutions is future work.
- **Names and canon:** visual identity names are references for design; obtain original art or appropriately licensed assets and document credits before release.
- **Part 9 and remaining Stands:** maintain an expansion backlog; research exact depiction and mechanics before marking any new entry ArtReady.

## 10. Implementation priorities

### Gate 1 — renderer architecture
Implement `StandVisualDef` registration, visual provider dispatch, material caching and directional offsets. Do not add dozens of hardcoded classes.

### Gate 2 — humanoid proof
Replace Star Platinum placeholder with an approved **four-direction RimWorld sprite set**. Check walking, melee, recall, save/load, downed, dead, four facings and mixed pawn sizes.

### Gate 3 — nonhumanoid proof
Implement Harvest with representative swarm units and Pearl Jam with event-only VFX. If either requires rewriting the renderer, schema is not yet flexible enough.

### Gate 4 — perception
Split viewer rendering from AI perception and targetability. Verify conditional exceptions and physical consequence rendering.

### Gate 5 — visual production pipeline
For each catalog entry: `Planned -> Prototype -> ArtReady`. Keep assets separately credited/licensed; never upgrade status merely because a texture exists.

## 11. Art direction decision log

| Decision | Status |
| --- | --- |
| RimWorld-first original art, not anime image pastes | Fixed |
| Silhouette before detail | Fixed |
| Separate gameplay class from visual archetype | Fixed |
| Visual profiles data-driven, not per-Stand renderer branches | Fixed |
| Low-cost VFX with caps for swarm and area effects | Fixed |
| Opaque bound objects vs intangible Stand visibility | Fixed |
| All Stand assets complete already | **No** |
| v0.2.0 Star Platinum placeholder is production-ready | **No** |
