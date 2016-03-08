using UnityEngine;
using System;
namespace TestApp.Mesh
{
    [Serializable]
    public class SquareGrid
    {
        public Square[,] squares;
        public ControlNode[,] controlNodes;


        public float squareSize = 0f;
        public int[,] map;

        public SquareGrid(int[,] map, float squareSize)
        {
            this.squareSize = squareSize;
            this.map = map;

            CreateSquare();
        }

        public void CreateSquare()
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX*squareSize;
            float mapHeight = nodeCountY*squareSize;

            controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth/2 + x*squareSize + squareSize/2, 0,
                        -mapHeight/2 + y*squareSize + squareSize/2);
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1],
                        controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }

        }

        public void FindCollisionPoint(Vector3 position)
        {
            float dist = float.MaxValue;
            int x1 = 0;
            int y1 = 0;
            for (int x = 0; x < controlNodes.GetLength(0); x++)
            {
                for (int y = 0; y < controlNodes.GetLength(1); y++)
                {
                    if (!controlNodes[x, y].active)
                    {
                        continue;
                    }
                    if (Vector3.Distance(controlNodes[x, y].position, position) < dist)
                    {
                        dist = Vector3.Distance(controlNodes[x, y].position, position);
                        x1 = x;
                        y1 = y;
                    }
                }
            }

            map[x1, y1] = 0;
            controlNodes[x1, y1].active = false;

            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);

            //Debug.Log("Collision node " + x1 + " " + y1);

            //for (int i = -1; i < 2; i++)
            //{
            //    for (int j = -1; j < 2; j++)
            //    {
            //        if (squares[x1 + i, y1 + j].CheckNodes(controlNodes[x1 + i, y1 + j]))
            //        {
            //            Debug.Log(" (1) Affected vertex " + (x1 + i) + " " + (y1 + j));
            //        }
            //    }
            //}

            

            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    if (squares[x, y].CheckNodes(controlNodes[x1, y1]))
                    {
                        //squares[x, y].DrawSquare(controlNodes[x1, y1]);
                        //Debug.Log("Affected vertex " + x + " " + y);
                    }

                }
            }
        }
    }
}