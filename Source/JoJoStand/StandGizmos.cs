using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace JoJoStand
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
    public static class PawnGizmosPatch
    {
        // The declared types must match exactly (Harmony 3 checks injections strictly).
        public static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            IEnumerable<Gizmo> previous = __result;
            __result = AddStandGizmos(__instance, previous);
        }

        private static IEnumerable<Gizmo> AddStandGizmos(Pawn pawn, IEnumerable<Gizmo> original)
        {
            if (original != null)
                foreach (Gizmo gizmo in original) yield return gizmo;

            if (!pawn.IsColonistPlayerControlled || pawn.Dead || !pawn.Spawned ||
                !StandSystem.CanHaveStand(pawn)) yield break;

            Hediff_StandBond bond = StandSystem.GetBond(pawn);
            if (bond == null || !bond.Awakened)
            {
                Thing arrow = FindAvailableArrow(pawn);
                Command_Action useArrow = new Command_Action
                {
                    defaultLabel = "JJ_UseArrow".Translate(),
                    defaultDesc = "JJ_UseArrowDesc".Translate(),
                    action = () =>
                    {
                        Thing currentArrow = FindAvailableArrow(pawn);
                        if (currentArrow != null) StandSystem.PierceWithArrow(pawn, currentArrow);
                    }
                };
                if (arrow == null) useArrow.Disable("JJ_NoArrow".Translate());
                yield return useArrow;
                yield break;
            }

            if (bond.Stand == null || bond.Stand.kind != StandKind.Manifested) yield break;
            yield return new Command_Action
            {
                defaultLabel = bond.Manifested ? "JJ_Recall".Translate() : "JJ_Summon".Translate(),
                defaultDesc = "JJ_SummonDesc".Translate(bond.Stand.label),
                action = () => { bond.Manifested = !bond.Manifested; }
            };
            if (!bond.Manifested) yield break;

            Command_Action punch = new Command_Action
            {
                defaultLabel = "JJ_Punch".Translate(),
                defaultDesc = "JJ_PunchDesc".Translate(),
                action = () =>
                {
                    TargetingParameters parameters = new TargetingParameters
                    {
                        canTargetPawns = true,
                        canTargetBuildings = true,
                        canTargetItems = false
                    };
                    Find.Targeter.BeginTargeting(parameters, (LocalTargetInfo target) =>
                    {
                        if (!StandSystem.Punch(pawn, target.Thing))
                            Messages.Message("JJ_PunchInvalid".Translate(),
                                MessageTypeDefOf.RejectInput);
                    });
                }
            };
            if (Find.TickManager.TicksGame < bond.NextPunchTick)
                punch.Disable("JJ_OnCooldown".Translate());
            yield return punch;
        }

        public static Thing FindAvailableArrow(Pawn pawn)
        {
            ThingDef def = DefDatabase<ThingDef>.GetNamed("JJ_StandArrow", false);
            if (pawn == null || def == null) return null;
            if (pawn.inventory != null)
            {
                foreach (Thing held in pawn.inventory.innerContainer)
                    if (held.def == def) return held;
            }
            if (!pawn.Spawned || pawn.Map == null) return null;
            for (int x = -1; x <= 1; x++)
            for (int z = -1; z <= 1; z++)
            {
                IntVec3 cell = pawn.Position + new IntVec3(x, 0, z);
                if (!cell.InBounds(pawn.Map)) continue;
                foreach (Thing thing in cell.GetThingList(pawn.Map))
                    if (thing.def == def) return thing;
            }
            return null;
        }
    }
}
