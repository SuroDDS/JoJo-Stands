using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JoJoStand
{
    /// <summary>
    /// Purely visual stand manifestation. Does not spawn another Thing or Pawn,
    /// so it cannot interfere with pathing, melee combat, target selection or saves.
    /// Only a cheap list refresh runs on a timer; all actual drawing is per-frame.
    /// </summary>
    public sealed class StandManifestationMapComponent : MapComponent
    {
        private const int RefreshIntervalTicks = 60;
        private const int AttackAnimationTicks = 18;
        private readonly List<Pawn> owners = new List<Pawn>();
        private int lastRefreshTick = -1;
        private static Material starPlatinumMaterial;

        public StandManifestationMapComponent(Map map) : base(map) { }

        public override void MapComponentUpdate()
        {
            if (Find.CurrentMap != map || StandMod.Config == null ||
                !StandMod.Config.showStandVisuals || Find.TickManager == null)
                return;

            int tick = Find.TickManager.TicksGame;
            if (lastRefreshTick < 0 || tick < lastRefreshTick ||
                tick - lastRefreshTick >= RefreshIntervalTicks)
            {
                RefreshOwners();
                lastRefreshTick = tick;
            }

            if (owners.Count == 0) return;
            if (starPlatinumMaterial == null)
                starPlatinumMaterial = MaterialPool.MatFrom(
                    "Things/StandManifestation/StarPlatinum",
                    ShaderDatabase.Transparent);

            foreach (Pawn owner in owners)
            {
                if (owner == null || owner.Dead || !owner.Spawned ||
                    owner.Map != map || owner.Drawer == null)
                    continue;

                Hediff_StandBond bond = StandSystem.GetBond(owner);
                if (bond == null || !bond.Awakened || !bond.Manifested ||
                    bond.Stand == null || bond.Stand.defName != "JJ_StarPlatinum")
                    continue;

                // Temporary omniscient player rendering. Simulated perception
                // and line-of-sight rules are intentionally NOT implemented yet.
                Vector3 drawPos = owner.DrawPos;
                float direction = owner.Rotation == Rot4.West ? 1f : -1f;
                drawPos.x += direction * 0.50f;
                drawPos.z += 0.33f;
                drawPos.y += 0.05f;

                // Small displacement synchronized with the successful punch.
                int age = tick - bond.LastVisualPunchTick;
                if (age >= 0 && age < AttackAnimationTicks)
                {
                    float progress = (float)age / AttackAnimationTicks;
                    drawPos.x += direction * 0.28f * Mathf.Sin(progress * Mathf.PI);
                }

                Graphics.DrawMesh(MeshPool.plane14, drawPos,
                    Quaternion.identity, starPlatinumMaterial, 0);
            }
        }

        private void RefreshOwners()
        {
            owners.Clear();
            if (map == null) return;
            foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
            {
                if (pawn != null && !pawn.Dead && StandSystem.HasStand(pawn))
                    owners.Add(pawn);
            }
        }
    }
}
