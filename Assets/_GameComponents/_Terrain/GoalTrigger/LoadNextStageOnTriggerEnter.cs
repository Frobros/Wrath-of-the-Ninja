using UnityEngine;

public class LoadNextStageOnTriggerEnter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager._StageManager.InitializeNextStage();
        }
    }
}
