using Common;
using UnityEngine;

namespace Basics.Lesson04
{
    public class Lesson04 : MonoBehaviour
    {
        public enum TransitionMode
        {
            Cycle,
            Random
        }

        [SerializeField] private Transform pointPrefab;
        [SerializeField, Range(10, 100)] private int resolution = 10;
        [SerializeField] private FunctionLibrary.EmFunction function;
        [SerializeField, Min(0.0f)] private float functionDuration = 1.0f;
        [SerializeField, Min(0.0f)] private float transitionDuration = 1.0f;
        [SerializeField] private TransitionMode transitionMode;

        private Transform[] points;

        private float duration;
        private bool transitioning;

        private FunctionLibrary.EmFunction transitionFunction;

        private void Awake()
        {
            var step = 2.0f / resolution;
            var scale = Vector3.one * step;

            points = new Transform[resolution * resolution];
            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i] = Instantiate(pointPrefab, transform, false);
                point.localScale = scale;
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

            if (transitioning)
            {
                UpdateFunctionTransition();
            }
            else
            {
                UpdateFunction();
            }
        }

        private void PickNextFunction()
        {
            function = transitionMode == TransitionMode.Cycle
                ? FunctionLibrary.GetNextFunctionName(function)
                : FunctionLibrary.GetRandomFunctionNameOtherThan(function);
        }

        private void UpdateFunctionTransition()
        {
            FunctionLibrary.Function
                from = FunctionLibrary.GetFunction(transitionFunction),
                to = FunctionLibrary.GetFunction(function);
            var progress = duration / transitionDuration;
            var time = Time.time;
            var step = 2f / resolution;
            var v = 0.5f * step - 1f;
            for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
            {
                if (x == resolution)
                {
                    x = 0;
                    z += 1;
                    v = ((z + 0.5f) * step - 1f);
                }

                var u = ((x + 0.5f) * step - 1f);
                points[i].localPosition = FunctionLibrary.Morph(
                    u, v, time, from, to, progress
                );
            }
        }

        private void UpdateFunction()
        {
            var fDelegate = FunctionLibrary.GetFunction(function);

            var time = Time.time;
            var step = 2.0f / resolution;

            var v = 0.5f * step - 1f;
            for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
            {
                if (x == resolution)
                {
                    x = 0;
                    z += 1;
                    v = ((z + 0.5f) * step - 1f);
                }

                var u = ((x + 0.5f) * step - 1f);
                points[i].localPosition = fDelegate(u, v, time);
            }
        }
    }
}