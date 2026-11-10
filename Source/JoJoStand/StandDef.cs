using Verse;

namespace JoJoStand
{
    public enum StandKind
    {
        Manifested,
        Swarm,
        Bound,
        Autonomous,
        Passive
    }

    /// <summary>
    /// Data-driven stand definitions. Only Manifested has a usable ability in v0.1.
    /// Future content can add StandDefs without modifying the persistent save format.
    /// </summary>
    public sealed class StandDef : Def
    {
        public StandKind kind = StandKind.Manifested;
        public float selectionWeight = 1f;
        public float attackRange = 2.9f;
        public float punchDamage = 14f;
        public int punchCooldownTicks = 180;
    }
}
