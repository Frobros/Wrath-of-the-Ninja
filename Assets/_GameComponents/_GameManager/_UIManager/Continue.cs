using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Continue : MonoBehaviour
{
    private AudioManager audioManager;
    private NinjaStatesAnimationSound player;
    public TextMeshProUGUI text;
    public static bool frozen;
    private bool initialize;
    private StageManager stageManager;

    private void Start()
    {
        InitializeComponent();
        stageManager = GameManager._StageManager;
        audioManager = GameManager._AudioManager;
    }

    private void InitializeComponent()
    {
        player = FindObjectOfType<NinjaStatesAnimationSound>();
        text = GetComponent<TextMeshProUGUI>();
        text.text = "";
        frozen = false;
        initialize = false;
    }

    void Update()
    {
        if (player != null && !initialize)
        {
            if (!frozen && player.dead)
            {
                Time.timeScale = 0F;
                frozen = true;
                audioManager.PlaySound("pain", 1);
                stageManager.ReloadScene();
            }
        }
    }


    public void setText()
    {
        text.text = "CONTINUE ?";
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
        InitializeComponent();
    }
}
