using UnityEngine;

namespace DesertDash.World
{
    public sealed class PickupSpin : MonoBehaviour
    {
        [SerializeField] private float degreesPerSecond = 150f;

        private void Update()
        {
            transform.Rotate(0f, degreesPerSecond * Time.deltaTime, 0f, Space.World);
        }
    }
}
