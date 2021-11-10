using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkpointId = 0;
    private StageManager stageManager;

    public int CheckpointId { get { return checkpointId; } }

    private void Start() { stageManager = GameManager._StageManager; }

    private void OnTriggerEnter2D(Collider2D collision) { stageManager.changeCurrentCheckpoint(checkpointId); }
}
