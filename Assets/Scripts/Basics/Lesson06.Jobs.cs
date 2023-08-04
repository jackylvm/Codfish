using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using quaternion = Unity.Mathematics.quaternion;

namespace Basics
{
    public partial class Lesson06
    {
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct UpdateFractalLevelJob : IJobFor
        {
            public float spinAngleDelta;
            public float scale;

            [ReadOnly] public NativeArray<FractalPart> parents;
            public NativeArray<FractalPart> parts;

            [WriteOnly] public NativeArray<float3x4> matrices;

            public void Execute(int idx)
            {
                var parent = parents[idx / 5];
                var part = parts[idx];

                part.spinAngle += spinAngleDelta;
                part.worldRotation = mul(
                    parent.worldRotation,
                    mul(part.rotation, quaternion.RotateY(part.spinAngle))
                );
                part.worldPosition = parent.worldPosition + mul(parent.worldRotation, 1.5f * scale * part.direction);
                // part.worldRotation = parent.worldRotation * (part.rotation * Quaternion.Euler(0.0f, part.spinAngle, 0.0f));
                // part.worldPosition = parent.worldPosition + parent.worldRotation * (1.5f * scale * part.direction);

                parts[idx] = part;

                var r = float3x3(part.worldRotation) * scale;
                matrices[idx] = float3x4(r.c0, r.c1, r.c2, part.worldPosition);
                // matrices[idx] = Matrix4x4.TRS(part.worldPosition, part.worldRotation, float3(scale));
            }
        }
    }
}