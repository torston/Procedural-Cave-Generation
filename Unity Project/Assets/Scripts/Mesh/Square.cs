using System;
using System.Collections.Generic;
using UnityEngine;

namespace TestApp.Mesh
{
    [Serializable]
    public class Square
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centreTop, centreRight, centreBottom, centreLeft;
        public int configuration;

        public VertWithIndexes vertsTemp;
        public bool needRecalculate = true;

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

        public void CheckNodes(ControlNode controlNode)
        {
            var controlNodes = new List<Node>
            {
                topLeft, topRight, bottomRight, bottomLeft
            };

            if (!controlNodes.Contains(controlNode))
            {
                return;
            }

            configuration = 0;

            needRecalculate = true;

            vertsTemp.vertsList = new List<Vector3>();
            vertsTemp.triandles = new List<int>();
            vertsTemp.checkedVertices = new HashSet<int>();

            CheckNodes();
        }
    }
}