using UnityEngine;

namespace Lesson02
{
    public class Lesson02 : MonoBehaviour
    {
        [SerializeField] private Transform pointPrefab;
        [SerializeField, Range(10, 100)] private int resolution = 10;

        private Transform[] points;

        private void Awake()
        {
            var step = 2.0f / resolution;
            var scale = Vector3.one * step;
            var position = Vector3.zero;

            points = new Transform[resolution];
            for (var i = 0; i < points.Length; i++)
            {
                position.x = ((i + 0.5f) * step - 1f);

                var point = points[i] = Instantiate(pointPrefab, transform, false);
                point.localPosition = position;
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
            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i];

                var position = point.localPosition;
                position.y = Mathf.Sin((position.x + Time.time) * Mathf.PI);
                point.localPosition = position;
            }
        }
    }
}