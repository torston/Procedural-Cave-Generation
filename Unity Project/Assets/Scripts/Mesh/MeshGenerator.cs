using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TestApp.Mesh
{
    public class MeshGenerator : MonoBehaviour
    {
        public SquareGrid squareGrid;
        public MeshFilter walls;
        public MeshFilter cave;

        List<Vector3> vertices;

        Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
        List<List<int>> outlines = new List<List<int>>();
        HashSet<int> checkedVertices = new HashSet<int>();

        private GameObject[,] meshes;


        public void Collision(Vector3 position)
        {
            squareGrid.FindCollisionPoint(position);

            //triangleDictionary.Clear();
            outlines.Clear();
            checkedVertices.Clear();

            CreateMesh();
            CreateWallMesh();
        }

        public void GenerateMesh(int[,] map, float squareSize)
        {
            triangleDictionary.Clear();
            outlines.Clear();
            checkedVertices.Clear();

            squareGrid = new SquareGrid(map, squareSize);
            meshes = new GameObject[map.GetLength(0), map.GetLength(1)];
            CreateMesh();
            CreateWallMesh();
        }

        void CreateMesh()
        {
            int count = 0;
            for (int x = 0; x < squareGrid.map.GetLength(0); x++)
            {
                for (int y = 0; y < squareGrid.map.GetLength(1); y++)
                {
                    count += squareGrid.map[x, y];
                }
            }
            Menu.Instance.Nodes = count;
            vertices = new List<Vector3>();

            for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
                {
                    if (squareGrid.squares[x, y].needRecalculate)
                    {
                        //Debug.Log("Recalculate node " + x + " " + y);
                        TriangulateSquare(squareGrid.squares[x, y]);
                    }
                    else
                    {
                        if (squareGrid.squares[x, y].vertsTemp != null)
                        {
                            vertices.AddRange(squareGrid.squares[x, y].vertsTemp.vertsList);
                        }

                    }
                }
            }

            for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
                {
                    if (!squareGrid.squares[x, y].needRecalculate)
                    {
                        continue;
                    }

                    squareGrid.squares[x, y].needRecalculate = false;

                    if (meshes[x, y] != null)
                    {
                        Destroy(meshes[x, y]);
                    }

                    if (squareGrid.squares[x, y].vertsTemp == null)
                    {
                        continue;
                    }



                    var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.name = x + " " + y;

                    meshes[x, y] = go;
                    var meshF = go.GetComponent<MeshFilter>();
                    Destroy(meshF.GetComponent<Collider>());
                    UnityEngine.Mesh tmp = new UnityEngine.Mesh();
                    meshF.mesh = tmp;

                    tmp.vertices = squareGrid.squares[x, y].vertsTemp.vertsList.ToArray();
                    tmp.triangles = squareGrid.squares[x, y].triandles.ToArray();
                    tmp.RecalculateNormals();

                    go.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Player");

                }
            }
        }

        void CreateWallMesh()
        {
            MeshCollider currentCollider = GetComponent<MeshCollider>();
            Destroy(currentCollider);
            CalculateMeshOutlines();

            List<Vector3> wallVertices = new List<Vector3>();
            List<int> wallTriangles = new List<int>();
            UnityEngine.Mesh wallMesh = new UnityEngine.Mesh();
            float wallHeight = 5;

            foreach (List<int> outline in outlines)
            {
                for (int i = 0; i < outline.Count - 1; i++)
                {
                    int startIndex = wallVertices.Count;
                    wallVertices.Add(vertices[outline[i]]); // left
                    wallVertices.Add(vertices[outline[i + 1]]); // right
                    wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); // bottom left
                    wallVertices.Add(vertices[outline[i + 1]] - Vector3.up * wallHeight); // bottom right

                    wallTriangles.Add(startIndex + 0);
                    wallTriangles.Add(startIndex + 2);
                    wallTriangles.Add(startIndex + 3);

                    wallTriangles.Add(startIndex + 3);
                    wallTriangles.Add(startIndex + 1);
                    wallTriangles.Add(startIndex + 0);
                }
            }
            wallMesh.vertices = wallVertices.ToArray();
            wallMesh.triangles = wallTriangles.ToArray();
            walls.mesh = wallMesh;

            MeshCollider wallCollider = gameObject.AddComponent<MeshCollider>();
            wallCollider.sharedMesh = wallMesh;

        }

        void TriangulateSquare(Square square)
        {
            switch (square.configuration)
            {
                case 0:
                    break;

                // 1 points:
                case 1:
                    MeshFromPoints(square, square.centreLeft, square.centreBottom, square.bottomLeft);
                    break;
                case 2:
                    MeshFromPoints(square, square.bottomRight, square.centreBottom, square.centreRight);
                    break;
                case 4:
                    MeshFromPoints(square, square.topRight, square.centreRight, square.centreTop);
                    break;
                case 8:
                    MeshFromPoints(square, square.topLeft, square.centreTop, square.centreLeft);
                    break;

                // 2 points:
                case 3:
                    MeshFromPoints(square, square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                    break;
                case 6:
                    MeshFromPoints(square, square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
                    break;
                case 9:
                    MeshFromPoints(square, square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
                    break;
                case 12:
                    MeshFromPoints(square, square.topLeft, square.topRight, square.centreRight, square.centreLeft);
                    break;
                case 5:
                    MeshFromPoints(square, square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
                    break;
                case 10:
                    MeshFromPoints(square, square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
                    break;

                // 3 point:
                case 7:
                    MeshFromPoints(square, square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                    break;
                case 11:
                    MeshFromPoints(square, square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
                    break;
                case 13:
                    MeshFromPoints(square, square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
                    break;
                case 14:
                    MeshFromPoints(square, square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
                    break;

                // 4 point:
                case 15:
                    MeshFromPoints(square, square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);

                    square.checkedVertices.Add(square.topLeft.vertexIndex - square.firstIndex);
                    square.checkedVertices.Add(square.topRight.vertexIndex - square.firstIndex);
                    square.checkedVertices.Add(square.bottomRight.vertexIndex - square.firstIndex);
                    square.checkedVertices.Add(square.bottomLeft.vertexIndex - square.firstIndex);

                    checkedVertices.Add(square.topLeft.vertexIndex);
                    checkedVertices.Add(square.topRight.vertexIndex);
                    checkedVertices.Add(square.bottomRight.vertexIndex);
                    checkedVertices.Add(square.bottomLeft.vertexIndex);
                    break;
            }

        }

        void MeshFromPoints(Square square, params Node[] points)
        {
            //if (square.firstIndex == -1)
            {
                square.firstIndex = vertices.Count;
            }
            //var localIndex = vertices.Count;

            square.vertsTemp = AssignVertices(points);
            square.triangleL.Clear();

            if (points.Length >= 3)
            {
                CreateTriangle(square, points[0], points[1], points[2]);
            }
            if (points.Length >= 4)
            {
                CreateTriangle(square, points[0], points[2], points[3]);
            }
            if (points.Length >= 5)
            {
                CreateTriangle(square, points[0], points[3], points[4]);
            }
            if (points.Length >= 6)
            {
                CreateTriangle(square, points[0], points[4], points[5]);
            }
        }

        public class VertWithIndexes
        {
            public List<Vector3> vertsList = new List<Vector3>();
            public List<int> vertIndexes = new List<int>();
        }

        VertWithIndexes AssignVertices(Node[] points)
        {
            VertWithIndexes info = new VertWithIndexes();

            for (int i = 0; i < points.Length; i++)
            {
                //if (info.firstIndex == -1)
                //{
                //    info.firstIndex = vertices.Count;
                //}
                //if (points[i].vertexIndex == -1)
                {
                    points[i].vertexIndex = vertices.Count;
                    vertices.Add(points[i].position);

                    info.vertIndexes.Add(points[i].vertexIndex);
                    info.vertsList.Add(points[i].position);


                }
            }

            return info;
        }

        void CreateTriangle(Square square, Node a, Node b, Node c)
        {
            square.triandles.Add(a.vertexIndex - square.firstIndex);
            square.triandles.Add(b.vertexIndex - square.firstIndex);
            square.triandles.Add(c.vertexIndex - square.firstIndex);

            Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
            square.triangleL.Add(triangle);
            //AddTriangleToDictionary(triangle.vertexIndexA, triangle);
            //AddTriangleToDictionary(triangle.vertexIndexB, triangle);
            //AddTriangleToDictionary(triangle.vertexIndexC, triangle);
        }

        void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
        {

            if (triangleDictionary.ContainsKey(vertexIndexKey))
            {
                calc++;
                triangleDictionary[vertexIndexKey].Add(triangle);
            }
            else
            {
                calc2++;
                List<Triangle> triangleList = new List<Triangle>();
                triangleList.Add(triangle);
                triangleDictionary.Add(vertexIndexKey, triangleList);
            }
        }

        int calc = 0;
        int calc2 = 0;

        void CalculateMeshOutlines()
        {
            checkedVertices.Clear();
            vertices.Clear();

            triangleDictionary = new Dictionary<int, List<Triangle>>();

            for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
                {
                    if (squareGrid.squares[x, y].vertsTemp == null)
                    {
                        continue;
                    }

                    var count = vertices.Count;
                    foreach (var vertice in squareGrid.squares[x, y].vertsTemp.vertsList)
                    {
                        //if (vertices.Count == 9135)
                        //{
                        //    Debug.Log(x + " - " + y);
                        //}
                        vertices.Add(vertice);
                    }

                    foreach (var checkedVertex in squareGrid.squares[x, y].checkedVertices)
                    {
                        checkedVertices.Add(checkedVertex + count);
                    }


                    if (squareGrid.squares[x, y].triangleL.Count > 0)
                    {
                        var mod = 0;
                        foreach (var triangle1 in squareGrid.squares[x, y].triangleL)
                        {
                            Triangle triangle = 
                                //triangle1;
                            new Triangle(
                            squareGrid.squares[x, y].triandles[mod] + count,
                            squareGrid.squares[x, y].triandles[mod + 1] + count,
                            squareGrid.squares[x, y].triandles[mod + 2] + count
                            );

                            AddTriangleToDictionary(triangle.vertexIndexA, triangle);
                            AddTriangleToDictionary(triangle.vertexIndexB, triangle);
                            AddTriangleToDictionary(triangle.vertexIndexC, triangle);

                            mod += 3;
                        }

                    }
                }
            }
            for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
            {
                if (!checkedVertices.Contains(vertexIndex))
                {
                    //while (!triangleDictionary.ContainsKey(vertexIndex))
                    //{
                    //    vertexIndex++;
                    //}
                    int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                    if (newOutlineVertex != -1)
                    {
                        checkedVertices.Add(vertexIndex);

                        List<int> newOutline = new List<int>();
                        newOutline.Add(vertexIndex);
                        outlines.Add(newOutline);
                        FollowOutline(newOutlineVertex, outlines.Count - 1);
                        outlines[outlines.Count - 1].Add(vertexIndex);
                    }
                }
            }

            SimplifyMeshOutlines();
        }

        void SimplifyMeshOutlines()
        {
            for (int outlineIndex = 0; outlineIndex < outlines.Count; outlineIndex++)
            {
                List<int> simplifiedOutline = new List<int>();
                Vector3 dirOld = Vector3.zero;
                for (int i = 0; i < outlines[outlineIndex].Count; i++)
                {
                    Vector3 p1 = vertices[outlines[outlineIndex][i]];
                    Vector3 p2 = vertices[outlines[outlineIndex][(i + 1) % outlines[outlineIndex].Count]];
                    Vector3 dir = p1 - p2;
                    if (dir != dirOld)
                    {
                        dirOld = dir;
                        simplifiedOutline.Add(outlines[outlineIndex][i]);
                    }
                }
                outlines[outlineIndex] = simplifiedOutline;
            }
        }

        void FollowOutline(int vertexIndex, int outlineIndex)
        {
            outlines[outlineIndex].Add(vertexIndex);
            checkedVertices.Add(vertexIndex);
            int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

            if (nextVertexIndex != -1)
            {
                FollowOutline(nextVertexIndex, outlineIndex);
            }
        }

        int GetConnectedOutlineVertex(int vertexIndex)
        {
            if (!triangleDictionary.ContainsKey(vertexIndex))
            {
                Debug.Log(vertexIndex);
            }
            List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

            for (int i = 0; i < trianglesContainingVertex.Count; i++)
            {
                Triangle triangle = trianglesContainingVertex[i];

                for (int j = 0; j < 3; j++)
                {
                    int vertexB = triangle[j];
                    if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB))
                    {
                        if (IsOutlineEdge(vertexIndex, vertexB))
                        {
                            return vertexB;
                        }
                    }
                }
            }

            return -1;
        }

        bool IsOutlineEdge(int vertexA, int vertexB)
        {
            List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];
            int sharedTriangleCount = 0;

            for (int i = 0; i < trianglesContainingVertexA.Count; i++)
            {
                if (trianglesContainingVertexA[i].Contains(vertexB))
                {
                    sharedTriangleCount++;
                    if (sharedTriangleCount > 1)
                    {
                        break;
                    }
                }
            }
            return sharedTriangleCount == 1;
        }
    }
}
