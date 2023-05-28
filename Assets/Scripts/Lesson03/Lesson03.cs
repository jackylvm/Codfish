using Common;
using UnityEngine;

namespace Lesson03
{
    public class Lesson03 : MonoBehaviour
    {
        [SerializeField] private Transform pointPrefab;
        [SerializeField, Range(10, 100)] private int resolution = 10;
        [SerializeField] private FunctionLibrary.EmFunction function;

        private Transform[] points;

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

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
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