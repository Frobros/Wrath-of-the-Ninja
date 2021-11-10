using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject pauseButton;
    public GameObject pauseMenu;
    public GameObject resumeButton;
    public GameObject textBox;

    private StageManager stageManager;

    private void Start() { stageManager = GameManager._StageManager; }

    private void Update()
    {
        if (GameManager._Input.escape)
        {
            if (isPaused)
            {
                OnResume();
            }
            else
            {
                OnPause();
            }
        }
    }

    public void OnPause()
    {
        if (!Continue.frozen)
        {
            isPaused = true;
            if (!TouchBehaviour.active) 
                EventSystem.current.SetSelectedGameObject(resumeButton);
            pauseMenu.SetActive(true);
            pauseButton.SetActive(false);
            Time.timeScale = 0F;
        }
    }

    public void OnResume()
    {
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        isPaused = false;
        Time.timeScale = 1F;
    }

    public void OnRetry()
    {
        OnResume();
        stageManager.ReloadScene();
    }

    public void OnExit()
    {
        OnResume();
        stageManager.ExitScene();
    }
}
