using UnityEngine;
using System;

namespace TestApp.Mesh
{
	[Serializable]
    public class ControlNode : Node
    {

        public bool active;
        public Node above, right;

        public ControlNode(Vector3 pos, bool active, float squareSize) : base(pos)
        {
            this.active = active;
            above = new Node(position + Vector3.forward * squareSize / 2f);
            
//			var obj = GameObject.CreatePrimitive (PrimitiveType.Sphere);
//			obj.transform.position = position + Vector3.forward * squareSize / 2f;
//			obj.transform.localScale = Vector3.one * 0.1f;
//			obj.GetComponent<MeshRenderer> ().material = Resources.Load<Material> ("Player");
			right = new Node(position + Vector3.right * squareSize / 2f);

//			var obj1 = GameObject.CreatePrimitive (PrimitiveType.Sphere);
//			obj1.transform.position = position + Vector3.right * squareSize / 2f;
//			obj1.transform.localScale = Vector3.one * 0.1f;
//			obj1.GetComponent<MeshRenderer> ().material = Resources.Load<Material> ("Player");


        }

    }
}