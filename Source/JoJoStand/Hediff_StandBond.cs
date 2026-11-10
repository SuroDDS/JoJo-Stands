using Verse;

namespace JoJoStand
{
    /// <summary>
    /// Stored on the Pawn, so the bond persists through save/load, caravans and captures.
    /// The latent state is deliberately hidden from the health UI.
    /// </summary>
    public sealed class Hediff_StandBond : HediffWithComps
    {
        public StandDef Stand;
        public bool Awakened;
        public bool Manifested;
        public int NextPunchTick;
        // Runtime visual timing only; not gameplay state and not persisted.
        public int LastVisualPunchTick = -100000;
        public int LastAwakeningDay = -1;

        public override bool Visible => Awakened;
        public override string LabelBase => Awakened && Stand != null ? Stand.label : base.LabelBase;
        public override string Description => Awakened && Stand != null ? Stand.description : base.Description;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref Stand, "jjStand");
            Scribe_Values.Look(ref Awakened, "jjAwakened", false);
            Scribe_Values.Look(ref Manifested, "jjManifested", false);
            Scribe_Values.Look(ref NextPunchTick, "jjNextPunchTick", 0);
            Scribe_Values.Look(ref LastAwakeningDay, "jjLastAwakeningDay", -1);
        }
    }
}
