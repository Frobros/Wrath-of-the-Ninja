using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// This is a Singleton
public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    private AudioManager audioManager;
    private InputManager input;
    private StageManager stageManager;
    private TextBoxManager textBoxManager;
    private UIManager ui;
    private bool isStopped;

    public static AudioManager _AudioManager { get { return instance.audioManager; } }
    public static InputManager _Input { get { return instance.input; } }
    public static StageManager _StageManager { get { return instance.stageManager; } }
    public static TextBoxManager _TextBoxManager { get { return instance.textBoxManager; } }
    public static UIManager _UI { get { return instance.ui; } }
    public static bool IsStopped { get { return instance.isStopped; } }
    public static bool IsPaused { get { return instance.ui.isPaused; } }

    // Singleton pattern
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;

        audioManager = GetComponentInChildren<AudioManager>();
        input = GetComponentInChildren<InputManager>();
        stageManager = GetComponentInChildren<StageManager>();
        textBoxManager = GetComponentInChildren<TextBoxManager>();
        ui = GetComponentInChildren<UIManager>();
    }

    // The following Methods handle the event of a freshly loaded scene
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
        audioManager.OnLevelFinishedLoading(scene);
        StartCoroutine(ActivateControlsIn(1F));
        Time.timeScale = 1f;
    }

    private IEnumerator ActivateControlsIn(float v)
    {
        yield return new WaitForSeconds(v);
        input.active = true;
    }

    internal static void Pause(bool pause = true)
    {
        instance.isStopped = pause;
        Time.timeScale = instance.isStopped ? 0f : 1f;
    }
}
