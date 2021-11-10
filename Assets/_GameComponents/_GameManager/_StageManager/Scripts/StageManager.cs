using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    private bool isLoading = false;
    private bool 
        hasTextSceneEnded,
        onTitle,
        onControls,
        onIntro,
        onStage,
        onOutro,
        onCredits;

    public int 
        currentStage = 0,
        currentCheckpoint = 0,
        lastStage = 3;

    private string activeSceneName;
    private FadeInAndOutCamera fader;

    private Checkpoint[] checkpoints;
    private InputManager input;

    public bool TextSceneHasEnded { set { hasTextSceneEnded = value; } }
    public bool IsOnStage { get { return onStage; } }

    private void Start()
    {
        fader = FindObjectOfType<FadeInAndOutCamera>();
        input = GameManager._Input;
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
        if (onTitle && input.confirm && !isLoading)
        {
            input.confirm = false;
            isLoading = true;
            fader.FadeToNextScene(Scenes.CONTROLS);
        }

    }

    private void ProtocolOnControls()
    {
        if (onControls)
        {
            if (input.escape)
            {
                input.escape = false;
                fader.FadeToNextScene(Scenes.TITLE);
            }
            else if (input.confirm)
            {
                input.confirm = false;
                fader.FadeToNextScene(Scenes.INTRO);
            }
        }
    }


    private void ProtocolOnIntro()
    {
        if (!isLoading && onIntro && hasTextSceneEnded)
        {
            isLoading = true;
            currentStage = 0;
            fader.FadeToNextScene(Scenes.STAGES[currentStage]);
        }
    }

    private void ProtocolOnStage()
    {
        if (onStage)
        {
            if (input.reset)
            {
                fader.FadeToNextScene(Scenes.STAGES[currentStage]);
            }
        }
    }
    private void ProtocolOnOutro()
    {
        if (onOutro && hasTextSceneEnded && input.confirm)
        {
            input.confirm = false;
            hasTextSceneEnded = false;
            fader.FadeToNextScene(Scenes.CREDITS);
        }
    }

    private void ProtocolOnCredits()
    {
        if (onCredits)
        {
            if (!isLoading && (input.escape || input.confirm))
            {
                isLoading = true;
                input.escape = false;
                input.confirm = false;
                fader.FadeToNextScene(Scenes.INTRO);
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
        string[] stages = Scenes.STAGES;
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i] == activeScene)
            {
                return i;
            }
        }
        return -1;
    }

    public void InitializeNextStage()
    {
        if (!isLoading)
        {
            isLoading = true;
            currentStage++;
            currentCheckpoint = 0;
            if (currentStage > 0 && currentStage <= lastStage)
            {
                fader.FadeToNextScene(Scenes.STAGES[currentStage]);
            }
            else
            {
                fader.FadeToNextScene(Scenes.CREDITS);
            }
        }
    }

    public void ReloadScene()
    {
        if (!isLoading)
        {
            isLoading = true;
            fader.FadeToNextScene(Scenes.STAGES[currentStage]);
        }
    }
    internal void changeCurrentCheckpoint(int newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
    }

    public void ExitScene()
    {
        fader.FadeToNextScene(Scenes.TITLE);
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
        isLoading = false;
        onStage = scene.name.Contains("stage");
        IdentifyScene(scene);

        if (onStage)
        {
            checkpoints = FindObjectsOfType<Checkpoint>();

            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (currentCheckpoint == checkpoint.CheckpointId)
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
