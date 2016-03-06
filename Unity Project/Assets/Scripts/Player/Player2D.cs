using UnityEngine;

namespace TestApp.Player
{
    public class Player2D : MonoBehaviour
    {
        private Rigidbody2D cachedRigidbody;
        private Vector2 velocity;

        private void Start()
        {
            cachedRigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * 10;
        }

        private void FixedUpdate()
        {
            cachedRigidbody.MovePosition(cachedRigidbody.position + velocity * Time.fixedDeltaTime);
        }
    }
}
