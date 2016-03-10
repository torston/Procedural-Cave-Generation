using UnityEngine;
using TestApp.Mesh;

public class CollisionProcessor : MonoBehaviour
{
    private MeshGenerator generator;

    public void Init(MeshGenerator generator)
    {
        this.generator = generator;
    }

    public bool ProcessCollision(Collision collision)
    {
        if (collision.gameObject.tag != "wall")
        {
            return false;
        }
        collision.contacts[0].thisCollider.enabled = false;
        generator.Collision(collision.contacts[0].point);
        collision.contacts[0].thisCollider.enabled = true;
        return true;
    }
}
