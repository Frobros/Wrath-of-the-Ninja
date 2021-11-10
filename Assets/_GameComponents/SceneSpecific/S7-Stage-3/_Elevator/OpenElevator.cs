using UnityEngine;


public class OpenElevator : MonoBehaviour {
    private InputManager input;

    private void Start()
    {
        input = GameManager._Input;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (input.up)
        {
            GameManager._StageManager.InitializeNextStage();
        }
    }
}
