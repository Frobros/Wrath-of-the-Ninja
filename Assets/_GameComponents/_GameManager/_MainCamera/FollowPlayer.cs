using UnityEngine;
using UnityEngine.SceneManagement;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;
    private Vector3 currentVelocity = Vector3.zero;
    private Bounds[] cameraBounds;
    private Bounds[] adjustedBounds;

    [SerializeField] private float smoothTime = 20;
    [SerializeField] private float velocity = 0.0F;
    private bool onStage;

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        onStage = scene.name.Contains("stage");

        if (onStage)
        {
            player = FindObjectOfType<NinjaStatesAnimationSound>().transform;
            initializeBounds();
            transform.position = NextPosition();
        }
    }

    void FixedUpdate()
    {
        if (onStage && adjustedBounds != null && player != null)
        {
            DebugBounds();
            rolling();
        }
    }

    private void initializeBounds()
    {
        cameraBounds = FindObjectOfType<CameraBound>().Bounds   ;
        adjustedBounds = new Bounds[cameraBounds.Length];
        int index = 0;
        foreach (Bounds bounds in cameraBounds)
        {
            adjustedBounds[index] = SharedCameraFunctions.adjustedBounds(bounds.min, bounds.max);
            index++;
        }
    }

    public void rolling()
    {
        Vector3 nextPosition = NextPosition();
        transform.position = Vector3.SmoothDamp(transform.position, nextPosition, ref currentVelocity, smoothTime);
    }

    public Vector3 NextPosition()
    {
        Vector3 nextPosition = player.position;
        nextPosition.z = transform.position.z;
        bool wasBoundFound = false;
        for (int i = 0; i < cameraBounds.Length; i++)
        {
            if (SharedCameraFunctions.positionInBounds(cameraBounds[i], player.position))
            {
                Vector2
                    min = adjustedBounds[i].min,
                    max = adjustedBounds[i].max;

                nextPosition = new Vector3(
                    nextPosition.x < min.x
                    ? min.x : nextPosition.x > max.x
                        ? max.x : nextPosition.x,
                    nextPosition.y < min.y
                        ? min.y : nextPosition.y > max.y
                            ? max.y : nextPosition.y,
                    transform.position.z
                );
                wasBoundFound = true;
                break;
            }
        }
        return wasBoundFound ? nextPosition : transform.position;
    }
    private void DebugBounds()
    {
        foreach (Bounds bounds in adjustedBounds)
        {
            Debug.DrawLine(bounds.min, bounds.min + new Vector3(2F * bounds.extents.x, 0, 0), Color.green);
            Debug.DrawLine(bounds.min + new Vector3(0, 2F * bounds.extents.y, 0), bounds.max, Color.green);
            Debug.DrawLine(bounds.max, bounds.max - new Vector3(0, 2F * bounds.extents.y, 0), Color.green);
            Debug.DrawLine(bounds.min, bounds.min + new Vector3(0, 2F * bounds.extents.y, 0), Color.green);
        }
    }
}
