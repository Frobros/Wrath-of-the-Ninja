using UnityEngine;

public class SecurityWatch : MonoBehaviour {
    private NinjaStatesAnimationSound player;
    public LayerMask whatIsDetectable, whatBlocksRay, whatIsLight;
    public GameObject line;
    private Vector2[] detectionRays;
    private LineRenderer[] detectionLines;
    public float 
        angle,
        maxDepth,
        maxDepthWithLight;
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
            detectionLines[i] = Instantiate(line,transform).GetComponent<LineRenderer>();
        }
    }
    
    private void DrawRays()
    {
        bool detected = false;

        for (int i = 0; i < numberRays; i++)
        {
            Vector2 direction = transform.TransformDirection(detectionRays[i]);
            RaycastHit2D normalRayHitsPlayer = Physics2D.Raycast(transform.position, direction, maxDepth, whatIsDetectable.value);
            RaycastHit2D normalRayIsBlocked = Physics2D.Raycast(transform.position, direction, maxDepth, whatBlocksRay.value);
            RaycastHit2D[] extendedRayHitsLights = Physics2D.RaycastAll(transform.position, direction, maxDepthWithLight, whatIsLight.value);

            if (normalRayHitsPlayer.collider)
            {
                Debug.DrawRay(
                    transform.position, 
                    (Vector3) normalRayHitsPlayer.point - transform.position, 
                    Color.red
                );
            } else if (normalRayIsBlocked.collider)
            {
                Debug.DrawRay(
                    transform.position,
                    (Vector3) normalRayIsBlocked.point - transform.position,
                    Color.green
                );
            }
            else if (extendedRayHitsLights.Length > 0)
            {
                // for each lightend area, find exit point

                foreach (RaycastHit2D hitsLightArea in extendedRayHitsLights)
                {
                    Vector2 plusDirection = hitsLightArea.point - (Vector2)transform.position;
                    Debug.DrawRay(
                        transform.position,
                        plusDirection,
                        Color.green
                    );
                    Debug.DrawRay(
                        hitsLightArea.point,
                        (maxDepth - plusDirection.magnitude) * direction,
                        Color.yellow
                    );
                }
            }
            else
            {
                Debug.DrawRay(
                    transform.position,
                    direction * maxDepth,
                    Color.green
                );
            }
        }
        /*

        for (int i = 0; i < numberRays; i++)
        {
            Vector2 direction = transform.TransformDirection(detectionRays[i]);
            RaycastHit2D hitDetectable = Physics2D.Raycast(transform.position, direction, maxDepth, whatIsDetectable.value);
            RaycastHit2D hitBlocker = Physics2D.Raycast(transform.position, direction, maxDepth, whatBlocksRay.value);
            if ((hitBlocker.collider && hitDetectable.collider && hitDetectable.distance < hitBlocker.distance
                || !hitBlocker.collider && hitDetectable.collider)
                && hitDetectable.transform.GetComponent<NinjaStatesAnimationSound>().isDetectable(direction))
            {
                direction = hitDetectable.point - (Vector2) transform.position;
                detectionLines[i].SetPosition(0, transform.position);
                detectionLines[i].SetPosition(1, transform.position + (Vector3) direction);
                Debug.DrawRay(transform.position, direction, Color.red);
                //Debug.Log("Player at " + hitDetectable.point);
                detected = true;
            } else
            {
                direction = hitBlocker.collider ? hitBlocker.point - (Vector2)transform.position : maxDepth * SimpleMath.Normalize2D(direction);
                detectionLines[i].SetPosition(0, transform.position - Vector3.forward);
                detectionLines[i].SetPosition(1, transform.position + (Vector3)direction - Vector3.forward);
                Debug.DrawRay(transform.position, direction, Color.green);
            }
        }
        playerDetected = detected;
        */
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