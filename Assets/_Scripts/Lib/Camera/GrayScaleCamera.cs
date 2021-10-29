using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class GrayScaleCamera : MonoBehaviour
{
    public Material grayscaleMaterial;
    private NinjaStatesAnimationSound player;
    public bool grayedOut = false;
    [Range(0f, 1f)]
    public float grayscale;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (player != null && player.dead && !grayedOut)
        {
            Debug.Log("DEAD!");
            grayedOut = true;
            grayscaleMaterial.SetFloat("_BLACK_AND_WHITE", 1);
        }
        else
        {
            Debug.Log("NOT DEAD YET!");
            grayscaleMaterial.SetFloat("_BLACK_AND_WHITE", grayscale);
        }
        Graphics.Blit(source, destination, grayscaleMaterial);
    }


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
        player = FindObjectOfType<NinjaStatesAnimationSound>();
        grayedOut = false;
        grayscaleMaterial.SetFloat("_BLACK_AND_WHITE", 1);
        Debug.Log("RESET GRAY SCALE");
    }
}