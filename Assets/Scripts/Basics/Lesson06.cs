using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static Unity.Mathematics.math;
using quaternion = Unity.Mathematics.quaternion;

namespace Basics
{
    /// <summary>
    /// Fractal,分形
    /// </summary>
    public partial class Lesson06 : MonoBehaviour
    {
        private static readonly int matricesId = Shader.PropertyToID("_Matrices");

        private static readonly Vector3[] directions =
        {
            up(), right(), left(), forward(), back()
        };

        private static readonly Quaternion[] quaternions =
        {
            quaternion.identity,
            quaternion.RotateZ(-0.5f * PI),
            quaternion.RotateZ(0.5f * PI),
            quaternion.RotateX(0.5f * PI),
            quaternion.RotateX(-0.5f * PI),
        };

        private static MaterialPropertyBlock propertyBlock;

        private struct FractalPart
        {
            public Vector3 direction;
            public Vector3 worldPosition;
            public Quaternion rotation;
            public Quaternion worldRotation;
            public float spinAngle;
        }

        [SerializeField, Range(1, 8)] private int depth = 4;
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;

        // private FractalPart[][] parts;
        private NativeArray<FractalPart>[] parts;

        // private Matrix4x4[][] matrices;
        private NativeArray<Matrix4x4>[] matrices;

        private ComputeBuffer[] matricesBuffers;

        private void Awake()
        {
            parts = new NativeArray<FractalPart>[depth];
            matrices = new NativeArray<Matrix4x4>[depth];
            matricesBuffers = new ComputeBuffer[depth];
            const int stride = 16 * 4;
            for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
            {
                parts[i] = new NativeArray<FractalPart>(length, Allocator.Persistent);
                matrices[i] = new NativeArray<Matrix4x4>(length, Allocator.Persistent);
                matricesBuffers[i] = new ComputeBuffer(length, stride);
            }

            parts[0][0] = CreatePart(0);
            for (var li = 1; li < parts.Length; li++)
            {
                var levelParts = parts[li];
                for (var fpi = 0; fpi < levelParts.Length; fpi += 5)
                {
                    for (var ci = 0; ci < 5; ci++)
                    {
                        levelParts[fpi + ci] = CreatePart(ci);
                    }
                }
            }
        }

        private void OnEnable()
        {
            parts = new NativeArray<FractalPart>[depth];
            matrices = new NativeArray<Matrix4x4>[depth];
            matricesBuffers = new ComputeBuffer[depth];

            const int stride = 16 * 4;
            for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
            {
                parts[i] = new NativeArray<FractalPart>(length, Allocator.Persistent);
                matrices[i] = new NativeArray<Matrix4x4>(length, Allocator.Persistent);
                matricesBuffers[i] = new ComputeBuffer(length, stride);
            }

            parts[0][0] = CreatePart(0);
            for (var li = 1; li < parts.Length; li++)
            {
                var levelParts = parts[li];
                for (var fpi = 0; fpi < levelParts.Length; fpi += 5)
                {
                    for (var ci = 0; ci < 5; ci++)
                    {
                        levelParts[fpi + ci] = CreatePart(ci);
                    }
                }
            }

            propertyBlock ??= new MaterialPropertyBlock();
        }

        private void OnDisable()
        {
            for (var i = 0; i < matricesBuffers.Length; i++)
            {
                matricesBuffers[i].Release();
                parts[i].Dispose();
                matrices[i].Dispose();
            }

            parts = null;
            matrices = null;
            matricesBuffers = null;
        }

        private void OnValidate()
        {
            if (parts != null && enabled)
            {
                OnDisable();
                OnEnable();
            }
        }

        private FractalPart CreatePart(int childIdx)
        {
            return new FractalPart
            {
                direction = directions[childIdx],
                rotation = quaternions[childIdx],
            };
        }

        private void Update()
        {
            var spinAngleDelta = 0.125f * PI * Time.deltaTime;

            var tr = transform;

            var rootPart = parts[0][0];
            rootPart.worldRotation = mul(
                tr.rotation,
                mul(rootPart.rotation, quaternion.RotateY(rootPart.spinAngle))
            );
            // rootPart.worldRotation = tr.rotation * (rootPart.rotation * Quaternion.Euler(0f, rootPart.spinAngle, 0f));
            rootPart.worldPosition = tr.position;
            rootPart.spinAngle = spinAngleDelta;

            parts[0][0] = rootPart;

            var objectScale = tr.lossyScale.x;
            matrices[0][0] = Matrix4x4.TRS(
                rootPart.worldPosition, rootPart.worldRotation, float3(objectScale)
            );
            // matrices[0][0] = Matrix4x4.TRS(
            //     rootPart.worldPosition, rootPart.worldRotation, objectScale * Vector3.one
            // );

            JobHandle jobHandle = default;

            var scale = objectScale;
            for (var li = 1; li < parts.Length; li++)
            {
                scale *= 0.5f;

                jobHandle = new UpdateFractalLevelJob
                {
                    spinAngleDelta = spinAngleDelta,
                    scale = scale,
                    parents = parts[li - 1],
                    parts = parts[li],
                    matrices = matrices[li]
                }.Schedule(parts[li].Length, jobHandle);

                // var job = new UpdateFractalLevelJob
                // {
                //     spinAngleDelta = spinAngleDelta,
                //     scale = scale,
                //     parents = parts[li - 1],
                //     parts = parts[li],
                //     matrices = matrices[li]
                // };
                //
                // jobHandle = job.Schedule(parts[li].Length, jobHandle);
                // job.Schedule(parts[li].Length, default).Complete();
                // for (var fpi = 0; fpi < parts[li].Length; fpi++)
                // {
                //     job.Execute(fpi);
                // }
            }

            jobHandle.Complete();

            for (var i = 0; i < matricesBuffers.Length; i++)
            {
                matricesBuffers[i].SetData(matrices[i]);
            }

            var bounds = new Bounds(rootPart.worldPosition, 3f * objectScale * Vector3.one);
            for (var i = 0; i < matricesBuffers.Length; i++)
            {
                var buffer = matricesBuffers[i];
                buffer.SetData(matrices[i]);
                propertyBlock.SetBuffer(matricesId, buffer);
                Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, buffer.count, propertyBlock);
            }
        }
    }
}