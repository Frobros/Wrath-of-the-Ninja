﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class EnableOnStage : MonoBehaviour
{
    public GameObject[] onlyDisplayOnStage;

    void Awake()
    {
        OnlyDiplayOnScene(SceneManager.GetActiveScene().name);
    }

    private void OnlyDiplayOnScene(string sceneName)
    {
        bool onStage = sceneName.Contains("stage");
        foreach (GameObject go in onlyDisplayOnStage)
        {
            go.SetActive(onStage);
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
        OnlyDiplayOnScene(scene.name);
    }
}
