using UnityEngine;
using TestApp.Mesh;

namespace TestApp.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private MeshGenerator generator;

        private Rigidbody cachedRigidbody;
        private Vector3 velocity;
        private Vector3 direction;

        private CollisionProcessor collisionProcessor;
        private GameObject bullet;

        public Vector3 Direction
        {
            get
            {
                return direction;
            }
        }

        private void Start()
        {
            cachedRigidbody = GetComponent<Rigidbody>();

            collisionProcessor = gameObject.AddComponent<CollisionProcessor>();
            collisionProcessor.Init(generator);
        }

        private void Update()
        {
            velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * 10;

            if (velocity != Vector3.zero)
            {
                direction = velocity / 10f;
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (bullet != null)
                {
                    return;
                }
                Shoot();
            }
        }

        private void FixedUpdate()
        {
            cachedRigidbody.MovePosition(cachedRigidbody.position + velocity * Time.fixedDeltaTime);
        }

        private void Shoot()
        {
            bullet = Instantiate(Resources.Load<GameObject>("Prefabs/bullet"));
            bullet.GetComponent<Bullet>().Init(direction, transform.position, collisionProcessor);
        }

        private void OnCollisionEnter(Collision collision)
        {
            collisionProcessor.ProcessCollision(collision);
        }
    }
}

