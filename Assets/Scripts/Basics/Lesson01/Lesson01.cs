using System;
using UnityEngine;

namespace Basics.Lesson01
{
    public class Lesson01 : MonoBehaviour
    {
        private const float hours2Degrees = -30.0f;
        private const float minutes2Degrees = -6.0f;
        private const float seconds2Degrees = -6.0f;

        [SerializeField] private Transform hoursPivot;
        [SerializeField] private Transform minutesPivot;
        [SerializeField] private Transform secondsPivot;

        private void Awake()
        {
            var now = DateTime.Now;
            hoursPivot.localRotation = Quaternion.Euler(0.0f, 0.0f, hours2Degrees * now.Hour);
            minutesPivot.localRotation = Quaternion.Euler(0.0f, 0.0f, minutes2Degrees * now.Minute);
            secondsPivot.localRotation = Quaternion.Euler(0.0f, 0.0f, seconds2Degrees * now.Second);
        }

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            var now = DateTime.Now.TimeOfDay;

            hoursPivot.localRotation = Quaternion.Euler(0.0f, 0.0f, hours2Degrees * (float)now.TotalHours);
            minutesPivot.localRotation = Quaternion.Euler(0.0f, 0.0f, minutes2Degrees * (float)now.TotalMinutes);
            secondsPivot.localRotation = Quaternion.Euler(0.0f, 0.0f, seconds2Degrees * (float)now.TotalSeconds);
        }
    }
}