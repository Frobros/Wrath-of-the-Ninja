using System;
using UnityEngine;

public class SecurityWatch : MonoBehaviour {
    private NinjaStatesAnimationSound player;
    public LayerMask whatIsPlayer, whatBlocksRay, whatIsLight;
    public GameObject line;
    private Vector2[] detectionRays;
    private LineRenderer[] detectionLines;
    public float 
        angle,
        fNormalRayDepth,
        fExtendedRayDepth;
    public int numberRays;
    public bool 
        playerDetected = false,
        initialize;
    
    void Start ()
    {
        initializeFOV();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<NinjaStatesAnimationSound>();
    }

    void Update () {
        DrawRays();
        DestroyPlayer();
        if (initialize)
        {
            initializeFOV();
        }
    }


    private void initializeFOV() {
        detectionRays = new Vector2[numberRays];
        detectionLines = new LineRenderer[numberRays];
        float step = angle / (numberRays-1);
        float alpha = - 0.5f * angle;
        for (int i = 0; i < numberRays; i++, alpha += step)
        {
            float x = Mathf.Cos(alpha * Mathf.Deg2Rad);
            float y = Mathf.Sin(alpha * Mathf.Deg2Rad);
            Vector2 direction = new Vector2(x, y);
            detectionRays[i] = direction;
            // detectionLines[i] = Instantiate(line,transform).GetComponent<LineRenderer>();
        }
    }
    
    private void DrawRays()
    {
        for (int i = 0; i < numberRays; i++)
        {
            Vector2 origin = transform.position;
            Vector2 direction = transform.TransformDirection(detectionRays[i]);
            RaycastHit2D normalRayHitsPlayer = Physics2D.Raycast(origin, direction, fNormalRayDepth, whatIsPlayer.value);
            RaycastHit2D normalRayIsBlocked = Physics2D.Raycast(origin, direction, fNormalRayDepth, whatBlocksRay.value);
            RaycastHit2D extendedRayHitsPlayer = Physics2D.Raycast(origin, direction, fExtendedRayDepth, whatIsPlayer.value);
            RaycastHit2D extendedRayIsBlocked = Physics2D.Raycast(origin, direction, fExtendedRayDepth, whatBlocksRay.value);
            RaycastHit2D[] extendedRayHitsLights = Physics2D.RaycastAll(origin, direction, fExtendedRayDepth, whatIsLight.value);
            
            RaycastHit2D inverseExtendedRayHitsPlayer;
            RaycastHit2D inverseExtendedRayIsBlocked;
            RaycastHit2D[] inverseExtendedRayHitsLights;
            
            // In english: if light area is hit before it is blocked
            if (extendedRayHitsLights.Length > 0 &&
                !(extendedRayIsBlocked.collider && isPointBeforeLightArea(origin, extendedRayIsBlocked.point, extendedRayHitsLights))) 
            {
                inverseExtendedRayHitsPlayer = Physics2D.Raycast(
                    origin + fExtendedRayDepth * direction,
                    -direction,
                    fExtendedRayDepth,
                    whatIsPlayer
                );

                inverseExtendedRayIsBlocked = Physics2D.Raycast(
                    origin + fExtendedRayDepth * direction,
                    -direction,
                    fExtendedRayDepth,
                    whatBlocksRay
                );
                inverseExtendedRayHitsLights = Physics2D.RaycastAll(
                    origin + fExtendedRayDepth * direction,
                    -direction,
                    fExtendedRayDepth,
                    whatIsLight
                );
                
                foreach (RaycastHit2D enterLightArea in extendedRayHitsLights)
                {
                    RaycastHit2D exitLightArea = Array.Find<RaycastHit2D>(inverseExtendedRayHitsLights, area => enterLightArea.collider == area.collider);

                    if (extendedRayHitsPlayer.collider 
                        && (SimpleMath.PointInBound(extendedRayHitsPlayer.point, enterLightArea.point, exitLightArea.point)
                        || SimpleMath.PointInBound(inverseExtendedRayHitsPlayer.point, enterLightArea.point, exitLightArea.point)))
                    {
                        Debug.DrawRay(
                            origin,
                            extendedRayHitsPlayer.point - origin,
                            Color.red
                        );
                    }
                    else if (extendedRayIsBlocked.collider
                        && (SimpleMath.PointInBound(extendedRayIsBlocked.point, enterLightArea.point, exitLightArea.point)
                        || SimpleMath.PointInBound(inverseExtendedRayIsBlocked.point, enterLightArea.point, exitLightArea.point)))
                    {
                        Debug.DrawRay(
                            enterLightArea.point,
                            extendedRayIsBlocked.point - enterLightArea.point,
                            Color.blue
                        );
                    }
                    else if (exitLightArea.collider)
                    {
                        Debug.DrawRay(
                            enterLightArea.point,
                            exitLightArea.point - enterLightArea.point,
                            Color.blue
                        );
                    }
                }
            }

            if (normalRayIsBlocked.collider)
            {
                Debug.DrawRay(
                    origin,
                    normalRayIsBlocked.point - origin,
                    Color.green
                );
            }
            else if (normalRayHitsPlayer.collider)
            {
                Debug.DrawRay(
                    origin,
                    normalRayHitsPlayer.point - origin,
                    Color.red
                );
            }
            else
            {
                Debug.DrawRay(
                    origin,
                    direction * fNormalRayDepth,
                    Color.green
                );
            }
        }

    }

    private bool isPointBeforeLightArea(Vector2 origin, Vector2 point, RaycastHit2D[] extendedRayHitsLights)
    {
        foreach (RaycastHit2D raycastHit2D in extendedRayHitsLights)
        {
            if (!SimpleMath.PointInBound(point, origin, raycastHit2D.point))
                return false;
        }
        return true;
    }

    private void DestroyPlayer()
    {
        if (playerDetected)
            player.KillNinja();
    }

    public bool isPlayerDetected()
    {
        return playerDetected;
    }
}