using UnityEngine;

namespace Basics
{
    /// <summary>
    /// Fractal,分形
    /// </summary>
    public class Lesson06 : MonoBehaviour
    {
        static readonly int matricesId = Shader.PropertyToID("_Matrices");

        private static readonly Vector3[] directions =
        {
            Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back,
        };

        private static readonly Quaternion[] quaternions =
        {
            Quaternion.identity,
            Quaternion.Euler(0.0f, 0.0f, -90.0f),
            Quaternion.Euler(0.0f, 0.0f, 90.0f),
            Quaternion.Euler(90.0f, 0.0f, 0.0f),
            Quaternion.Euler(-90.0f, 0.0f, 0.0f),
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

        private FractalPart[][] parts;
        private Matrix4x4[][] matrices;

        private ComputeBuffer[] matricesBuffers;

        private void Awake()
        {
            parts = new FractalPart[depth][];
            matrices = new Matrix4x4[depth][];
            matricesBuffers = new ComputeBuffer[depth];
            const int stride = 16 * 4;
            for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
            {
                parts[i] = new FractalPart[length];
                matrices[i] = new Matrix4x4[length];
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
            parts = new FractalPart[depth][];
            matrices = new Matrix4x4[depth][];
            matricesBuffers = new ComputeBuffer[depth];

            const int stride = 16 * 4;
            for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
            {
                parts[i] = new FractalPart[length];
                matrices[i] = new Matrix4x4[length];
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
            foreach (var buffer in matricesBuffers)
            {
                buffer.Release();
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
            var spinAngleDelta = 22.5f * Time.deltaTime;

            var rootPart = parts[0][0];
            rootPart.worldRotation = rootPart.rotation * Quaternion.Euler(0.0f, spinAngleDelta, 0.0f);
            rootPart.spinAngle = spinAngleDelta;

            parts[0][0] = rootPart;

            matrices[0][0] = Matrix4x4.TRS(
                rootPart.worldPosition, rootPart.worldRotation, Vector3.one
            );

            var scale = 1.0f;
            for (var li = 1; li < parts.Length; li++)
            {
                scale *= 0.5f;
                var parentParts = parts[li - 1];
                var levelParts = parts[li];
                var levelMatrices = matrices[li];
                for (var fpi = 0; fpi < levelParts.Length; fpi++)
                {
                    var parent = parentParts[fpi / 5];
                    var part = levelParts[fpi];

                    part.spinAngle += spinAngleDelta;
                    part.worldRotation = parent.worldRotation * (part.rotation * Quaternion.Euler(0.0f, part.spinAngle, 0.0f));
                    part.worldPosition = (
                        parent.worldPosition +
                        parent.worldRotation * (1.5f * scale * part.direction)
                    );

                    levelParts[fpi] = part;

                    levelMatrices[fpi] = Matrix4x4.TRS(
                        part.worldPosition, part.worldRotation, scale * Vector3.one
                    );
                }
            }

            for (var i = 0; i < matricesBuffers.Length; i++)
            {
                matricesBuffers[i].SetData(matrices[i]);
            }

            var bounds = new Bounds(Vector3.zero, 3f * Vector3.one);
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