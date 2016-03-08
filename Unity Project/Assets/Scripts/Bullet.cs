using UnityEngine;
using System.Collections;
using TestApp.Mesh;

public class Bullet : MonoBehaviour 
{
	public static bool bulletActive;

	Vector3 direction;
	float time = 1f;
	float timeFlying = 0f;
	MeshGenerator gen;

	public void Init (Vector3 direction, Vector3 startPos, MeshGenerator gen) 
	{

		Menu.Instance.Bullets--;
		this.gen = gen;
		gameObject.layer = 9;
		transform.position = startPos;
		bulletActive = true;
		this.direction = direction;

		var rig = gameObject.AddComponent<Rigidbody> ();
	
		rig.detectCollisions = true;
		rig.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rig.interpolation = RigidbodyInterpolation.Extrapolate;

    }

	void FixedUpdate () 
	{
		transform.Translate (direction, Space.World);

		timeFlying += Time.fixedDeltaTime;

		if (timeFlying >= time) {
			DestroyBullet ();
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag != "wall") 
		{
			return;
		}
        GetComponent<Collider>().enabled = false;
        gen.Collision (collision.contacts [0].point);
		DestroyBullet ();
	}

	void DestroyBullet(){
		bulletActive = false;
		Destroy (gameObject);
	}
}
