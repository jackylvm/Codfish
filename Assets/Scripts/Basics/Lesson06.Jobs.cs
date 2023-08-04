using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Basics
{
    public partial class Lesson06
    {
        [BurstCompile(CompileSynchronously = true)]
        private struct UpdateFractalLevelJob : IJobFor
        {
            public float spinAngleDelta;
            public float scale;

            [ReadOnly] public NativeArray<FractalPart> parents;
            public NativeArray<FractalPart> parts;

            [WriteOnly] public NativeArray<Matrix4x4> matrices;

            public void Execute(int idx)
            {
                var parent = parents[idx / 5];
                var part = parts[idx];

                part.spinAngle += spinAngleDelta;
                part.worldRotation = parent.worldRotation * (part.rotation * Quaternion.Euler(0.0f, part.spinAngle, 0.0f));
                part.worldPosition = parent.worldPosition + parent.worldRotation * (1.5f * scale * part.direction);

                parts[idx] = part;
                matrices[idx] = Matrix4x4.TRS(part.worldPosition, part.worldRotation, scale * Vector3.one);
            }
        }
    }
}