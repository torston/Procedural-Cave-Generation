using UnityEngine;

namespace TestApp.Player
{
    public class Player : MonoBehaviour
    {
        private Rigidbody cachedRigidbody;
        private Vector3 velocity;

        private void Start()
        {
            cachedRigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * 10;
        }

        private void FixedUpdate()
        {
            cachedRigidbody.MovePosition(cachedRigidbody.position + velocity * Time.fixedDeltaTime);
        }
    }
}

