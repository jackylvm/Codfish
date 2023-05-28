using System;
using TMPro;
using UnityEngine;

namespace Lesson04
{
    public class FrameRateCounter : MonoBehaviour
    {
        public enum DisplayMode
        {
            FPS,
            MS
        }

        [SerializeField] private TextMeshProUGUI display;
        [SerializeField, Range(0.1f, 2.0f)] private float sampleDuration = 1.0f;
        [SerializeField] private DisplayMode displayMode = DisplayMode.FPS;

        private int frames;

        private float duration;
        private float bestDuration = float.MaxValue;

        private float worstDuration;

        // Update is called once per frame
        private void Update()
        {
            var frameDuration = Time.unscaledDeltaTime;

            frames += 1;
            duration += frameDuration;

            if (frameDuration < bestDuration)
            {
                bestDuration = frameDuration;
            }

            if (frameDuration > worstDuration)
            {
                worstDuration = frameDuration;
            }

            if (duration >= sampleDuration)
            {
                switch (displayMode)
                {
                    case DisplayMode.FPS:
                        display.SetText(
                            $"FPS\nA:{frames / duration:F}\nB:{1.0f / bestDuration:F}\nW:{1.0f / worstDuration:F}"
                        );
                        break;
                    case DisplayMode.MS:
                        display.SetText(
                            $"MS\n{1000.0f * bestDuration:F}\n{1000.0f * duration / frames:F}\n{1000.0f * worstDuration:F}"
                        );
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                frames = 0;
                duration = 0.0f;

                bestDuration = float.MaxValue;
                worstDuration = 0.0f;
            }
        }
    }
}