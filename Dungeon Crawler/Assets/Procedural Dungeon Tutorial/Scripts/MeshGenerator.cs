using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class MeshGenerator : MonoBehaviour {

    public NavMeshSurface surface;

    public SquareGrid squareGrid;
    public float wallHeight;

    public Material wallMaterial;
    public Material topMaterial;
    public Material groundMaterial;

    //This is technically for a specific part, the cave
    public int tileAmount;

    Mesh groundMesh;
    Mesh topMesh;
    Mesh wallMesh;

    GameObject ground;
    GameObject top;
    GameObject walls;
    GameObject navMesh;

    List<Vector3> topVertices;
    List<int> topTriangles;

    Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
    List<List<int>> outlines = new List<List<int>>();
    HashSet<int> checkedVertices = new HashSet<int>();

    public void GenerateDungeonMesh(int[,] map, float squareSize, int width, int height) {
        
        triangleDictionary.Clear();
        outlines.Clear();
        checkedVertices.Clear();

        CreateGroundMesh(squareSize, width, height);

        CreateTopMesh(map, squareSize);
        
        CreateWallMesh();

        surface.BuildNavMesh();
    }

    void CreateGroundMesh(float squareSize, int width, int height) {
        Vector3[] groundVertices = new Vector3[4];
        Vector2[] groundUVs = new Vector2[4];
        int[] groundTriangles = new int[6];

        groundVertices[0] = squareSize * new Vector3(-width / 2, 0, height / 2);
        groundVertices[1] = squareSize * new Vector3(width / 2, 0, height / 2);
        groundVertices[2] = squareSize * new Vector3(-width / 2, 0, -height / 2);
        groundVertices[3] = squareSize * new Vector3(width / 2, 0, -height / 2);

        groundUVs[0] = squareSize * new Vector2(-width / 2, height / 2);
        groundUVs[1] = squareSize * new Vector2(width / 2, height / 2);
        groundUVs[2] = squareSize * new Vector2(-width / 2, -height / 2);
        groundUVs[3] = squareSize * new Vector2(width / 2, -height / 2);

        groundTriangles[0] = 0;
        groundTriangles[1] = 1;
        groundTriangles[2] = 2;
        groundTriangles[3] = 2;
        groundTriangles[4] = 1;
        groundTriangles[5] = 3;

        groundMesh = new Mesh();

        groundMesh.vertices = groundVertices;
        groundMesh.uv = groundUVs;
        groundMesh.triangles = groundTriangles;

        ground = new GameObject("Ground", typeof(MeshCollider), typeof(MeshFilter), typeof(MeshRenderer));
        ground.GetComponent<MeshCollider>().sharedMesh = groundMesh;
        ground.GetComponent<MeshFilter>().mesh = groundMesh;
        ground.GetComponent<MeshRenderer>().material = groundMaterial;
    }

    void CreateTopMesh(int[,] map, float squareSize) {
        squareGrid = new SquareGrid(map, squareSize);
        
        topVertices = new List<Vector3>();
        topTriangles = new List<int>();

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++) {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++) {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        topMesh = new Mesh();

        topMesh.vertices = topVertices.ToArray();
        topMesh.triangles = topTriangles.ToArray();

        top = new GameObject("Top", typeof(MeshCollider), typeof(MeshFilter), typeof(MeshRenderer));
        top.GetComponent<MeshCollider>().sharedMesh = topMesh;
        top.GetComponent<MeshFilter>().mesh = topMesh;
        top.GetComponent<MeshRenderer>().material = topMaterial;
        top.GetComponent<Transform>().localPosition += Vector3.up * wallHeight;

        Vector2[] topUVs = new Vector2[topVertices.Count];
        for (int i = 0; i < topVertices.Count; i++) {
            float percentX = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, topVertices[i].x) * tileAmount;
            float percentY = Mathf.InverseLerp(-map.GetLength(1) / 2 * squareSize, map.GetLength(1) / 2 * squareSize, topVertices[i].z) * tileAmount;
            topUVs[i] = new Vector2(percentX, percentY);
        }
        topMesh.uv = topUVs;
    }

    void CreateWallMesh() {

        CalculateMeshOutlines();

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();

        foreach (List<int> outline in outlines) {
            for (int i = 0; i < outline.Count - 1; i++) {
                int startIndex = wallVertices.Count;
                wallVertices.Add(topVertices[outline[i]] + Vector3.up * wallHeight); // left
                wallVertices.Add(topVertices[outline[i + 1]] + Vector3.up * wallHeight); // right
                wallVertices.Add(topVertices[outline[i]] ); // bottom left
                wallVertices.Add(topVertices[outline[i + 1]] ); // bottom right

                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3);

                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 0);
            }
        }

        wallMesh = new Mesh();

        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();

        walls = new GameObject("Walls", typeof(MeshCollider), typeof(MeshFilter), typeof(MeshRenderer));
        walls.GetComponent<MeshCollider>().sharedMesh = wallMesh;
        walls.GetComponent<MeshFilter>().mesh = wallMesh;
        walls.GetComponent<MeshRenderer>().material = wallMaterial;
    }

    void TriangulateSquare(Square square) {
        switch (square.configuration) {
            case 0:
                break;

            // 1 points:
            case 1:
                MeshFromPoints(square.centreLeft, square.centreBottom, square.bottomLeft);
                break;
            case 2:
                MeshFromPoints(square.bottomRight, square.centreBottom, square.centreRight);
                break;
            case 4:
                MeshFromPoints(square.topRight, square.centreRight, square.centreTop);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreLeft);
                break;

            // 2 points:
            case 3:
                MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 6:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreLeft);
                break;
            case 5:
                MeshFromPoints(square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            // 3 point:
            case 7:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            // 4 point:
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                checkedVertices.Add(square.topLeft.vertexIndex);
                checkedVertices.Add(square.topRight.vertexIndex);
                checkedVertices.Add(square.bottomRight.vertexIndex);
                checkedVertices.Add(square.bottomLeft.vertexIndex);
                break;
        }

    }

    void MeshFromPoints(params Node[] points) {
        AssignVertices(points);

        if (points.Length >= 3)
            CreateTriangle(points[0], points[1], points[2]);
        if (points.Length >= 4)
            CreateTriangle(points[0], points[2], points[3]);
        if (points.Length >= 5)
            CreateTriangle(points[0], points[3], points[4]);
        if (points.Length >= 6)
            CreateTriangle(points[0], points[4], points[5]);

    }

    void AssignVertices(Node[] points) {
        for (int i = 0; i < points.Length; i++) {
            if (points[i].vertexIndex == -1) {
                points[i].vertexIndex = topVertices.Count;
                topVertices.Add(points[i].position);
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c) {
        topTriangles.Add(a.vertexIndex);
        topTriangles.Add(b.vertexIndex);
        topTriangles.Add(c.vertexIndex);

        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddTriangleToDictionary(triangle.vertexIndexA, triangle);
        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
        AddTriangleToDictionary(triangle.vertexIndexC, triangle);
    }

    void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle) {
        if (triangleDictionary.ContainsKey(vertexIndexKey)) {
            triangleDictionary[vertexIndexKey].Add(triangle);
        } else {
            List<Triangle> triangleList = new List<Triangle>();
            triangleList.Add(triangle);
            triangleDictionary.Add(vertexIndexKey, triangleList);
        }
    }

    void CalculateMeshOutlines() {

        for (int vertexIndex = 0; vertexIndex < topVertices.Count; vertexIndex++) {
            if (!checkedVertices.Contains(vertexIndex)) {
                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                if (newOutlineVertex != -1) {
                    checkedVertices.Add(vertexIndex);

                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertexIndex);
                    outlines.Add(newOutline);
                    FollowOutline(newOutlineVertex, outlines.Count - 1);
                    outlines[outlines.Count - 1].Add(vertexIndex);
                }
            }
        }
    }

    void FollowOutline(int vertexIndex, int outlineIndex) {
        outlines[outlineIndex].Add(vertexIndex);
        checkedVertices.Add(vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

        if (nextVertexIndex != -1) {
            FollowOutline(nextVertexIndex, outlineIndex);
        }
    }

    int GetConnectedOutlineVertex(int vertexIndex) {
        List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

        for (int i = 0; i < trianglesContainingVertex.Count; i++) {
            Triangle triangle = trianglesContainingVertex[i];

            for (int j = 0; j < 3; j++) {
                int vertexB = triangle[j];
                if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB)) {
                    if (IsOutlineEdge(vertexIndex, vertexB)) {
                        return vertexB;
                    }
                }
            }
        }

        return -1;
    }

    bool IsOutlineEdge(int vertexA, int vertexB) {
        List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];
        int sharedTriangleCount = 0;

        for (int i = 0; i < trianglesContainingVertexA.Count; i++) {
            if (trianglesContainingVertexA[i].Contains(vertexB)) {
                sharedTriangleCount++;
                if (sharedTriangleCount > 1) {
                    break;
                }
            }
        }
        return sharedTriangleCount == 1;
    }

    struct Triangle {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;
        int[] vertices;

        public Triangle(int a, int b, int c) {
            vertexIndexA = a;
            vertexIndexB = b;
            vertexIndexC = c;

            vertices = new int[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }

        public int this[int i] {
            get {
                return vertices[i];
            }
        }


        public bool Contains(int vertexIndex) {
            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
        }
    }

    public class SquareGrid {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize) {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++) {
                for (int y = 0; y < nodeCountY; y++) {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapHeight / 2 + y * squareSize + squareSize / 2);
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++) {
                for (int y = 0; y < nodeCountY - 1; y++) {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }

        }
    }

    public class Square {

        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centreTop, centreRight, centreBottom, centreLeft;
        public int configuration;

        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft) {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomRight = _bottomRight;
            bottomLeft = _bottomLeft;

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

    }

    public class Node {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 _pos) {
            position = _pos;
        }
    }

    public class ControlNode : Node {

        public bool active;
        public Node above, right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
            active = _active;
            above = new Node(position + Vector3.forward * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
        }

    }
}