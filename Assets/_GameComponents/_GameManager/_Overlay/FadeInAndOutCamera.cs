using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FadeInAndOutCamera : MonoBehaviour
{
    private StageManager stageManager;
    private UIManager ui;
    private Animator animator;
    private Image black;
    private bool colorSet;

    [SerializeField] private float grayScaleSpeed;

    private void Start()
    {
        animator = GetComponent<Animator>();
        black = GetComponent<Image>();
        stageManager = GameManager._StageManager;
        ui = GameManager._UI;
    }

    private void Update()
    {
        if (ui.isPaused && !colorSet)
        {
            animator.SetBool("Overlay", true);
            colorSet = true;
        }
        else if (!ui.isPaused && colorSet)
        {
            animator.SetBool("Overlay", false);
            colorSet = false;
        }
    }

    private IEnumerator FadeOutContinue(string sceneName)
    {
        GameManager.Pause();
        FindObjectOfType<Continue>().setText();
        Volume gloabalVolume = FindObjectOfType<Volume>();

        gloabalVolume.profile.TryGet<ColorAdjustments>(out var colors);

        float grayScale =  0;
        yield return new WaitUntil(() => {
            if (grayScale >= -100)
            {
                grayScale -= grayScaleSpeed;
                colors.saturation.value = grayScale;
            }
            return GameManager._Input.jump;
        });
        animator.SetBool("FadeOut", true);
        yield return new WaitUntil(() => black.color.a == 1);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        GameManager.Pause(false);
    }

    private IEnumerator FadeOut(string sceneName)
    {
        GameManager.Pause();
        animator.SetBool("FadeOut", true);
        yield return new WaitUntil(() => black.color.a == 1);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        GameManager.Pause(false);
    }

    public void FadeToNextScene(string sceneName)
    {
        if (sceneName.Contains("stage") 
            && stageManager.IsOnStage
            && FindObjectOfType<NinjaStatesAnimationSound>().dead
        ) {
            StartCoroutine(FadeOutContinue(sceneName));
        } else
        {
            StartCoroutine(FadeOut(sceneName));
        }
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
        if (animator == null) animator = GetComponent<Animator>();
        animator.SetBool("FadeOut", false);
    }
}
