using System.Collections.Generic;
using UnityEngine;

public class InLightFieldOfView : MonoBehaviour
{
    public bool isPresent;
    [HideInInspector]
    public GameObject light;
    public int pieceId;
    List<Vector3> vertices;
    int[] triangles;
    Mesh mesh;
    private bool isFacingRight;

    internal bool IsFacingRight { set { isFacingRight = value; } }

    private void LateUpdate()
    {
        isPresent = false;
        CreateMesh();
    }

    public void setPresent()
    {
        isPresent = true;
    }

    public void addVertex(Vector2 vertex)
    {
        if (vertices == null)
        {
            vertices = new List<Vector3>();
        }
        vertices.Add(vertex);
    }

    internal void CreateMesh()
    {
        if (vertices != null && vertices.Count >= 2)
        {
            if (mesh == null)
            {
                mesh = new Mesh();
                GetComponent<MeshFilter>().mesh = mesh;
            }

            int traingleIndex = 0;


            triangles = new int[3 * (vertices.Count - 2)];
            for (int vertexIndex = 0; vertexIndex <= vertices.Count - 4; vertexIndex += 2)
            {
                traingleIndex = 3 * vertexIndex;
                if (isFacingRight)
                {
                    triangles[traingleIndex] = vertexIndex;
                    triangles[traingleIndex + 1] = vertexIndex + 2;
                    triangles[traingleIndex + 2] = vertexIndex + 1;

                    triangles[traingleIndex + 3] = vertexIndex + 1;
                    triangles[traingleIndex + 4] = vertexIndex + 2;
                    triangles[traingleIndex + 5] = vertexIndex + 3;
                }
                else
                {
                    triangles[traingleIndex] = vertexIndex;
                    triangles[traingleIndex + 1] = vertexIndex + 1;
                    triangles[traingleIndex + 2] = vertexIndex + 2;

                    triangles[traingleIndex + 3] = vertexIndex + 1;
                    triangles[traingleIndex + 4] = vertexIndex + 3;
                    triangles[traingleIndex + 5] = vertexIndex + 2;
                }
            }

            mesh.triangles = null;
            mesh.vertices = vertices.ToArray();
            mesh.uv = new Vector2[vertices.Count];
            mesh.triangles = triangles;
            vertices = new List<Vector3>();

        }
    }

}
