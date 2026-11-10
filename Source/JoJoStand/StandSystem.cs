using System.Collections.Generic;
using RimWorld;
using Verse;

namespace JoJoStand
{
    /// <summary>
    /// Cross-cutting gameplay API. No extra Pawn is spawned for the Stand itself.
    /// This is intentional: intangible Stands cannot be attacked as ordinary units.
    /// </summary>
    public static class StandSystem
    {
        public static Hediff_StandBond GetBond(Pawn pawn)
        {
            if (pawn == null || pawn.health == null) return null;
            return pawn.health.hediffSet.GetFirstHediffOfDef(
                DefDatabase<HediffDef>.GetNamed("JJ_StandBond")) as Hediff_StandBond;
        }

        public static bool CanHaveStand(Pawn pawn)
        {
            return pawn != null && !pawn.Dead && pawn.health != null &&
                   pawn.RaceProps != null && (pawn.RaceProps.Humanlike || pawn.RaceProps.Animal);
        }

        public static Hediff_StandBond AddLatentBond(Pawn pawn)
        {
            if (!CanHaveStand(pawn)) return null;
            Hediff_StandBond current = GetBond(pawn);
            if (current != null) return current;
            HediffDef def = DefDatabase<HediffDef>.GetNamed("JJ_StandBond");
            pawn.health.AddHediff(def);
            return GetBond(pawn);
        }

        public static bool Awaken(Pawn pawn, bool announce)
        {
            if (!CanHaveStand(pawn)) return false;
            Hediff_StandBond bond = AddLatentBond(pawn);
            if (bond == null || bond.Awakened) return false;

            StandDef choice = PickWeightedStand();
            if (choice == null)
            {
                Log.Error("[JoJoStands] No StandDefs loaded.");
                return false;
            }
            bond.Stand = choice;
            bond.Awakened = true;
            bond.Manifested = false;
            bond.NextPunchTick = 0;
            if (announce && pawn.Faction == Faction.OfPlayer)
            {
                Messages.Message("JJ_Awakened".Translate(pawn.LabelShort, choice.label),
                    pawn, MessageTypeDefOf.PositiveEvent);
            }
            return true;
        }

        private static StandDef PickWeightedStand()
        {
            IReadOnlyList<StandDef> defs = DefDatabase<StandDef>.AllDefsListForReading;
            float sum = 0f;
            for (int i = 0; i < defs.Count; i++)
                if (defs[i].selectionWeight > 0f) sum += defs[i].selectionWeight;
            if (sum <= 0f) return null;
            float roll = Rand.Value * sum;
            for (int i = 0; i < defs.Count; i++)
            {
                StandDef def = defs[i];
                if (def.selectionWeight <= 0f) continue;
                roll -= def.selectionWeight;
                if (roll <= 0f) return def;
            }
            return defs[defs.Count - 1];
        }

        public static bool HasStand(Pawn pawn)
        {
            Hediff_StandBond bond = GetBond(pawn);
            return bond != null && bond.Awakened && bond.Stand != null;
        }

        public static bool CanSeeStand(Pawn observer)
        {
            // Visibility for intangible Stands. Bound/object Stands will need a distinct rule.
            return HasStand(observer);
        }

        public static bool CanPunch(Pawn owner, Thing target)
        {
            Hediff_StandBond bond = GetBond(owner);
            if (bond == null || !bond.Awakened || !bond.Manifested || bond.Stand == null ||
                bond.Stand.kind != StandKind.Manifested || owner == null || owner.Dead ||
                !owner.Spawned || target == null || !target.Spawned || target.Map != owner.Map ||
                target == owner || Find.TickManager == null) return false;
            if (Find.TickManager.TicksGame < bond.NextPunchTick) return false;
            if (owner.Position.DistanceTo(target.Position) > bond.Stand.attackRange) return false;
            return GenSight.LineOfSight(owner.Position, target.Position, owner.Map);
        }

        public static bool Punch(Pawn owner, Thing target)
        {
            if (!CanPunch(owner, target)) return false;
            Hediff_StandBond bond = GetBond(owner);
            bond.NextPunchTick = Find.TickManager.TicksGame + bond.Stand.punchCooldownTicks;
            target.TakeDamage(new DamageInfo(DamageDefOf.Blunt, bond.Stand.punchDamage,
                0f, -1f, owner));
            return true;
        }

        /// <summary>
        /// Deterministic eligibility check: repeating map scans cannot reroll the same Pawn.
        /// More detailed psychological fitness can be added without changing the arrow API.
        /// </summary>
        public static float WorthinessChance(Pawn pawn)
        {
            Hediff_StandBond bond = GetBond(pawn);
            return bond != null ? 0.90f : (StandMod.Config?.arrowWorthyChance ?? 0.55f);
        }

        public static void PierceWithArrow(Pawn pawn, Thing arrow)
        {
            if (!CanHaveStand(pawn) || HasStand(pawn) || arrow == null || arrow.Destroyed) return;
            arrow.Destroy(DestroyMode.Vanish);
            if (Rand.Value < WorthinessChance(pawn))
            {
                Awaken(pawn, true);
                return;
            }
            Messages.Message("JJ_ArrowRejected".Translate(pawn.LabelShort), pawn,
                MessageTypeDefOf.NegativeEvent);
            bool lethal = (StandMod.Config?.lethalArrowFailure ?? true) && Rand.Value < 0.35f;
            if (lethal) pawn.Kill(null);
            else pawn.TakeDamage(new DamageInfo(DamageDefOf.Stab, 14f));
        }
    }
}
