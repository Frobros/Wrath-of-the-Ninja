using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    private static bool initializing = false;
    public static bool 
        hasTextSceneEnded,
        paused = false,
        onTitle,
        onControls,
        onIntro,
        onStage,
        onOutro,
        onCredits;

    public static int 
        currentStage = 0,
        currentCheckpoint = 0,
        lastStage = 3;

    public static string activeSceneName;
    private static FadeInAndOutCamera fader;

    private Checkpoint[] checkpoints;

    private void Awake()
    {
        fader = FindObjectOfType<FadeInAndOutCamera>();
    }

    private void Update()
    {
        ProtocolOnTitle();
        ProtocolOnControls();
        ProtocolOnIntro();
        ProtocolOnStage();
        ProtocolOnOutro();
        ProtocolOnCredits();
    }


    private void ProtocolOnTitle()
    {
        if (onTitle && InputManager.confirm && !initializing)
        {
            InputManager.confirm = false;
            initializing = true;
            fader.FadeToNextScene("wotn_controls");
        }

    }

    private void ProtocolOnControls()
    {
        if (onControls)
        {
            if (InputManager.escape)
            {
                InputManager.escape = false;
                fader.FadeToNextScene("wotn_title");
            }
            else if (InputManager.confirm)
            {
                InputManager.confirm = false;
                fader.FadeToNextScene("wotn_intro");
            }
        }
    }


    private void ProtocolOnIntro()
    {
        if (!initializing && onIntro && hasTextSceneEnded)
        {
            initializing = true;
            currentStage = 0;
            fader.FadeToNextScene("wotn_stage_" + currentStage);
        }
    }

    private void ProtocolOnStage()
    {
        if (onStage)
        {
            if (InputManager.reset)
            {
                fader.FadeToNextScene(SceneManager.GetActiveScene().name);
            }
        }
    }
    private void ProtocolOnOutro()
    {
        if (onOutro && hasTextSceneEnded && InputManager.confirm)
        {
            InputManager.confirm = false;
            hasTextSceneEnded = false;
            fader.FadeToNextScene("wotn_credits");
        }
    }

    private void ProtocolOnCredits()
    {
        if (onCredits)
        {
            if (!initializing && (InputManager.escape || InputManager.confirm))
            {
                initializing = true;
                InputManager.escape = false;
                InputManager.confirm = false;
                fader.FadeToNextScene("wotn_title");
            }
        }
    }

    public void IdentifyScene(Scene scene)
    {
        activeSceneName = scene.name;
        onTitle = activeSceneName.Contains("title");
        onControls = activeSceneName.Contains("controls");
        onIntro = activeSceneName.Contains("intro");
        onStage = activeSceneName.Contains("stage");
        onOutro = activeSceneName.Contains("outro");
        onCredits = activeSceneName.Contains("credits");
        if (onStage)
        {
            currentStage = ExtractStageNumber(activeSceneName);
        }
    }

    private int ExtractStageNumber(string activeScene)
    {
        string[] splitSceneName = activeScene.Split('_');
        if (splitSceneName.Length == 3)
            return Int32.Parse(splitSceneName[2]);
        return -1;
    }

    public static void InitializeNextStage()
    {
        if (!initializing)
        {
            initializing = true;
            currentStage++;
            currentCheckpoint = 0;
            if (currentStage > 0 && currentStage <= lastStage)
            {
                fader.FadeToNextScene("wotn_stage_" + currentStage);
            }
            else
            {
                fader.FadeToNextScene("wotn_outro");
            }
        }
    }

    public static void ReloadScene()
    {
        if (!initializing)
        {
            initializing = true;
            fader.FadeToNextScene("wotn_stage_" + currentStage);
        }
    }
    internal static void changeCurrentCheckpoint(int newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
    }

    public static void ExitScene()
    {
        fader.FadeToNextScene("wotn_title");
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
        hasTextSceneEnded = false;
        initializing = false;
        onStage = scene.name.Contains("stage");
        IdentifyScene(scene);

        if (onStage)
        {
            checkpoints = FindObjectsOfType<Checkpoint>();

            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (currentCheckpoint == checkpoint.getCheckpointIdentifier())
                {
                    FindObjectOfType<NinjaMove>().transform.position = checkpoint.transform.position;
                }
            }
        }

        if (fader == null)
        {
            fader = FindObjectOfType<FadeInAndOutCamera>();
        }
    }
}
