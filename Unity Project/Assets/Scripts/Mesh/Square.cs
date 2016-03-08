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

        public MeshGenerator.VertWithIndexes vertsTemp;
        public int firstIndex = -1;
        public List<int> triandles = new List<int>();
        public HashSet<int> checkedVertices = new HashSet<int>();
        public bool needRecalculate = true;
        public List<Triangle> triangleL = new List<Triangle>();

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

            CheckNodes();
        }

        public void DrawSquare(ControlNode f = null)
        {
            var list = new List<Node>();

            list.Add(topLeft);
            list.Add(topRight);
            list.Add(bottomRight);
            list.Add(bottomLeft);
            list.Add(centreTop);
            list.Add(centreRight);
            list.Add(centreBottom);
            list.Add(centreLeft);

            foreach (var item in list)
            {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.transform.position = item.position;
                obj.transform.localScale = Vector3.one * 0.1f;

                if (item == topLeft ||
                    item == topRight ||
                    item == bottomLeft ||
                    item == bottomRight)
                {
                    if (f != null && item == f)
                    {
                        obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Player");
                    }
                    else
                    {
                        obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Red");
                    }

                }
            }

        }

        public void CheckNodes()
        {

            if (topLeft.active)
                configuration += 8;
            if (topRight.active)
                configuration += 4;
            if (bottomRight.active)
                configuration += 2;
            if (bottomLeft.active)
                configuration += 1;
        }

        public bool CheckNodes(ControlNode controlNode)
        {


            var controlNodes = new List<Node>()
            {
                topLeft, topRight, bottomRight, bottomLeft
            };

            if (controlNodes.Contains(controlNode))
            {

                needRecalculate = true;

                configuration = 0;

                vertsTemp = null;
                triandles = new List<int>();
                checkedVertices = new HashSet<int>();
                needRecalculate = true;
                triangleL = new List<Triangle>();

                CheckNodes();


                return true;
            }
            else
            {
                return false;
            }
        }
    }
}