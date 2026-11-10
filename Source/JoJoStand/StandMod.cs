using HarmonyLib;
using UnityEngine;
using Verse;

namespace JoJoStand
{
    public sealed class StandSettings : ModSettings
    {
        public bool npcUsers = true;
        public bool animalUsers = false;
        public bool inheritPotential = true;
        public bool naturalAwakening = true;
        public bool lethalArrowFailure = true;
        public float npcChance = 0.03f;
        public float animalChance = 0.005f;
        public float arrowWorthyChance = 0.55f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref npcUsers, "npcUsers", true);
            Scribe_Values.Look(ref animalUsers, "animalUsers", false);
            Scribe_Values.Look(ref inheritPotential, "inheritPotential", true);
            Scribe_Values.Look(ref naturalAwakening, "naturalAwakening", true);
            Scribe_Values.Look(ref lethalArrowFailure, "lethalArrowFailure", true);
            Scribe_Values.Look(ref npcChance, "npcChance", 0.03f);
            Scribe_Values.Look(ref animalChance, "animalChance", 0.005f);
            Scribe_Values.Look(ref arrowWorthyChance, "arrowWorthyChance", 0.55f);
        }
    }

    public sealed class StandMod : Mod
    {
        public static StandSettings Config;

        public StandMod(ModContentPack content) : base(content)
        {
            Config = GetSettings<StandSettings>();
        }

        public override string SettingsCategory() => "JJ_ModTitle".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard list = new Listing_Standard();
            list.Begin(inRect);
            list.CheckboxLabeled("JJ_EnableNpc".Translate(), ref Config.npcUsers);
            list.CheckboxLabeled("JJ_EnableAnimals".Translate(), ref Config.animalUsers);
            list.CheckboxLabeled("JJ_InheritPotential".Translate(), ref Config.inheritPotential);
            list.CheckboxLabeled("JJ_NaturalAwakening".Translate(), ref Config.naturalAwakening);
            list.CheckboxLabeled("JJ_LethalArrow".Translate(), ref Config.lethalArrowFailure);
            list.GapLine();
            list.Label("JJ_NpcChance".Translate(Config.npcChance.ToStringPercent()));
            Config.npcChance = list.Slider(Config.npcChance, 0f, 0.20f);
            list.Label("JJ_AnimalChance".Translate(Config.animalChance.ToStringPercent()));
            Config.animalChance = list.Slider(Config.animalChance, 0f, 0.05f);
            list.Label("JJ_ArrowChance".Translate(Config.arrowWorthyChance.ToStringPercent()));
            Config.arrowWorthyChance = list.Slider(Config.arrowWorthyChance, 0.05f, 0.95f);
            list.End();
        }
    }

    [StaticConstructorOnStartup]
    internal static class StandBootstrap
    {
        static StandBootstrap()
        {
            new Harmony("local.jojostands.prototype")
    .PatchAll(typeof(StandMod).Assembly);
            Log.Message("[JoJoStands] Prototype 0.1 initialized.");
        }
    }
}
