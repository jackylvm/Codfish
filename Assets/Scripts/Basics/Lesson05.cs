using Common;
using UnityEngine;

namespace Basics
{
    public class Lesson05 : MonoBehaviour
    {
        private enum TransitionMode
        {
            Cycle,
            Random
        }

        private const int maxResolution = 1000;

        private static readonly int positionsId = Shader.PropertyToID("_Positions");
        private static readonly int resolutionId = Shader.PropertyToID("_Resolution");
        private static readonly int stepId = Shader.PropertyToID("_Step");
        private static readonly int timeId = Shader.PropertyToID("_Time");
        private static readonly int transitionProgressId = Shader.PropertyToID("_TransitionProgress");

        [SerializeField, Range(10, maxResolution)]
        private int resolution = 10;

        [SerializeField] private FunctionLibrary.EmFunction function;
        [SerializeField, Min(0.0f)] private float functionDuration = 1.0f;
        [SerializeField, Min(0.0f)] private float transitionDuration = 1.0f;
        [SerializeField] private TransitionMode transitionMode;
        [SerializeField] private ComputeShader computeShader;
        [SerializeField] private Material material;
        [SerializeField] private Mesh mesh;

        private float duration;
        private bool transitioning;

        private ComputeBuffer m_PositionsBuffer;

        private FunctionLibrary.EmFunction transitionFunction;

        private void OnEnable()
        {
            m_PositionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
        }

        private void OnDisable()
        {
            if (m_PositionsBuffer != null)
            {
                m_PositionsBuffer.Release();
                m_PositionsBuffer = null;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            duration += Time.deltaTime;
            if (transitioning)
            {
                if (duration >= transitionDuration)
                {
                    duration -= transitionDuration;
                    transitioning = false;
                }
            }
            else if (duration >= functionDuration)
            {
                duration -= functionDuration;
                transitioning = true;
                transitionFunction = function;
                PickNextFunction();
            }

            UpdateFunctionOnGPU();
        }

        private void PickNextFunction()
        {
            function = transitionMode == TransitionMode.Cycle
                ? FunctionLibrary.GetNextFunctionName(function)
                : FunctionLibrary.GetRandomFunctionNameOtherThan(function);
        }

        private void UpdateFunctionOnGPU()
        {
            float step = 2f / resolution;
            computeShader.SetInt(resolutionId, resolution);
            computeShader.SetFloat(stepId, step);
            computeShader.SetFloat(timeId, Time.time);
            if (transitioning)
            {
                computeShader.SetFloat(
                    transitionProgressId,
                    Mathf.SmoothStep(0f, 1f, duration / transitionDuration)
                );
            }

            var kernelIndex =
                (int) function +
                (int) (transitioning ? transitionFunction : function) *
                FunctionLibrary.FunctionCount;
            computeShader.SetBuffer(kernelIndex, positionsId, m_PositionsBuffer);

            int groups = Mathf.CeilToInt(resolution / 8f);
            computeShader.Dispatch(kernelIndex, groups, groups, 1);

            material.SetBuffer(positionsId, m_PositionsBuffer);
            material.SetFloat(stepId, step);
            var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
            Graphics.DrawMeshInstancedProcedural(
                mesh, 0, material, bounds, resolution * resolution
            );
        }
    }
}