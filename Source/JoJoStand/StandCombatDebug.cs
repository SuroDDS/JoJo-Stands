using System.Collections.Generic;
using RimWorld;
using Verse;

namespace JoJoStand
{
    /// <summary>
    /// Opt-in diagnostics for the Stand attack pipeline.
    /// Does not modify damage types or the vanilla combat log.
    /// NPC rejection reports are throttled to avoid log flooding.
    /// </summary>
    public static class StandCombatDebug
    {
        private const int NpcRejectIntervalTicks = 600;
        private static readonly Dictionary<int, int> LastNpcRejectTick = new Dictionary<int, int>();

        public static bool Enabled => StandMod.Config != null && StandMod.Config.combatDebug;

        public static void Hit(Pawn owner, Thing target, StandDef stand)
        {
            if (!Enabled || owner == null || stand == null) return;

            int now = Find.TickManager != null ? Find.TickManager.TicksGame : -1;
            string targetName = target != null ? target.LabelShort : "<null>";
            float distance = target != null && target.Spawned && owner.Spawned
                ? owner.Position.DistanceTo(target.Position) : -1f;

            Log.Message("[JoJoStands][COMBAT][HIT] tick=" + now
                + " stand=" + stand.defName
                + " owner=" + owner.LabelShort
                + " target=" + targetName
                + " baseDamage=" + stand.punchDamage.ToString("0.#")
                + " type=Blunt distance=" + distance.ToString("0.##")
                + " cooldown=" + stand.punchCooldownTicks);

            if (target != null && Find.CurrentMap != null && target.Map == Find.CurrentMap)
            {
                Messages.Message("JJ_DebugHit".Translate(
                    stand.label, owner.LabelShort, targetName,
                    stand.punchDamage.ToString("0.#")),
                    target, MessageTypeDefOf.NeutralEvent, false);
            }
        }

        public static void Rejected(Pawn owner, Thing target, string reason, bool manual)
        {
            if (!Enabled) return;

            int now = Find.TickManager != null ? Find.TickManager.TicksGame : 0;
            if (!manual && owner != null)
            {
                int previous;
                if (LastNpcRejectTick.TryGetValue(owner.thingIDNumber, out previous)
                    && now - previous < NpcRejectIntervalTicks) return;
                LastNpcRejectTick[owner.thingIDNumber] = now;
            }

            Log.Message("[JoJoStands][COMBAT][REJECT] tick=" + now
                + " owner=" + (owner != null ? owner.LabelShort : "<null>")
                + " target=" + (target != null ? target.LabelShort : "<null>")
                + " reason=" + reason);
        }
    }
}
