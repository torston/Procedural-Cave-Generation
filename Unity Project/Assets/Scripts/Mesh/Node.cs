using UnityEngine;
using System;

namespace TestApp.Mesh
{
	[Serializable]
    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 position)
        {
            this.position = position;
        }
    }
}