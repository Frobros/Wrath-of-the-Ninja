using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public GameObject prefabInLightFieldOfView;
    private Mesh mesh;
    
    public LayerMask whatBlocksRay;
    public LayerMask whatIsSuspicious;
    public LayerMask whatIsLight;


    private bool facingRight = false;
    
    public float normalViewDistance = 2f;
    public float inLightViewDistance = 5f;

    public float fovAngle = 90f;
    public int rayCount = 100;
    private List<GameObject> inLightFieldOfViews;
    public bool detected;
    private bool detectedInlight,
        detectedNormal;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        inLightFieldOfViews = new List<GameObject>();
        // Time.timeScale = 0.2f;
    }

    private void LateUpdate() 
    {
        facingRight = Mathf.Abs(transform.parent.eulerAngles.y) > 0f;
        UpdateInLightFOV();
        UpdateNormalFOV();
        detected = detectedInlight || detectedNormal;
    }

    private void UpdateInLightFOV()
    {
        bool currentlyDetected = false;

        inLightFieldOfViews.RemoveAll(item => item == null);
        inLightFieldOfViews.ForEach(item => item.GetComponent<InLightFieldOfView>().setFacingRight(facingRight));
        Vector2 origin = Vector2.zero;
        float currentAngle = fovAngle / 2f;
        float angleIncrease = fovAngle / rayCount;

        // pieces management
        List<GameObject> previouslyBlockedLights = new List<GameObject>();
        List<GameObject> nowBlockedLights = new List<GameObject>();
        Dictionary<int, int> lightIdsNumberOfPieces = new Dictionary<int, int>();
        
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex = SimpleMath.VectorFromAngle(currentAngle);
            Vector2 projectedRayDirection = Project3DPointOntoXYPlane(vertex);
            float inLightViewRayLength = inLightViewDistance * projectedRayDirection.magnitude;
            Vector2 endOfInLightRay = (Vector2)transform.position + inLightViewDistance * projectedRayDirection;
            Vector2 endOfNormalRay = (Vector2)transform.position + normalViewDistance * projectedRayDirection;
            
            RaycastHit2D extendedRayHitsPlayer = Physics2D.Raycast(
                transform.position,
                projectedRayDirection,
                inLightViewRayLength,
                whatIsSuspicious
            );
            RaycastHit2D extendedRayIsBlocked = Physics2D.Raycast(
                transform.position,
                projectedRayDirection,
                inLightViewRayLength,
                whatBlocksRay
            );
            RaycastHit2D[] extendedRayHitsLights = Physics2D.RaycastAll(
                transform.position,
                projectedRayDirection,
                inLightViewRayLength,
                whatIsLight
            )
                .OrderBy(hit => hit.distance)
                .ToArray();
                        
            RaycastHit2D inverseExtendedRayHitsPlayer;
            RaycastHit2D inverseExtendedRayIsBlocked;
            RaycastHit2D[] inverseExtendedRayHitsLights;
            if (extendedRayHitsLights.Length > 0)
            {
                inverseExtendedRayHitsPlayer = Physics2D.Raycast(
                    endOfInLightRay,
                    -projectedRayDirection,
                    inLightViewRayLength,
                    whatIsSuspicious
                );
                inverseExtendedRayIsBlocked = Physics2D.Raycast(
                    endOfInLightRay,
                    -projectedRayDirection,
                    inLightViewRayLength,
                    whatBlocksRay
                );
                inverseExtendedRayHitsLights = Physics2D.RaycastAll(
                    endOfInLightRay,
                    -projectedRayDirection,
                    inLightViewRayLength,
                    whatIsLight
                );
                foreach (RaycastHit2D enterLightArea in extendedRayHitsLights)
                {
                    // pieces management
                    GameObject light = enterLightArea.collider.gameObject;
                    int lightId = light.GetComponent<Lamp>().lightId;
                    if (!lightIdsNumberOfPieces.ContainsKey(lightId))
                    {
                        lightIdsNumberOfPieces.Add(lightId, 0);
                        previouslyBlockedLights.Add(light);
                    }

                    // light is blocked
                    if (extendedRayIsBlocked.collider && SimpleMath.PointACloserToPointC(extendedRayIsBlocked.point, enterLightArea.point, transform.position))
                    {
                        nowBlockedLights.Add(light);
                        if (!previouslyBlockedLights.Contains(light))
                        {
                            lightIdsNumberOfPieces[lightId]++;
                        }
                    }
                    // light is visible
                    else  {
                        RaycastHit2D exitLightArea = Array.Find<RaycastHit2D>(inverseExtendedRayHitsLights, area => enterLightArea.collider == area.collider);

                        Vector2 startOfSightInLight = enterLightArea.point;
                        Vector2 endOfSightInLight = endOfInLightRay;

                        if (SimpleMath.PointACloserToPointC(startOfSightInLight, endOfNormalRay, transform.position))
                        {
                            startOfSightInLight = endOfNormalRay;
                        }

                        if (extendedRayIsBlocked.collider
                            && (SimpleMath.PointInBound(extendedRayIsBlocked.point, enterLightArea.point, exitLightArea.point)
                            || SimpleMath.PointInBound(inverseExtendedRayIsBlocked.point, enterLightArea.point, exitLightArea.point))
                        )
                        {
                            endOfSightInLight = extendedRayIsBlocked.point;
                        }
                        else if (exitLightArea.collider)
                        {
                            endOfSightInLight = exitLightArea.point;
                        }

                        if (SimpleMath.PointInBound(endOfSightInLight, transform.position, endOfNormalRay))
                        {
                            endOfSightInLight = endOfNormalRay;
                        }

                        Vector3 vertexStart = InverseProject3DPointOntoXYPlane(vertex, startOfSightInLight);
                        Vector3 vertexEnd = InverseProject3DPointOntoXYPlane(vertex, endOfSightInLight);


                        /* if this light was blocked before, create a new InLightFOV,
                         * take the number from the dictionary and set it as piece Id
                         * Afterwards set its mesh's vertices
                         * 
                         * else create a new InLightFOV, if it wasn't created before,
                         * and set it's vertices
                         */
                        GameObject fovGO = inLightFieldOfViews.Find(
                            inLightFieldOfView => inLightFieldOfView.GetComponent<InLightFieldOfView>().light.Equals(light)
                            && inLightFieldOfView.GetComponent<InLightFieldOfView>().pieceId == lightIdsNumberOfPieces[lightId]
                        );
                        if (fovGO == null)
                        {
                            fovGO = Instantiate(prefabInLightFieldOfView, transform);
                            fovGO.GetComponent<InLightFieldOfView>().light = light;
                            fovGO.GetComponent<InLightFieldOfView>().pieceId = lightIdsNumberOfPieces[lightId];
                            fovGO.name = "InLightFOV_" + lightId + "_" + lightIdsNumberOfPieces[lightId];
                            inLightFieldOfViews.Add(fovGO);
                        }
                        fovGO.GetComponent<InLightFieldOfView>().setPresent();
                        fovGO.GetComponent<InLightFieldOfView>().addVertex(vertexStart);
                        fovGO.GetComponent<InLightFieldOfView>().addVertex(vertexEnd);

                        Collider2D hitPlayer = extendedRayHitsPlayer.collider ? extendedRayHitsPlayer.collider : inverseExtendedRayHitsPlayer.collider;
                        if (hitPlayer
                            && (SimpleMath.PointInBound(extendedRayHitsPlayer.point, startOfSightInLight, endOfSightInLight) 
                                || SimpleMath.PointInBound(inverseExtendedRayHitsPlayer.point, startOfSightInLight, endOfSightInLight)))
                        {
                            currentlyDetected = hitPlayer.GetComponent<NinjaStatesAnimationSound>().isDetectableFrom(projectedRayDirection);
                        }
                    }
                }
                previouslyBlockedLights = nowBlockedLights;
                nowBlockedLights = new List<GameObject>();
            } 
            currentAngle -= angleIncrease;
        }
        foreach (GameObject inLightFieldOfView in inLightFieldOfViews)
        {
            if (!inLightFieldOfView.GetComponent<InLightFieldOfView>().isPresent)
            {
                Destroy(inLightFieldOfView);
            }
        }

        detectedInlight = currentlyDetected;
    }

    private void UpdateNormalFOV()
    {
        bool currentlyDetected = false;
        Vector3 origin = Vector3.zero;
        float currentAngle = fovAngle / 2f;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];

        float angleIncrease = fovAngle / rayCount;

        int[] triangles = new int[rayCount * 3 * 2];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex = SimpleMath.VectorFromAngle(currentAngle);
            Vector2 projectedRayDirection = Project3DPointOntoXYPlane(vertex);
            float normalViewRayLength = normalViewDistance * projectedRayDirection.magnitude;

            // world space
            RaycastHit2D raycastHit2D = Physics2D.Raycast(
                transform.position,
                projectedRayDirection,
                normalViewRayLength,
                whatBlocksRay
            );
            RaycastHit2D playerDetected = Physics2D.Raycast(
                transform.position,
                projectedRayDirection,
                normalViewRayLength,
                whatIsSuspicious
            );

            if (raycastHit2D.collider)
            {
                vertex = InverseProject3DPointOntoXYPlane(vertex, raycastHit2D.point);
            }
            else
            {
                vertex = normalViewDistance * vertex.normalized;
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



            if (playerDetected.collider)
            {
                if (!raycastHit2D.collider
                    || (raycastHit2D.collider && SimpleMath.PointACloserToPointC(playerDetected.point, raycastHit2D.point, transform.position)))
                {
                    Debug.Log(playerDetected.collider);
                    currentlyDetected = playerDetected.collider.GetComponent<NinjaStatesAnimationSound>().isDetectableFrom(projectedRayDirection);
                }
            }

            vertexIndex++;
            currentAngle -= angleIncrease;
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        detectedNormal = currentlyDetected;
    }


    private Vector2 Project3DPointOntoXYPlane(Vector3 vertex)
    {
        return (Vector2) (transform.rotation * vertex);
    }

    private Vector3 InverseProject3DPointOntoXYPlane(Vector3 vertex, Vector2 hitpoint)
    {
        Vector3 vertexInWorldSpace = transform.TransformPoint(vertex);
        Vector3 vertexDirectionInWorldSpace = vertexInWorldSpace - transform.position;
        Vector3 newVertexInWorldSpace = Vector3.zero;
        SimpleMath.LineLineIntersection(
            out newVertexInWorldSpace,
            hitpoint,
            Vector3.forward,
            transform.position,
            vertexDirectionInWorldSpace
        );
        return transform.InverseTransformPoint(newVertexInWorldSpace);
    }
}
