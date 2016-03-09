using UnityEngine;
using System.Collections.Generic;

namespace TestApp.Mesh
{
    public class MeshGenerator : MonoBehaviour
    {
        public SquareGrid squareGrid;
        public MeshFilter walls;
        public Transform caveTransform;

        private List<Vector3> vertices;

        private Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
        private readonly List<List<int>> outlines = new List<List<int>>();
        private readonly HashSet<int> checkedVertices = new HashSet<int>();

        private GameObject[,] meshes;

        public void Collision(Vector3 position)
        {
            squareGrid.FindCollisionPoint(position);

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

        private void CreateMesh()
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

                    CreateMeshPart(x, y);
                }
            }
        }

        private void CreateMeshPart(int x, int y)
        {
            var go = Instantiate(Resources.Load<GameObject>("Prefabs/meshTemplate"));
            go.name = x + " " + y;
            go.transform.parent = caveTransform;
            meshes[x, y] = go;

            var meshFilter = go.GetComponent<MeshFilter>();

            UnityEngine.Mesh mesh = new UnityEngine.Mesh();

            meshFilter.mesh = mesh;

            mesh.vertices = squareGrid.squares[x, y].vertsTemp.vertsList.ToArray();
            mesh.triangles = squareGrid.squares[x, y].vertsTemp.triandles.ToArray();
            mesh.RecalculateNormals();

            go.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Player");
        }

        private void CreateWallMesh()
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

        private void TriangulateSquare(Square square)
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

                    square.vertsTemp.checkedVertices.Add(square.topLeft.vertexIndex - square.vertsTemp.firstIndex);
                    square.vertsTemp.checkedVertices.Add(square.topRight.vertexIndex - square.vertsTemp.firstIndex);
                    square.vertsTemp.checkedVertices.Add(square.bottomRight.vertexIndex - square.vertsTemp.firstIndex);
                    square.vertsTemp.checkedVertices.Add(square.bottomLeft.vertexIndex - square.vertsTemp.firstIndex);

                    checkedVertices.Add(square.topLeft.vertexIndex);
                    checkedVertices.Add(square.topRight.vertexIndex);
                    checkedVertices.Add(square.bottomRight.vertexIndex);
                    checkedVertices.Add(square.bottomLeft.vertexIndex);
                    break;
            }

        }

        private void MeshFromPoints(Square square, params Node[] points)
        {
            VertWithIndexes info = new VertWithIndexes();

            square.vertsTemp = info;
            square.vertsTemp.firstIndex = vertices.Count;

            AssignVertices(points, info);

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

        private void AssignVertices(Node[] points, VertWithIndexes info)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);

                info.vertsList.Add(points[i].position);
            }
        }

        private void CreateTriangle(Square square, Node a, Node b, Node c)
        {
            square.vertsTemp.triandles.Add(a.vertexIndex - square.vertsTemp.firstIndex);
            square.vertsTemp.triandles.Add(b.vertexIndex - square.vertsTemp.firstIndex);
            square.vertsTemp.triandles.Add(c.vertexIndex - square.vertsTemp.firstIndex);
        }

        private void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
        {

            if (triangleDictionary.ContainsKey(vertexIndexKey))
            {
                triangleDictionary[vertexIndexKey].Add(triangle);
            }
            else
            {
                List<Triangle> triangleList = new List<Triangle>();
                triangleList.Add(triangle);
                triangleDictionary.Add(vertexIndexKey, triangleList);
            }
        }

        private void CalculateMeshOutlines()
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
                        vertices.Add(vertice);
                    }

                    foreach (var checkedVertex in squareGrid.squares[x, y].vertsTemp.checkedVertices)
                    {
                        checkedVertices.Add(checkedVertex + count);
                    }

                    if (squareGrid.squares[x, y].vertsTemp.triandles.Count > 0)
                    {
                        for (int i = 0; i < squareGrid.squares[x, y].vertsTemp.triandles.Count -1 ; i += 3)
                        {
                            Triangle triangle = new Triangle(
                                squareGrid.squares[x, y].vertsTemp.triandles[i] + count,
                                squareGrid.squares[x, y].vertsTemp.triandles[i + 1] + count,
                                squareGrid.squares[x, y].vertsTemp.triandles[i + 2] + count
                                );

                            AddTriangleToDictionary(triangle.vertexIndexA, triangle);
                            AddTriangleToDictionary(triangle.vertexIndexB, triangle);
                            AddTriangleToDictionary(triangle.vertexIndexC, triangle);
                        }
                    }
                }
            }

            for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
            {
                if (!checkedVertices.Contains(vertexIndex))
                {
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

        private void SimplifyMeshOutlines()
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

        private void FollowOutline(int vertexIndex, int outlineIndex)
        {
            outlines[outlineIndex].Add(vertexIndex);
            checkedVertices.Add(vertexIndex);
            int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

            if (nextVertexIndex != -1)
            {
                FollowOutline(nextVertexIndex, outlineIndex);
            }
        }

        private int GetConnectedOutlineVertex(int vertexIndex)
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

        private bool IsOutlineEdge(int vertexA, int vertexB)
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

    public class VertWithIndexes
    {
        public List<Vector3> vertsList = new List<Vector3>();
        public List<int> triandles = new List<int>();
        public HashSet<int> checkedVertices = new HashSet<int>();


        public int firstIndex = -1;
    }
}