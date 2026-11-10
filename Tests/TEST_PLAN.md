# v0.1 test plan — RimWorld 1.7 + Biotech

## Before launch
- [ ] Compile with .NET SDK 10 against *real* RimWorld 1.7 game DLLs; inspect first errors.
- [ ] Enable Harmony 1.7 + Biotech + this mod in that order.
- [ ] Confirm `[JoJoStands] Prototype 0.1 initialized.` in the RimWorld log.
- [ ] Confirm no failed `Def`/`XML` parse messages.

## Arrow
- [ ] Dev-mode spawn `JJ_StandArrow` on a tile adjacent to a colonist.
- [ ] Command `Use Stand Arrow` becomes enabled.
- [ ] Clicking consumes exactly one Arrow.
- [ ] A successful attempt grants *one* bond and Star Platinum.
- [ ] Reattempt is impossible for a pawn with an awakened Stand.
- [ ] Rejected attempt causes stab damage or death (according to settings).
- [ ] Settings change takes effect; chance=95% is not guaranteed success.

## Manifested Stand
- [ ] `Manifest Stand` toggles to `Recall Stand`.
- [ ] `Stand Punch` selects a reachable pawn and deals blunt damage once.
- [ ] Out-of-range/blocked targets do not deal damage.
- [ ] Cooldown blocks repeated attacks.
- [ ] While recalled, Punch is unavailable.
- [ ] Saving while manifested preserves Stand choice, cooldown and manifestation on reload.

## Biotech + NPC
- [ ] Spawn a Biotech infant with a potential-carrying parent; observe silent latent assignment after up to 2500 ticks.
- [ ] Latent bond is not displayed in Health tab, but is present in save XML.
- [ ] Child ages to >=3 and can naturally awaken during later daily checks.
- [ ] Changing NPC probability changes future encounter population, not existing Stands.
- [ ] Non-player hostile Stand user within 2.9 cells auto-attacks, then respects cooldown.
- [ ] Turn animal Stand distribution on and verify eligible animals may receive a bond.

## Safety / compatibility
- [ ] Pawn downed during a punch, pawn dies during Arrow procedure.
- [ ] Arrow in carried inventory and adjacent map tile.
- [ ] Save, quit, reload, re-check status of captured and caravan pawns.
- [ ] Patch stack with popular mods that add Pawn gizmos.
- [ ] Test disabling each option mid-save; confirm no retroactive data deletion.
- [ ] Check performance with 100+ animals and large raids.

**Known unverified:** This code has not been compiled or run in RimWorld at generation time. In particular, verify 1.7 Harmony callback signatures, XML def loading, game-owned APIs and nullable game state against the exact user's build before calling it release-ready.
