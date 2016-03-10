using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 direction;

    private float time = 1f;
    private float timeFlying = 0f;

    private CollisionProcessor collisionProcessor;

    private void FixedUpdate()
    {
        transform.Translate(direction, Space.World);

        timeFlying += Time.fixedDeltaTime;

        if (timeFlying >= time)
        {
            DestroyBullet();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collisionProcessor.ProcessCollision(collision))
        {
            return;
        }
        DestroyBullet();
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }

    public void Init(Vector3 direction, Vector3 position, CollisionProcessor collisionProcessor)
    {
        transform.position = position;

        this.direction = direction;
        this.collisionProcessor = collisionProcessor;
    }
}
