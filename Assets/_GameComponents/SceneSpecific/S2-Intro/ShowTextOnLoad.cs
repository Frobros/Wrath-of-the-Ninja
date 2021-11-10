using UnityEngine;

public class ShowTextOnLoad : MonoBehaviour
{
    private bool hasTextSceneEnded;
    private StageManager stageManager;
    private TextBoxManager textBoxManager;

    private void Start()
    {
        stageManager = GameManager._StageManager;
        textBoxManager = GameManager._TextBoxManager;
        textBoxManager.EnableTextBox("intro");
    }

    private void Update()
    {
        if (textBoxManager.hasConversationEnded() && !hasTextSceneEnded)
        {
            hasTextSceneEnded = true;
            stageManager.TextSceneHasEnded = hasTextSceneEnded;
        }
    }
}
