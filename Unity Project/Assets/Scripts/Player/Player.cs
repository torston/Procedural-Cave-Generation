using UnityEngine;
using TestApp.Mesh;

namespace TestApp.Player
{
    public class Player : MonoBehaviour
    {
        private Rigidbody cachedRigidbody;
        private Vector3 velocity;
		private Vector3 direction;
		public MeshGenerator generator;

        private void Start()
        {
            cachedRigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * 10;

			if (velocity != Vector3.zero) {
				direction = velocity / 10f;
			}

			if (Input.GetKeyUp(KeyCode.Space)) {

				if (Bullet.bulletActive) {
					return;
				}
				Shoot ();
			}
        }

		private void Shoot()
		{
			var go = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			go.transform.localScale = Vector3.one * 1f;

			go.AddComponent<Bullet> ().Init(direction, gameObject.transform.position, generator);
		}

        private void FixedUpdate()
        {
            cachedRigidbody.MovePosition(cachedRigidbody.position + velocity * Time.fixedDeltaTime);
        }

		void OnCollisionEnter(Collision collision)
		{
			//Debug.Log (1);

			if (collision.gameObject.tag != "wall") {
				return;
			}
            GetComponent<Collider>().enabled = false;
            //foreach (var contact in collision.contacts)
		    {

                generator.Collision(collision.contacts[0].point);
                var contactPoint = collision.collider.ClosestPointOnBounds(collision.contacts[0].point);

            }

            GetComponent<Collider>().enabled = true;

            return;
		}
    }
}

