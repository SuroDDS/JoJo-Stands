using System.Collections.Generic;
using RimWorld;
using Verse;

namespace JoJoStand
{
    /// <summary>
    /// Periodic registration avoids fragile patches to individual childbirth paths.
    /// Births created by Biotech are found after parent relations are established.
    /// Also gives hostile NPC Stand users a minimal close-range attack behavior.
    /// </summary>
    public sealed class StandGameComponent : GameComponent
    {
        public StandGameComponent(Game game) { }

        public override void GameComponentTick()
        {
            if (StandMod.Config == null || Find.TickManager == null) return;
            int now = Find.TickManager.TicksGame;
            if (now % 2500 == 0) RegisterPawns(now);
            if (now % 60 == 0) TickNpcCombat();
        }

        private static void RegisterPawns(int now)
        {
            foreach (Map map in Find.Maps)
            {
                foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
                {
                    if (!StandSystem.CanHaveStand(pawn)) continue;
                    if (pawn.Faction != Faction.OfPlayer && !StandMod.Config.npcUsers) continue;
                    if (pawn.RaceProps.Animal && !StandMod.Config.animalUsers) continue;
                    Hediff_StandBond bond = StandSystem.GetBond(pawn);
                    if (bond == null) bond = TryAssignNaturalPotential(pawn);
                    if (bond != null && !bond.Awakened && StandMod.Config.naturalAwakening)
                        TickAwakening(pawn, bond, now);
                }
            }
        }

        private static Hediff_StandBond TryAssignNaturalPotential(Pawn pawn)
        {
            float chance;
            bool newborn = pawn.RaceProps.Humanlike && pawn.ageTracker != null &&
                           pawn.ageTracker.AgeBiologicalYearsFloat < 3f;
            if (newborn && StandMod.Config.inheritPotential)
            {
                int parentUsers = CountParentsWithPotential(pawn);
                chance = parentUsers >= 2 ? 0.85f : parentUsers == 1 ? 0.50f : 0.02f;
            }
            else if (pawn.RaceProps.Animal)
            {
                chance = StandMod.Config.animalChance;
            }
            else
            {
                chance = pawn.Faction == Faction.OfPlayer ? 0.02f : StandMod.Config.npcChance;
            }

            // Seeded chance prevents repeated rolls each scan.
            if (Rand.ValueSeeded(unchecked(pawn.thingIDNumber * 397 ^ 0x6A7C43)) >= chance)
                return null;
            Hediff_StandBond bond = StandSystem.AddLatentBond(pawn);
            if (bond != null && !newborn && pawn.Faction != Faction.OfPlayer)
                StandSystem.Awaken(pawn, false);
            return bond;
        }

        private static int CountParentsWithPotential(Pawn pawn)
        {
            if (pawn.relations == null) return 0;
            int count = 0;
            foreach (DirectPawnRelation relation in pawn.relations.DirectRelations)
            {
                if (relation.def == PawnRelationDefOf.Parent &&
                    relation.otherPawn != null && StandSystem.GetBond(relation.otherPawn) != null)
                {
                    count++;
                }
            }
            return count;
        }

        private static void TickAwakening(Pawn pawn, Hediff_StandBond bond, int now)
        {
            if (pawn.ageTracker == null || pawn.ageTracker.AgeBiologicalYearsFloat < 3f) return;
            int day = now / 60000;
            if (bond.LastAwakeningDay >= day) return;
            bond.LastAwakeningDay = day;
            float chance = pawn.ageTracker.AgeBiologicalYearsFloat >= 13f ? 0.02f : 0.004f;
            if (Rand.Value < chance)
                StandSystem.Awaken(pawn, true);
        }

        private static void TickNpcCombat()
        {
            if (!StandMod.Config.npcUsers) return;
            foreach (Map map in Find.Maps)
            {
                IReadOnlyList<Pawn> pawns = map.mapPawns.AllPawnsSpawned;
                foreach (Pawn user in pawns)
                {
                    if (user.Dead || user.Faction == null || user.Faction == Faction.OfPlayer ||
                        user.Downed || !StandSystem.HasStand(user)) continue;
                    Hediff_StandBond bond = StandSystem.GetBond(user);
                    if (bond.Stand.kind != StandKind.Manifested) continue;
                    foreach (Pawn target in pawns)
                    {
                        if (target == user || target.Dead || !target.Spawned ||
                            target.Faction == null || !user.Faction.HostileTo(target.Faction)) continue;
                        if (user.Position.DistanceTo(target.Position) > bond.Stand.attackRange) continue;
                        bond.Manifested = true;
                        if (StandSystem.Punch(user, target)) break;
                    }
                }
            }
        }
    }
}
