using System;
using UnityEngine;

namespace Basics
{
    /// <summary>
    /// Fractal,分形
    /// </summary>
    public class Lesson06 : MonoBehaviour
    {
        private static Vector3[] directions = new Vector3[]
        {
            Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back,
        };

        private static Quaternion[] quaternions = new Quaternion[]
        {
            Quaternion.identity,
            Quaternion.Euler(0.0f, 0.0f, -90.0f),
            Quaternion.Euler(0.0f, 0.0f, 90.0f),
            Quaternion.Euler(90.0f, 0.0f, 0.0f),
            Quaternion.Euler(-90.0f, 0.0f, 0.0f),
        };

        private struct FractalPart
        {
            public Vector3 direction;
            public Quaternion rotation;
            public Transform transform;
        }

        private FractalPart[][] parts;

        [SerializeField, Range(1, 8)] private int depth = 4;
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;

        private void Awake()
        {
            parts = new FractalPart[depth][];
            for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
            {
                parts[i] = new FractalPart[length];
            }

            var scale = 1.0f;
            parts[0][0] = CreatePart(0, 0, scale);
            for (var i = 1; i < parts.Length; i++)
            {
                scale *= 0.5f;
                var levelParts = parts[i];
                for (var j = 0; j < levelParts.Length; j += 5)
                {
                    for (var k = 0; k < 5; k++)
                    {
                        levelParts[j + k] = CreatePart(i, k, scale);
                    }
                }
            }
        }

        private FractalPart CreatePart(int levelIdx, int childIdx, float scale)
        {
            var go = new GameObject($"Fractal Part L{levelIdx} C{childIdx}")
            {
                transform =
                {
                    localScale = scale * Vector3.one
                }
            };
            go.transform.SetParent(transform, false);

            go.AddComponent<MeshFilter>().mesh = mesh;
            go.AddComponent<MeshRenderer>().material = material;

            return new FractalPart
            {
                direction = directions[childIdx],
                rotation = quaternions[childIdx],
                transform = go.transform
            };
        }

        private void Update()
        {
            var deltaRotation = Quaternion.Euler(0f, 22.5f * Time.deltaTime, 0f);

            var rootPart = parts[0][0];
            rootPart.rotation *= deltaRotation;
            rootPart.transform.localRotation = rootPart.rotation;
            parts[0][0] = rootPart;

            for (var i = 1; i < parts.Length; i++)
            {
                var parentParts = parts[i - 1];
                var levelParts = parts[i];
                for (var j = 0; j < levelParts.Length; j++)
                {
                    var parentTransform = parentParts[j / 5].transform;
                    var part = levelParts[j];

                    part.rotation *= deltaRotation;

                    var localRotation = parentTransform.localRotation;
                    part.transform.localRotation = localRotation * part.rotation;
                    part.transform.localPosition = (
                        parentTransform.localPosition +
                        localRotation * (1.5f * part.transform.localScale.x * part.direction)
                    );

                    levelParts[j] = part;
                }
            }
        }
    }
}