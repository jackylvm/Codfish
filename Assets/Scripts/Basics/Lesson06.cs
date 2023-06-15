using UnityEngine;

namespace Basics
{
    /// <summary>
    /// Fractal,分形
    /// </summary>
    public class Lesson06 : MonoBehaviour
    {
        [SerializeField, Range(1, 8)] private int depth = 4;

        // Start is called before the first frame update
        private void Start()
        {
            name = $"Fractal {depth}";
            if (depth <= 1)
            {
                return;
            }

            var childA = CreateChild(Vector3.up, Quaternion.identity);
            var childB = CreateChild(Vector3.right, Quaternion.Euler(0.0f, 0.0f, -90.0f));
            var childC = CreateChild(Vector3.left, Quaternion.Euler(0.0f, 0.0f, 90.0f));
            var childD = CreateChild(Vector3.forward, Quaternion.Euler(90.0f, 0.0f, 0.0f));
            var childE = CreateChild(Vector3.back, Quaternion.Euler(-90.0f, 0.0f, 0.0f));

            childA.transform.SetParent(transform, false);
            childB.transform.SetParent(transform, false);
            childC.transform.SetParent(transform, false);
            childD.transform.SetParent(transform, false);
            childE.transform.SetParent(transform, false);
        }

        private Lesson06 CreateChild(Vector3 direction, Quaternion quaternion)
        {
            var child = Instantiate(this);
            child.depth = depth - 1;

            var transform1 = child.transform;
            transform1.localPosition = 0.75f * direction;
            transform1.localRotation = quaternion;
            transform1.localScale = 0.5f * Vector3.one;
            return child;
        }

        // Update is called once per frame
        private void Update()
        {
            transform.Rotate(0.0f, 22.5f * Time.deltaTime, 0.0f);
        }
    }
}