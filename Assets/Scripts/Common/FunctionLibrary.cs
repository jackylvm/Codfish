using UnityEngine;
using static UnityEngine.Mathf;

namespace Common
{
    public static class FunctionLibrary
    {
        public delegate Vector3 Function(float u, float v, float t);

        public enum EmFunction
        {
            Wave,
            MultiWave01,
            MultiWave02,
            Ripple,
            Sphere01,
            Sphere02,
            Torus,
        }

        private static readonly Function[] functions = new Function[]
        {
            Wave, MultiWave01, MultiWave02, Ripple, Sphere01, Sphere02, Torus
        };

        public static Function GetFunction(EmFunction name)
        {
            return functions[(int) name];
        }

        public static EmFunction GetNextFunctionName(EmFunction name)
        {
            if ((int) name < functions.Length - 1)
            {
                return name + 1;
            }

            return 0;
        }

        public static EmFunction GetRandomFunctionNameOtherThan(EmFunction name)
        {
            var choice = (EmFunction) Random.Range(1, functions.Length);
            return choice == name ? 0 : choice;
        }

        public static Vector3 Morph(
            float u, float v, float t, Function from, Function to, float progress
        )
        {
            return Vector3.LerpUnclamped(from(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress));
        }

        private static Vector3 Wave(float u, float v, float t)
        {
            var p = Vector3.zero;
            p.x = u;
            p.y = Sin(PI * (u + v + t));
            p.z = v;
            return p;
        }

        private static Vector3 MultiWave01(float u, float v, float t)
        {
            var p = Vector3.zero;
            p.x = u;
            p.z = v;

            var y = Sin(PI * (u + 0.5f * t));
            y += 0.5f * Sin(2.0f * PI * (v + t));
            p.y = y * (2.0f / 3.0f);
            return p;
        }

        private static Vector3 MultiWave02(float u, float v, float t)
        {
            var p = Vector3.zero;
            p.x = u;
            p.z = v;

            var y = Sin(PI * (u + 0.5f * t));
            y += 0.5f * Sin(2.0f * PI * (v + t));
            y += Sin(PI * (u + v + 0.25f * t));
            p.y = y * (1.0f / 2.5f);
            return p;
        }

        private static Vector3 Ripple(float u, float v, float t)
        {
            var p = Vector3.zero;
            p.x = u;
            p.z = v;

            var d = Sqrt(u * u + v * v);
            var y = Sin(PI * (4.0f * d - t));
            p.y = y / (1.0f + 10.0f * d);
            return p;
        }

        private static Vector3 Sphere01(float u, float v, float t)
        {
            var r = Cos(0.5f * PI * v);

            var p = Vector3.zero;
            p.x = r * Sin(PI * u);
            p.y = Sin(PI * 0.5f * v);
            p.z = r * Cos(PI * u);

            return p;
        }

        private static Vector3 Sphere02(float u, float v, float t)
        {
            // 可挑战半径值,实现不同的效果
            // var r = 0.5f + 0.5f * Sin(PI * t);
            // var r = 0.9f + 0.1f * Sin(8.0f * PI * u);
            // var r = 0.9f + 0.1f * Sin(8.0f * PI * v);
            var r = 0.9f + 0.1f * Sin(PI * (6.0f * u + 4.0f * v + t));

            var s = r * Cos(0.5f * PI * v);

            var p = Vector3.zero;
            p.x = s * Sin(PI * u);
            p.y = r * Sin(0.5f * PI * v);
            p.z = s * Cos(PI * u);

            return p;
        }

        private static Vector3 Torus(float u, float v, float t)
        {
            // 圆环
            // var r1 = 0.75f;
            // var r2 = 0.25f;

            // 可动圆环
            var r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
            var r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));

            var s = r1 + r2 * Cos(PI * v);

            var p = Vector3.zero;
            p.x = s * Sin(PI * u);
            p.y = r2 * Sin(PI * v);
            p.z = s * Cos(PI * u);

            return p;
        }
    }
}