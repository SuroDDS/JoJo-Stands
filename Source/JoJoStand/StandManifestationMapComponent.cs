using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace JoJoStand
{
    /// <summary>
    /// Draws Star Platinum as a non-physical four-directional world sprite.
    /// The 4x2 atlas contains columns South, East, North, West and
    /// rows Idle, Attack. No extra Pawn/Thing is created.
    /// </summary>
    public sealed class StandManifestationMapComponent : MapComponent
    {
        private const int RefreshIntervalTicks = 60;
        private const int AttackAnimationTicks = 18;
        private const float DrawScale = 1.10f;
        private const string AtlasPath = "Stands/JJ_StarPlatinum/StarPlatinumAtlas";

        private readonly List<Pawn> owners = new List<Pawn>();
        private int lastRefreshTick = -1;

        private static Material atlasMaterial;
        private static Mesh[,] atlasMeshes; // [row: idle/attack, column: S/E/N/W]

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
            EnsureGraphics();

            foreach (Pawn owner in owners)
            {
                if (owner == null || owner.Dead || !owner.Spawned ||
                    owner.Map != map || owner.Drawer == null)
                    continue;

                Hediff_StandBond bond = StandSystem.GetBond(owner);
                if (bond == null || !bond.Awakened || !bond.Manifested ||
                    bond.Stand == null || bond.Stand.defName != "JJ_StarPlatinum")
                    continue;

                // Explicit atlas column mapping. Do not rely on Rot4's numeric order.
                int column;
                if (owner.Rotation == Rot4.South) column = 0;
                else if (owner.Rotation == Rot4.East) column = 1;
                else if (owner.Rotation == Rot4.North) column = 2;
                else column = 3;

                int age = tick - bond.LastVisualPunchTick;
                bool attacking = age >= 0 && age < AttackAnimationTicks;

                Vector3 drawPos = owner.DrawPos;
                // Stand stays to the rear-side, with directional offsets in X/Z cells.
                // The pawn's draw position/rotation are read each frame, so it follows movement.
                switch (column)
                {
                    case 0: drawPos.x += 0.47f; drawPos.z += 0.24f; break;
                    case 1: drawPos.x -= 0.35f; drawPos.z += 0.32f; break;
                    case 2: drawPos.x -= 0.47f; drawPos.z += 0.21f; break;
                    default: drawPos.x += 0.35f; drawPos.z += 0.32f; break;
                }

                // Small idle hover, deterministic per owner; no gameplay displacement.
                drawPos.z += 0.018f * Mathf.Sin((tick + owner.thingIDNumber * 7) * 0.065f);
                if (attacking)
                {
                    float progress = (float)age / AttackAnimationTicks;
                    float lunge = 0.24f * Mathf.Sin(progress * Mathf.PI);
                    if (column == 1) drawPos.x += lunge;
                    else if (column == 3) drawPos.x -= lunge;
                    else drawPos.z += column == 0 ? -lunge : lunge;
                }

                drawPos.y += 0.05f;
                Graphics.DrawMesh(atlasMeshes[attacking ? 1 : 0, column],
                    Matrix4x4.TRS(drawPos, Quaternion.identity, Vector3.one * DrawScale),
                    atlasMaterial, 0);
            }
        }

        private static void EnsureGraphics()
        {
            if (atlasMaterial == null)
                atlasMaterial = MaterialPool.MatFrom(AtlasPath, ShaderDatabase.Transparent);

            if (atlasMeshes != null) return;
            atlasMeshes = new Mesh[2, 4];
            for (int row = 0; row < 2; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    float u0 = column * 0.25f;
                    float u1 = (column + 1) * 0.25f;
                    // The source PNG's first (idle) row is at the TOP;
                    // Unity UV origin is bottom-left.
                    float v0 = row == 0 ? 0.5f : 0f;
                    float v1 = row == 0 ? 1f : 0.5f;

                    Mesh mesh = new Mesh
                    {
                        name = "JoJo_StarPlatinum_" + row + "_" + column,
                        vertices = new[]
                        {
                            new Vector3(-0.5f, 0f, -0.5f),
                            new Vector3(-0.5f, 0f, 0.5f),
                            new Vector3(0.5f, 0f, 0.5f),
                            new Vector3(0.5f, 0f, -0.5f)
                        },
                        uv = new[]
                        {
                            new Vector2(u0, v0),
                            new Vector2(u0, v1),
                            new Vector2(u1, v1),
                            new Vector2(u1, v0)
                        },
                        triangles = new[] { 0, 1, 2, 0, 2, 3 }
                    };
                    mesh.RecalculateBounds();
                    atlasMeshes[row, column] = mesh;
                }
            }
        }

        private void RefreshOwners()
        {
            owners.Clear();
            if (map == null) return;
            foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
                if (pawn != null && !pawn.Dead && StandSystem.HasStand(pawn))
                    owners.Add(pawn);
        }
    }
}
