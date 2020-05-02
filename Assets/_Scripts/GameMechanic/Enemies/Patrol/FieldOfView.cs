using System;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Mesh mesh;
    public LayerMask whatBlocksRay;
    private bool facingRight = false;
    private float rotationFactorX;
    public float normalViewDistance = 2f;


    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void Update() 
    {
        facingRight = Mathf.Abs(transform.parent.eulerAngles.y) > 0f;

        UpdateNormalFOV();
        UpdateInLightFOV();
    }

    private void UpdateInLightFOV()
    {

    }

    private void UpdateNormalFOV()
    {

        Vector3 origin = Vector3.zero;

        float fovAngle = 90f;
        int rayCount = 30;
        float currentAngle = 45f;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];

        float angleIncrease = fovAngle / rayCount;

        int[] triangles = new int[rayCount * 3 * 2];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= rayCount; i++)
        {
            // local space
            Vector3 vertex = origin + normalViewDistance * SimpleMath.VectorFromAngle(currentAngle);
            rotationFactorX = Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad);
            Vector2 projectedRayDirection = normalViewDistance * new Vector2(
                rotationFactorX * SimpleMath.VectorFromAngle(currentAngle).x,
                SimpleMath.VectorFromAngle(currentAngle).y
            );

            // world space
            RaycastHit2D raycastHit2D = Physics2D.Raycast(
                transform.position,
                projectedRayDirection,
                projectedRayDirection.magnitude,
                whatBlocksRay
            );
            Debug.DrawRay(
                transform.position,
                projectedRayDirection,
                Color.red
            );

            if (raycastHit2D.collider)
            {
                Vector3 vertexInWorldSpace = transform.TransformPoint(vertex);
                Vector3 vertexDirectionInWorldSpace = vertexInWorldSpace - transform.position;
                Vector3 newVertexInWorldSpace = Vector3.zero;
                SimpleMath.LineLineIntersection(
                    out newVertexInWorldSpace,
                    raycastHit2D.point,
                    Vector3.forward,
                    transform.position,
                    vertexDirectionInWorldSpace
                );
                vertex = transform.InverseTransformPoint(newVertexInWorldSpace);
            }
            vertices[vertexIndex] = vertex;
            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;

                if (facingRight)
                {
                    triangles[triangleIndex + 1] = vertexIndex - 1;
                    triangles[triangleIndex + 2] = vertexIndex;
                }
                else
                {
                    triangles[triangleIndex + 1] = vertexIndex;
                    triangles[triangleIndex + 2] = vertexIndex - 1;
                }
                triangleIndex += 3;
            }
            vertexIndex++;
            currentAngle -= angleIncrease;
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
