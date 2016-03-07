using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace TestApp.Mesh
{
	[Serializable]
    public class Square
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centreTop, centreRight, centreBottom, centreLeft;
        public int configuration;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;

            centreTop = topLeft.right;
            centreRight = bottomRight.above;
            centreBottom = bottomLeft.right;
            centreLeft = bottomLeft.above;

            if (topLeft.active)
                configuration += 8;
            if (topRight.active)
                configuration += 4;
            if (bottomRight.active)
                configuration += 2;
            if (bottomLeft.active)
                configuration += 1;
        }

		public void DrawSquare() {
		
			var list = new List<Node> ();

			list.Add (topLeft);
			list.Add (topRight);
			list.Add (bottomRight);
			list.Add (bottomLeft);
			list.Add (centreTop);
			list.Add (centreRight);
			list.Add (centreBottom);
			list.Add (centreLeft);

			foreach (var item in list) 
			{
									var obj = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				obj.transform.position = item.position;
									obj.transform.localScale = Vector3.one * 0.1f;
			}

		}

    }
}